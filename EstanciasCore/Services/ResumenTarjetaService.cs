using DAL.Data;
using DAL.DTOs.Reportes;
using DAL.DTOs.Servicios;
using DAL.Mobile;
using DAL.Models;
using DAL.Models.Core;
using EstanciasCore.API.Controllers.Billetera;
using EstanciasCore.Interface;
using EstanciasCore.Services;
using iText.Html2pdf;
using iText.Kernel.Colors;
using iText.Kernel.Events;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class ResumenTarjetaService : IResumenTarjetaService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;

    public ResumenTarjetaService(IServiceScopeFactory scopeFactory, IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _configuration = configuration;
    }

    public async Task<bool> GenerarResumenTarjetas()
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            // --- 1. Configuración del Proceso ---
            var tamanoLote = _configuration.GetValue<int>("ProcesoResumen:TamanoLote", 100);
            var limiteConcurrencia = _configuration.GetValue<int>("ProcesoResumen:LimiteConcurrencia", 10);

            var resumenesGenerados = new ConcurrentBag<ResumenTarjeta>();
            var usuariosFallidos = new ConcurrentBag<(string UsuarioId, string Error)>();

            // --- 2. OBTENER O CREAR EL PERIODO ACTUAL ---
            Periodo periodo;
            DateTime fechaActual = DateTime.Now.AddDays(-1);
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<EstanciasContext>();

                // Buscamos si ya existe un período para la fecha actual.
                periodo = await context.Periodo.FirstOrDefaultAsync(p => fechaActual >= p.FechaDesde && fechaActual <= p.FechaHasta);

                if (periodo == null)
                {
                    // Si no existe, lo creamos para el mes actual.                    
                    var FechaDesde = fechaActual.AddMonths(-1).AddDays(1);
                    var FechaHasta = fechaActual;
                    DateTime proximoMes = fechaActual.AddMonths(1);
                    DateTime fechaDeVencimiento = new DateTime(proximoMes.Year, proximoMes.Month, 15);
                    periodo = new Periodo
                    {
                        Descripcion = fechaActual.ToString("MMMM yyyy", new CultureInfo("es-ES")), // ej. "agosto 2025"
                        FechaDesde = FechaDesde,
                        FechaHasta = FechaHasta,
                        Activo = true,
                        FechaVencimiento = fechaDeVencimiento
                    };
                    context.Add(periodo);
                    await context.SaveChangesAsync(); // Guardamos inmediatamente para que tenga un Id
                }
            }

            // --- 3. Obtención del Total de Usuarios (usando un scope temporal) ---
            int totalUsuarios;
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<EstanciasContext>();
                totalUsuarios = await context.Usuarios
                    .Include(u => u.Personas)
                    .CountAsync(u => u.Personas != null &&
                                     !string.IsNullOrEmpty(u.Personas.NroTarjeta) &&
                                     !string.IsNullOrEmpty(u.Personas.NroDocumento));
            }

            if (totalUsuarios == 0)
            {
                return false;
            }

            var totalLotes = (int)Math.Ceiling((double)totalUsuarios / tamanoLote);

            // --- 3. Procesamiento en Lotes Paralelos ---
            using (var semaphore = new SemaphoreSlim(limiteConcurrencia))
            {
                for (int i = 0; i < totalLotes; i++)
                {
                    // Obtenemos los usuarios del lote en su propio scope para mantener el contexto corto
                    List<UsuarioParaProcesarDTO> usuariosDelLote;
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<EstanciasContext>();
                        usuariosDelLote = await context.Usuarios
                            .Include(u => u.Personas)
                            .Where(u => u.Personas != null &&
                                        !string.IsNullOrEmpty(u.Personas.NroTarjeta) &&
                                        !string.IsNullOrEmpty(u.Personas.NroDocumento))
                            .OrderBy(u => u.Id)
                            .Skip(i * tamanoLote)
                            .Take(tamanoLote)
                            .Select(u => new UsuarioParaProcesarDTO
                            {
                                Id = u.Id,
                                NombreCompleto = u.Personas.GetNombreCompleto(),
                                NroDocumento = u.Personas.NroDocumento,
                                NroTarjeta = u.Personas.NroTarjeta,
                                UserName = u.UserName
                            })
                            .ToListAsync();
                    }

                    // Creamos una tarea por cada usuario del lote
                    var tasks = usuariosDelLote.Select(usuario => ProcessarUsuarioAsync(usuario, semaphore, resumenesGenerados, usuariosFallidos));
                    await Task.WhenAll(tasks);
                }
            }

            // --- 4. Guardado de Resultados ---
            using (var scope = _scopeFactory.CreateScope())
            {
                var finalContext = scope.ServiceProvider.GetRequiredService<EstanciasContext>();

                // 1. Agrega los resúmenes que se generaron correctamente
                await finalContext.AddRangeAsync(resumenesGenerados);

                // 2. Crea el objeto de log principal
                var logProcedimiento = new LogProcedimientos()
                {
                    Nombre = "Generación de Resumen Tarjeta",
                    Fecha = DateTime.Now,
                    StatusCode = "200",
                    RegistrosCreados = resumenesGenerados.Count(),
                    RegistrosConErrores = usuariosFallidos.Count(),
                    Tiempo = stopwatch.ElapsedMilliseconds,
                };

                // 3. Crea la lista de logs de errores (sin asignar el Id manualmente)
                var logErrores = usuariosFallidos.Select(x => new LogResumenesTarjetas()
                {
                    Fecha = DateTime.Now,
                    Mensaje = x.Error,
                    UsuarioId = x.UsuarioId
                }).ToList();

                // 4. ¡LA MAGIA! Asigna la colección de errores a la propiedad de navegación del log principal
                logProcedimiento.DetalleErrores = logErrores;

                // 5. Agrega el log principal al contexto. EF entenderá que también debe agregar los errores asociados.
                await finalContext.AddAsync(logProcedimiento);

                // 6. Guarda todo en UNA SOLA transacción.
                await finalContext.SaveChangesAsync();
            }

            stopwatch.Stop();
            return true;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            return false;
        }
    }

    private async Task ProcessarUsuarioAsync(UsuarioParaProcesarDTO usuario, SemaphoreSlim semaphore, ConcurrentBag<ResumenTarjeta> resumenes, ConcurrentBag<(string, string)> fallidos)
    {
        await semaphore.WaitAsync();
        try
        {
            var numeroTarjetaLimpio = usuario.NroTarjeta.Trim();

            if (!long.TryParse(numeroTarjetaLimpio, out long numeroTarjetaConvertido))
            {
                // El número de tarjeta no es un número válido.
                // Lo registramos como fallido y salimos de la tarea para este usuario.
                var errorMsg = $"El Nro. de Tarjeta '{numeroTarjetaLimpio}' no tiene un formato numérico válido.";
                fallidos.Add((usuario.Id, errorMsg));
                return; // Importante: Salir temprano para no continuar con este usuario.
            }

            // 1. CREAMOS UN SCOPE NUEVO Y AISLADO PARA ESTA TAREA
            using (var scope = _scopeFactory.CreateScope())
            {
                decimal MontoCuota = 0;
                decimal MontoProximaCuota = 0;
                decimal MontoPunitorios = 0;
                decimal DeudaTotal = 0;
                decimal TotalRedondeo = 0;
                decimal MontoDisponible = 0;
                List<MovimientoTarjetaDTO> comprasAgrupadas = new List<MovimientoTarjetaDTO>();
                //DateTime fechaActual = new DateTime(2025, 7, 25); //Cambiar para modo Prueba
                DateTime fechaActual = DateTime.Now;
                int diasEnMes = DateTime.DaysInMonth(fechaActual.Year, fechaActual.Month);
                DateTime fechaActualCuotas = new DateTime(fechaActual.Year, fechaActual.Month, diasEnMes);
                DateTime fechaActualCuotasProximo = fechaActualCuotas.AddMonths(1);

                // 2. OBTENEMOS LOS SERVICIOS Y EL DBCONTEXT DE ESTE SCOPE
                var scopedDatosServices = scope.ServiceProvider.GetRequiredService<IDatosTarjetaService>();
                var scopedContext = scope.ServiceProvider.GetRequiredService<EstanciasContext>();

                // 3. OBTENEMOS LOS DATOS DE CONFIGURACIÓN DENTRO DEL MISMO SCOPE
                //    Esto es más seguro que pasarlos desde fuera.
                DatosEstructura datosEstructura = await scopedContext.DatosEstructura.FirstOrDefaultAsync();                
                Periodo periodo = await scopedContext.Periodo.FirstOrDefaultAsync(p => fechaActual >= p.FechaDesde && fechaActual <= p.FechaHasta);

                if (datosEstructura == null || periodo == null)
                {
                    fallidos.Add((usuario.Id, "No se encontraron datos de estructura o período válidos."));
                    return; // Salimos si falta configuración crítica
                }

                // 4. EJECUTAMOS TU LÓGICA DE NEGOCIO ORIGINAL
                var datosMovimientos = await scopedDatosServices.ConsultarMovimientos(datosEstructura.UsernameWS, datosEstructura.PasswordWS, usuario.NroDocumento, numeroTarjetaConvertido, 100, 0);

                if (datosMovimientos.Detalle.Resultado == "EXITO")
                {
                    CultureInfo.CurrentCulture = new CultureInfo("es-AR");

                    //Monto Disponible
                    MontoDisponible = Math.Round(Convert.ToDecimal(datosMovimientos.Detalle.MontoDisponible.Replace(".", ",")), 2);

                    //Calcula Cuota del Mes
                    var datosResumen = await scopedDatosServices.CuotasDetallesResumen(datosMovimientos, fechaActual);

                    //Calculo de Punitorios
                    var datosResumenConPunitorios = scopedDatosServices.CalcularPunitoriosResumen(datosResumen).Result;



                    var datosParaResumenDTO = (TempalteResumenDTO)await scopedDatosServices.PrepararDatosResumen(datosMovimientos, datosResumen, periodo, usuario);
                    if (datosParaResumenDTO == null) return;

                    var html = await scopedDatosServices.RenderViewToStringAsync("ResumenBancarioTemplate", datosParaResumenDTO);

                    byte[] pdfBytesPDF;
                    using (var memoryStream = new MemoryStream())
                    {
                        HtmlConverter.ConvertToPdf(html, memoryStream);
                        pdfBytesPDF = memoryStream.ToArray();
                    }

                    var codigo = common.Encrypt(datosParaResumenDTO.NroDocumento, fechaActual.ToString("ddMMyyyy"));

                    // 5. CREAMOS EL RESUMEN USANDO IDs, NO ENTIDADES COMPLETAS
                    var resumenTarjeta = new ResumenTarjeta
                    {
                        Fecha = fechaActual,
                        FechaVencimiento = periodo.FechaVencimiento,
                        Monto = datosParaResumenDTO.SaldoActual,
                        MontoAdeudado = datosParaResumenDTO.SaldoAnterior,
                        NroComprobante = codigo,
                        Adjunto = pdfBytesPDF,
                        PeriodoId = periodo.Id,
                        UsuarioId = usuario.Id
                    };
                    resumenes.Add(resumenTarjeta);
                }
            }
        }
        catch (Exception ex)
        {
            fallidos.Add((usuario.Id, ex.Message));
        }
        finally
        {
            semaphore.Release();
        }
    }


}



