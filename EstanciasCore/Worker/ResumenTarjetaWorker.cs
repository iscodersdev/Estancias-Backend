using DAL.Data;
using EstanciasCore.Interface;
using EstanciasCore.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class ResumenMensualWorker : BackgroundService
{
    private readonly ILogger<ResumenMensualWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private readonly string[] _adminEmails;

    // Estado del worker
    private DateTime? _ultimaEjecucionMarcada = null;
    private int _intentosHoy = 0;
    private int _ultimoDiaDeIntentos = 0;

    public ResumenMensualWorker(ILogger<ResumenMensualWorker> logger, IServiceScopeFactory scopeFactory, IConfiguration configuration)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _configuration = configuration;

        var emails = _configuration["NotificationSettings:AdminEmails"];
        _adminEmails = emails?.Split(';', StringSplitOptions.RemoveEmptyEntries) ?? new string[0];
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker de Resúmenes Mensuales iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<EstanciasContext>();

                    // Consulta corregida para ser más directa
                    var procedimiento = await context.Procedimientos
                        .AsNoTracking()
                        .FirstOrDefaultAsync(p => p.Codigo == "GenerarResumen", stoppingToken);

                    if (procedimiento != null && procedimiento.Activo && DebeEjecutarHoy(procedimiento.DiaEjecucion))
                    {
                        await EjecutarProcesoConNotificaciones(scope);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error fatal en el ciclo del worker.");
                // Opcional: Enviar un email de fallo crítico si el propio worker falla
                await EnviarNotificacionAsync("Error Crítico en Worker", $"El worker de resúmenes ha fallado de forma inesperada. Error: {ex.Message}");
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    private async Task EjecutarProcesoConNotificaciones(IServiceScope scope)
    {
        string resultadoFinal = "FALLIDO"; // Estado por defecto
        _intentosHoy++;

        try
        {
            // 1. ENVIAR EMAIL DE INICIO
            await EnviarNotificacionAsync(
                "Inicio del Proceso de Resúmenes",
                $"El proceso ha comenzado a las {DateTime.Now:G}. (Intento {_intentosHoy})"
            );

            // 2. EJECUTAR LÓGICA DE NEGOCIO
            var resumenService = scope.ServiceProvider.GetRequiredService<IResumenTarjetaService>();
            bool exito = await resumenService.GenerarResumenTarjetas();
            resultadoFinal = exito ? "ÉXITO" : "FINALIZADO CON ADVERTENCIAS";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error crítico durante la ejecución del servicio en el intento {_intentosHoy}.");
            resultadoFinal = $"FALLIDO CRÍTICAMENTE: {ex.Message}";
        }
        finally
        {
            // 3. ENVIAR EMAIL DE FINALIZACIÓN (SIEMPRE SE EJECUTA)
            await EnviarNotificacionAsync(
                $"Proceso de Resúmenes Finalizado con Estado: {resultadoFinal}",
                $"La ejecución ha concluido a las {DateTime.Now:G}. El resultado fue: {resultadoFinal}."
            );

            // 4. MARCAR COMO EJECUTADO PARA NO REPETIR HOY
            _ultimaEjecucionMarcada = DateTime.Today;
        }
    }

    // --- MÉTODO PRIVADO QUE USA TU LÓGICA DE EMAIL ---
    private Task EnviarNotificacionAsync(string asunto, string cuerpoHTML)
    {
        if (_adminEmails.Length == 0)
        {
            _logger.LogWarning("No hay emails de administrador configurados. Se omite el envío de notificación.");
            return Task.CompletedTask;
        }

        _logger.LogInformation($"Preparando email: '{asunto}'");
        foreach (var emailDestino in _adminEmails)
        {
            try
            {
                // --- TU LÍNEA DE CÓDIGO INTEGRADA AQUÍ ---
                common.EnviarMail(emailDestino.Trim(), asunto, cuerpoHTML, "");
                _logger.LogInformation($"Email enviado exitosamente a: {emailDestino}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Fallo al enviar el email de notificación a: {emailDestino}");
            }
        }
        return Task.CompletedTask;
    }

    private bool DebeEjecutarHoy(int diaDeEjecucionDesdeBD)
    {
        var ahora = DateTime.Now;

        if (ahora.Day != _ultimoDiaDeIntentos)
        {
            _intentosHoy = 0;
            _ultimaEjecucionMarcada = null;
            _ultimoDiaDeIntentos = ahora.Day;
        }

        bool esDiaDeEjecucion = ahora.Day == diaDeEjecucionDesdeBD;
        bool yaSeEjecuto = _ultimaEjecucionMarcada.HasValue && _ultimaEjecucionMarcada.Value.Date == ahora.Date;
        bool limiteDeIntentosSuperado = _intentosHoy >= 3;

        return esDiaDeEjecucion && !yaSeEjecuto && !limiteDeIntentosSuperado;
    }
}