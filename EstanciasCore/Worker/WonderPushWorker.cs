using DAL.Data;
using DAL.DTOs.Reportes;
using DAL.DTOs.Servicios;
using DAL.Models;
using DAL.Models.Core;
using EstanciasCore.Areas.Administracion.ViewModels;
using EstanciasCore.Interface;
using EstanciasCore.Services;
using iText.Kernel.Counter.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

public class WonderPushWorker : BackgroundService
{
    // Dependencies and configuration
    private readonly ILogger<ResumenMensualWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private readonly string[] _adminEmails;

    // Worker state
    private DateTime? _ultimaEjecucionMarcada = null;
    private int _intentosHoy = 0;
    private int _ultimoDiaDeIntentos = 0;

    // Constructor
    public WonderPushWorker(ILogger<ResumenMensualWorker> logger, IServiceScopeFactory scopeFactory, IConfiguration configuration)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _configuration = configuration;

        var emails = _configuration["NotificationSettings:AdminEmails"];
        _adminEmails = emails?.Split(';', StringSplitOptions.RemoveEmptyEntries) ?? new string[0];
    }

    // Main execution loop
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker de WonderPush iniciado.");
        DateTime? ultimaRevisionAU = null;
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<EstanciasContext>();
                    var ahora = DateTime.Now;
                    var fechaHoy = ahora.Date;

                    // --- 1. PROCESO DE AUTOMÁTICAS ---

                    bool hayAutomaticasAhora = await context.Notificaciones
                     .AnyAsync(p => p.Activo
                               && p.TipoNotificacionesProcedimientos.Codigo == "AU"
                               && p.FechaEjecucion.Hour <= ahora.Hour
                               && p.FechaEjecucion.Day == fechaHoy.Day
                               && p.FechaUltimaEjecucion.Date != fechaHoy, stoppingToken);

                    if (hayAutomaticasAhora)
                    {
                        var procedimientoAutomaticos = await context.Notificaciones
                           .Include("NotificacionesPlantillas")
                           .Include("TipoNotificacionesProcedimientos")
                           .Where(p => p.TipoNotificacionesProcedimientos.Codigo == "AU")
                           .Where(p => p.Activo && p.FechaUltimaEjecucion.Date != fechaHoy)
                           .ToListAsync(stoppingToken);

                        foreach (var proc in procedimientoAutomaticos)
                        {
                            await EjecutarProcesoConNotificacionesAutomaticas(scope, proc);
                        }
                    }

                    bool CumplenAnios = await context.Usuarios
                    .AnyAsync(p => p.Personas.FechaNacimiento.Value.Day == DateTime.Now.Day && p.Personas.FechaNacimiento.Value.Month == DateTime.Now.Month, stoppingToken);

                    bool CumplenAniosSeEjecuto = !await context.Notificaciones
                    .AnyAsync(p => p.FechaUltimaEjecucion.Day == DateTime.Now.Day && p.FechaUltimaEjecucion.Month == DateTime.Now.Month, stoppingToken);

                    bool HoraDeEjecucion = await context.Notificaciones
                    .AnyAsync(p => p.FechaEjecucion.Hour == ahora.Hour && p.FechaEjecucion.Minute == ahora.Minute && p.Activo, stoppingToken);

                    if (CumplenAnios && CumplenAniosSeEjecuto && HoraDeEjecucion)
                    {
                        var procedimientoAutomaticos = await context.Notificaciones
                            .Include("NotificacionesPlantillas")
                            .Include("TipoNotificacionesProcedimientos")
                            .Where(p => p.TipoNotificacionesProcedimientos.Codigo == "AU")
                            .Where(p => p.Activo && p.FechaUltimaEjecucion.Date != fechaHoy)
                            .Where(p => p.Codigo == "C")
                            .ToListAsync(stoppingToken);

                        foreach (var proc in procedimientoAutomaticos)
                        {
                            await EjecutarProcesoConNotificacionesAutomaticas(scope, proc);
                        }
                    }

                    

                    // --- 2. PROCESO DE MANUALES (MA) ---
                    // Solo entramos si ya llegó la hora (FechaEjecucion <= ahora) y no se corrió hoy
                    bool hayManualesAhora = await context.Notificaciones
                        .AnyAsync(p => p.Activo
                                    && p.TipoNotificacionesProcedimientos.Codigo == "MA"
                                    && p.FechaEjecucion <= ahora
                                    && p.FechaEjecucion.Date == fechaHoy
                                    && p.FechaUltimaEjecucion.Date != fechaHoy, stoppingToken);

                    if (hayManualesAhora)
                    {
                        var procedimiento = await context.Notificaciones
                            .Include("NotificacionesPlantillas")
                            .Include("TipoNotificacionesProcedimientos")
                            .Include("ListaDistribucion")
                            .Where(p => p.TipoNotificacionesProcedimientos.Codigo == "MA")
                            .Where(p => p.Activo == true
                                     && p.FechaEjecucion.Hour == ahora.Hour
                                     && p.FechaEjecucion.Minute == ahora.Minute
                                     && p.FechaEjecucion.Date == fechaHoy
                                     && p.FechaUltimaEjecucion.Date != fechaHoy)
                            .Where(p => p.NotificacionesPlantillas != null && p.ListaDistribucion != null)
                            .AsNoTracking()
                            .ToListAsync(stoppingToken);

                        foreach (var proc in procedimiento)
                        {
                            await EjecutarProcesoConNotificaciones(scope, proc);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error fatal en el ciclo del worker.");
            }

            // Se ejecuta cada 1 min
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    // Process notifications based on their type
    private async Task EjecutarProcesoConNotificacionesAutomaticas(IServiceScope scope, Notificaciones notificaciones)
    {
        string resultadoFinal = "FALLIDO"; // Default state
        _intentosHoy++;

        try
        {
            if (string.Equals(notificaciones.Codigo, "C"))
            {
                await EjecutarSaludoCumpleanosAsync(scope, notificaciones);
                resultadoFinal = "OK";
            }
            else if (string.Equals(notificaciones.Codigo, "AS") && notificaciones.FechaEjecucion.Day == DateTime.Now.Day)
            {
                await EjecutarAvisosSaldos(scope, notificaciones);
                resultadoFinal = "OK";
            }
            else if (string.Equals(notificaciones.Codigo, "PV") && notificaciones.FechaEjecucion.Day == DateTime.Now.Day)
            {
                await EjecutarAvisosSaldos(scope, notificaciones);
                resultadoFinal = "OK";
            }
            else if (string.Equals(notificaciones.Codigo, "MI") && notificaciones.FechaEjecucion.Day == DateTime.Now.Day)
            {
                await EjecutarAvisosSaldos(scope, notificaciones);
                resultadoFinal = "OK";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error crítico durante la ejecución del servicio en el intento {_intentosHoy}.");
            resultadoFinal = $"FALLIDO CRÍTICAMENTE: {ex.Message}";
        }
        finally
        {
            _ultimaEjecucionMarcada = DateTime.Today;
        }
    }


    private async Task EjecutarProcesoConNotificaciones(IServiceScope scope, Notificaciones notificaciones)
    {
        string resultadoFinal = "FALLIDO"; // Default state
        _intentosHoy++;

        try
        {
            await EjecutarNotificacionAsync(scope, notificaciones);
            resultadoFinal = "OK";
            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error crítico durante la ejecución del servicio en el intento {_intentosHoy}.");
            resultadoFinal = $"FALLIDO CRÍTICAMENTE: {ex.Message}";
        }
        finally
        {
            _ultimaEjecucionMarcada = DateTime.Today;
        }
    }

    // Process balance notifications
    private async Task EjecutarAvisosSaldos(IServiceScope scope, Notificaciones notificaciones)
    {
        var fechaActual = DateTime.Now;

        try
        {
            var tamanoLote = _configuration.GetValue<int>("ProcesoResumen:TamanoLote", 100);
            var limiteConcurrencia = _configuration.GetValue<int>("ProcesoResumen:LimiteConcurrencia", 10);
            int totalUsuarios;

            var context = scope.ServiceProvider.GetRequiredService<EstanciasContext>();
            totalUsuarios = await context.Usuarios
                .Include(u => u.Personas)
                .CountAsync(u => u.Personas != null &&
                                    !string.IsNullOrEmpty(u.Personas.NroTarjeta) &&
                                    !string.IsNullOrEmpty(u.Personas.NroDocumento));

            var totalLotes = (int)Math.Ceiling((double)totalUsuarios / tamanoLote);
            var empresa = new DatosEstructura();

            using (var semaphore = new SemaphoreSlim(limiteConcurrencia))
            {
                for (int i = 0; i < totalLotes; i++)
                {
                    // Fetch users in batches
                    List<UsuarioParaProcesarDTO> usuariosDelLote;
                    var contextBatch = scope.ServiceProvider.GetRequiredService<EstanciasContext>();
                    empresa = contextBatch.DatosEstructura.FirstOrDefault();
                    usuariosDelLote = await contextBatch.Usuarios
                        .Include(u => u.Personas)
                        .Where(u => u.Personas != null &&
                                    !string.IsNullOrEmpty(u.Personas.NroTarjeta) &&
                                    !string.IsNullOrEmpty(u.Personas.NroDocumento)).Where(x=>x.Personas.NroDocumento=="39283631")
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

                    // Process each user in the batch
                    var tasks = usuariosDelLote.Select(usuario => EjecutarEnvioConSaldos(usuario, semaphore, empresa, notificaciones));
                    await Task.WhenAll(tasks);
                }
            }

            var contextUpdate = scope.ServiceProvider.GetRequiredService<EstanciasContext>();
            notificaciones.FechaUltimaEjecucion = DateTime.Now;
            contextUpdate.Update(notificaciones);
            contextUpdate.SaveChanges();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar el saludo de cumpleaños.");
        }
    }

    // Send balance notifications for a single user
    private async Task EjecutarEnvioConSaldos(UsuarioParaProcesarDTO usuario, SemaphoreSlim semaphore, DatosEstructura datosEstructura, Notificaciones notificaciones)
    {
        await semaphore.WaitAsync();

        try
        {
            var numeroTarjetaLimpio = usuario.NroTarjeta.Trim();

            if (!long.TryParse(numeroTarjetaLimpio, out long numeroTarjetaConvertido))
            {
                var errorMsg = $"El Nro. de Tarjeta '{numeroTarjetaLimpio}' no tiene un formato numérico válido.";
                return;
            }

            using (var scope = _scopeFactory.CreateScope())
            {
                DateTime fechaMesActualCuotas = DateTime.Now;
                fechaMesActualCuotas = new DateTime(fechaMesActualCuotas.Year, fechaMesActualCuotas.Month, 01);

                int diasEnMes = DateTime.DaysInMonth(fechaMesActualCuotas.Year, fechaMesActualCuotas.Month);

                DateTime fechaPunitorios = fechaMesActualCuotas.Day > 15
                    ? new DateTime(fechaMesActualCuotas.Year, fechaMesActualCuotas.Month, diasEnMes)
                    : new DateTime(fechaMesActualCuotas.Year, fechaMesActualCuotas.Month, 15);

                DateTime fechaActualCuotas = new DateTime(fechaMesActualCuotas.Year, fechaMesActualCuotas.Month, diasEnMes);
                var scopedDatosServices = scope.ServiceProvider.GetRequiredService<IDatosTarjetaService>();
                var scopedContext = scope.ServiceProvider.GetRequiredService<EstanciasContext>();
                var datosMovimientos = await scopedDatosServices.ConsultarMovimientos(datosEstructura.UsernameWS, datosEstructura.PasswordWS, usuario.NroDocumento, numeroTarjetaConvertido, 100, 1);

                if (datosMovimientos.Detalle.Resultado == "EXITO")
                {
                    var datosResumen = await scopedDatosServices.CuotasDetallesResumen(datosMovimientos, fechaActualCuotas);
                    var datosResumenConPunitorios = scopedDatosServices.CalcularPunitoriosResumen(datosResumen).Result;
                    var periodo = new Periodo
                    {
                        FechaVencimiento = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 10),
                        FechaDesde = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 10),
                        FechaHasta = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 10)
                    };

                    var datosParaResumenDTO = (TempalteResumenDTO)await scopedDatosServices.PrepararDatosResumen(datosMovimientos, datosResumenConPunitorios, periodo, usuario);
                    if (datosParaResumenDTO == null) return;

                    if (datosParaResumenDTO.SaldoActual > 0)
                    {
                        NotificacionViewModelDTO notificacionesDTO = new NotificacionViewModelDTO()
                        {
                            Titulo = notificaciones.NotificacionesPlantillas.Titulo,
                            Mensaje = notificaciones.NotificacionesPlantillas.Mensaje,
                            ImagenUrl = notificaciones.NotificacionesPlantillas.ImagenUrl,
                            ImagenIcon = notificaciones.NotificacionesPlantillas.Icon,
                            DeepLink = notificaciones.NotificacionesPlantillas.DeepLink
                        };

                        var instalationId = new List<string> { usuario.UserName };
                        var wonderPushService = scope.ServiceProvider.GetRequiredService<IWonderPushService>();
                        var respuestawp = await wonderPushService.EnviarNotificacionAIds(notificacionesDTO, instalationId);
                    }
                }
            }
        }
        catch
        {
            // Handle exceptions silently
        }
        finally
        {
            semaphore.Release();
        }
    }

    // Send birthday greetings
    private async Task EjecutarSaludoCumpleanosAsync(IServiceScope scope, Notificaciones notificaciones)
    {
        var fechaActual = DateTime.Now;

        try
        {
            var context = scope.ServiceProvider.GetRequiredService<EstanciasContext>();
            var fechaInicio = fechaActual.Date;
            var dia = notificaciones.FechaEjecucion.Day;
            var fechaFin = fechaInicio.AddDays(-dia);

            var personasEnvio = context.Usuarios
                .Where(x => x.Personas != null &&
                            x.Personas.FechaNacimiento.Value.Month == fechaFin.Month &&
                            x.Personas.FechaNacimiento.Value.Day == fechaFin.Day)
                .ToList();

            var instalationId = personasEnvio
                .Select(u => u.DeviceId)
                .Where(id => !string.IsNullOrEmpty(id))
                .ToList();

            NotificacionViewModelDTO notificacionesDTO = new NotificacionViewModelDTO()
            {
                Titulo = notificaciones.NotificacionesPlantillas.Titulo,
                Mensaje = notificaciones.NotificacionesPlantillas.Mensaje,
                ImagenUrl = notificaciones.NotificacionesPlantillas.ImagenUrl,
                ImagenIcon = notificaciones.NotificacionesPlantillas.Icon,
                DeepLink = notificaciones.NotificacionesPlantillas.DeepLink
            };

            var wonderPushService = scope.ServiceProvider.GetRequiredService<IWonderPushService>();
            var respuestawp = await wonderPushService.EnviarNotificacionAIds(notificacionesDTO, instalationId);
            notificaciones.FechaUltimaEjecucion = DateTime.Now;
            context.Update(notificaciones);
            context.SaveChanges();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar el saludo de cumpleaños.");
        }
    }

    // Send birthday greetings
    private async Task EjecutarNotificacionAsync(IServiceScope scope, Notificaciones notificaciones)
    {
        var fechaActual = DateTime.Now;

        try
        {
            var context = scope.ServiceProvider.GetRequiredService<EstanciasContext>();
            List<Usuario> personasEnvio = new List<Usuario>();
            if (notificaciones.ListaDistribucion.Nombre=="Todos")
            {
                personasEnvio = context.Usuarios
                    .Where(x =>x.DeviceId != null)
                    .ToList();
            }
            else
            {
                personasEnvio = context.DistribucionDestinatarios
                    .Where(x => x.ListaDistribucion.Id == notificaciones.ListaDistribucion.Id && x.Destinatario.DeviceId != null).Select(d => d.Destinatario)
                    .ToList();

            }

            var instalationId = personasEnvio
                .Select(u => u.DeviceId)
                .Where(id => !string.IsNullOrEmpty(id))
                .ToList();

            NotificacionViewModelDTO notificacionesDTO = new NotificacionViewModelDTO()
            {
                Titulo = notificaciones.NotificacionesPlantillas.Titulo,
                Mensaje = notificaciones.NotificacionesPlantillas.Mensaje,
                ImagenUrl = notificaciones.NotificacionesPlantillas.ImagenUrl,
                ImagenIcon = notificaciones.NotificacionesPlantillas.Icon,
                DeepLink = notificaciones.NotificacionesPlantillas.DeepLink,
                
            };

            var wonderPushService = scope.ServiceProvider.GetRequiredService<IWonderPushService>();
            var respuestawp = await wonderPushService.EnviarNotificacionAIds(notificacionesDTO, instalationId);
            notificaciones.FechaUltimaEjecucion = DateTime.Now;
            context.Update(notificaciones);
            context.SaveChanges();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar el saludo de cumpleaños.");
        }
    }

    /// <summary>
    /// Devuelve true si las fechas son iguales (sin comparar hora y minutos)
    /// </summary>
    /// <param name="fecha1"></param>
    /// <param name="fecha2"></param>
    /// <returns></returns>
    private bool ValidarFecha(DateTime fecha1, DateTime fecha2)
    {
        bool bandera = true;

        if(fecha1.Day != fecha2.Day)
        {
            bandera = false;
        }

        if(fecha1.Month != fecha2.Month)
        {
            bandera = false;
        }

        if(fecha1.Year != fecha2.Year)
        {
            bandera = false;
        }
        return bandera;
    }
}