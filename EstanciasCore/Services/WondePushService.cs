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
using System.Net;
using System.Threading;
using System.Threading.Tasks;

public class WondePushService : IWondePushService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;

    public WondePushService(IServiceScopeFactory scopeFactory, IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _configuration = configuration;
    }

    public async Task<bool> EnviarNotificacionPorCumpleanios()
    {
        DatosEstructura datosEstructura = null;
        var stopwatch = Stopwatch.StartNew();
        try
        {
            // --- 1. Configuración del Proceso ---
            var tamanoLote = _configuration.GetValue<int>("ProcesoResumen:TamanoLote", 100);
            var limiteConcurrencia = _configuration.GetValue<int>("ProcesoResumen:LimiteConcurrencia", 10);
            DateTime fechaActual = DateTime.Now.AddDays(5);

            var resumenesGenerados = new ConcurrentBag<ResumenTarjeta>();
            var usuariosFallidos = new ConcurrentBag<(string UsuarioId, string Error)>();       

            // --- 3. Obtención del Total de Usuarios (usando un scope temporal) ---
            int totalUsuarios;
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<EstanciasContext>();
                totalUsuarios = await context.Usuarios.Where(x => x.Personas.FechaNacimiento.Value.Day == fechaActual.Day && x.Personas.FechaNacimiento.Value.Month == fechaActual.Month)
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
                    List<UsuarioParaProcesarWonderPushDTO> usuariosDelLote;
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<EstanciasContext>();
                        usuariosDelLote = await context.Usuarios
                            .Include(u => u.Personas)
                            .Where(u => u.Personas != null &&
                                        !string.IsNullOrEmpty(u.Personas.NroTarjeta) &&
                                        !string.IsNullOrEmpty(u.Personas.NroDocumento))
                            .Where(x => x.Personas.FechaNacimiento.Value.Day == fechaActual.Day && x.Personas.FechaNacimiento.Value.Month == fechaActual.Month)
                            .OrderBy(u => u.Id)
                            .Skip(i * tamanoLote)
                            .Take(tamanoLote)
                            .Select(u => new UsuarioParaProcesarWonderPushDTO
                            {
                                Id = u.Id,
                                NombreCompleto = u.Personas.GetNombreCompleto(),
                                NroDocumento = u.Personas.NroDocumento,
                                NroTarjeta = u.Personas.NroTarjeta,
                                UserName = u.UserName,
                                DeviceId = u.DeviceId,
                                Token = u.UserIdNotification
                            })
                            .ToListAsync();
                    }

                    string[] deviceId = usuariosDelLote.ToArray().Select(u => u.DeviceId).ToArray();

                    // Creamos una tarea por cada usuario del lote (Reemplazar por el envio de Notificacion)
                    var tasks = usuariosDelLote.Select(usuario => ProcessarEnvioNotificacionUsuarioAsync(deviceId));
                    await Task.WhenAll(tasks);
                }
            }

            // --- 4. Guardado de Resultados ---
            using (var scope = _scopeFactory.CreateScope())
            {
                //var finalContext = scope.ServiceProvider.GetRequiredService<EstanciasContext>(); 

                // 1. Agrega los resúmenes que se generaron correctamente
                //await finalContext.AddRangeAsync(resumenesGenerados);

                //// 2. Crea el objeto de log principal
                //var logProcedimiento = new LogProcedimientos()
                //{
                //    Nombre = "Generación de Resumen Tarjeta",
                //    Fecha = DateTime.Now,
                //    StatusCode = "200",
                //    RegistrosCreados = resumenesGenerados.Count(),
                //    RegistrosConErrores = usuariosFallidos.Count(),
                //    Tiempo = stopwatch.ElapsedMilliseconds,
                //};

                //// 3. Crea la lista de logs de errores (sin asignar el Id manualmente)
                //var logErrores = usuariosFallidos.Select(x => new LogResumenesTarjetas()
                //{
                //    Fecha = DateTime.Now,
                //    Mensaje = x.Error,
                //    UsuarioId = x.UsuarioId
                //}).ToList();

                // 4. ¡LA MAGIA! Asigna la colección de errores a la propiedad de navegación del log principal
                //logProcedimiento.DetalleErrores = logErrores;

                // 5. Agrega el log principal al contexto. EF entenderá que también debe agregar los errores asociados.
                //await finalContext.AddAsync(logProcedimiento);

                // 6. Guarda todo en UNA SOLA transacción.
                //await finalContext.SaveChangesAsync();
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

    private async Task ProcessarEnvioNotificacionUsuarioAsync(string[] deviceId)
    {
        try
        {
            HttpStatusCode response = common.EnviaNotificationWonderPushId("Feliz Cumpleaños", "Feliz Cumpleaños", deviceId);           
        }
        catch (Exception ex)
        {
            //fallidos.Add((usuario.Id, ex.Message));
        }
        finally
        {
            //semaphore.Release();
        }
    }


}



