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
    private readonly IWonderPushService _wonderPushService;
    private readonly IDatosTarjetaService _datosTarjetaService;

    // Worker state
    private DateTime? _ultimaEjecucionMarcada = null;
    private int _intentosHoy = 0;
    private int _ultimoDiaDeIntentos = 0;

    // Constructor
    public WonderPushWorker(ILogger<ResumenMensualWorker> logger, IServiceScopeFactory scopeFactory, IConfiguration configuration, IWonderPushService wonderPushService, IDatosTarjetaService datosTarjetaService)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _configuration = configuration;

        var emails = _configuration["NotificationSettings:AdminEmails"];
        _adminEmails = emails?.Split(';', StringSplitOptions.RemoveEmptyEntries) ?? new string[0];
        _wonderPushService = wonderPushService;
    }

    // Main execution loop
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker de WonderPush iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<EstanciasContext>();

                    // Fetch active notifications to process
                    var procedimiento = await context.Notificaciones.Include("NotificacionesPlantillas")
                        .Where(p => p.Activo == true && p.FechaUltimaEjecucion.Date != DateTime.Now.Date).AsNoTracking()
                        .ToListAsync(stoppingToken);

                    // Process each notification
                    foreach (var proc in procedimiento)
                    {
                        await EjecutarProcesoConNotificaciones(scope, proc);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error fatal en el ciclo del worker.");
            }

            // Wait before next execution cycle
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    // Process notifications based on their type
    private async Task EjecutarProcesoConNotificaciones(IServiceScope scope, Notificaciones notificaciones)
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

    // Process balance notifications
    private async Task EjecutarAvisosSaldos(IServiceScope scope, Notificaciones notificaciones)
    {
        var fechaActual = DateTime.Now;

        try
        {
            var tamanoLote = _configuration.GetValue<int>("ProcesoResumen:TamanoLote", 100);
            var limiteConcurrencia = _configuration.GetValue<int>("ProcesoResumen:LimiteConcurrencia", 10);
            int totalUsuarios;

            using (var context = scope.ServiceProvider.GetRequiredService<EstanciasContext>())
            {
                totalUsuarios = await context.Usuarios
                    .Include(u => u.Personas)
                    .CountAsync(u => u.Personas != null &&
                                     !string.IsNullOrEmpty(u.Personas.NroTarjeta) &&
                                     !string.IsNullOrEmpty(u.Personas.NroDocumento));
            }

            var totalLotes = (int)Math.Ceiling((double)totalUsuarios / tamanoLote);
            var empresa = new DatosEstructura();

            using (var semaphore = new SemaphoreSlim(limiteConcurrencia))
            {
                for (int i = 0; i < totalLotes; i++)
                {
                    // Fetch users in batches
                    List<UsuarioParaProcesarDTO> usuariosDelLote;
                    using (var context = scope.ServiceProvider.GetRequiredService<EstanciasContext>())
                    {
                        empresa = context.DatosEstructura.FirstOrDefault();
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

                    // Process each user in the batch
                    var tasks = usuariosDelLote.Select(usuario => EjecutarEnvioConSaldos(usuario, semaphore, empresa, notificaciones));
                    await Task.WhenAll(tasks);
                }
            }

            using (var context = scope.ServiceProvider.GetRequiredService<EstanciasContext>())
            {
                notificaciones.FechaUltimaEjecucion = DateTime.Now;
                context.Update(notificaciones);
                context.SaveChanges();
            }
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
                            DeepLink = notificaciones.NotificacionesPlantillas.DeepLink
                        };

                        var instalationId = new List<string> { usuario.UserName };
                        var respuestawp = await _wonderPushService.EnviarNotificacionAIds(notificacionesDTO, instalationId);
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
            using (var context = scope.ServiceProvider.GetRequiredService<EstanciasContext>())
            {
                var fechaInicio = fechaActual.Date;
                var fechaFin = fechaInicio.AddDays(7);

                var personasEnvio = context.Usuarios
                    .Where(x => x.Personas != null &&
                                x.Personas.FechaNacimiento.Value.Month == fechaActual.Month &&
                                x.Personas.FechaNacimiento.Value.Day >= fechaInicio.Day &&
                                x.Personas.FechaNacimiento.Value.Day <= fechaFin.Day)
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
                    DeepLink = notificaciones.NotificacionesPlantillas.DeepLink
                };

                var respuestawp = await _wonderPushService.EnviarNotificacionAIds(notificacionesDTO, instalationId);
                notificaciones.FechaUltimaEjecucion = DateTime.Now;
                context.Update(notificaciones);
                context.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar el saludo de cumpleaños.");
        }
    }
}