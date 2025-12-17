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

public class WonderPushWorker : BackgroundService
{
    private readonly ILogger<ResumenMensualWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private readonly string[] _adminEmails;

    // Estado del worker
    private DateTime? _ultimaEjecucionMarcada = null;
    private int _intentosHoy = 0;
    private int _ultimoDiaDeIntentos = 0;

    public WonderPushWorker(ILogger<ResumenMensualWorker> logger, IServiceScopeFactory scopeFactory, IConfiguration configuration)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _configuration = configuration;

        var emails = _configuration["NotificationSettings:AdminEmails"];
        _adminEmails = emails?.Split(';', StringSplitOptions.RemoveEmptyEntries) ?? new string[0];
    }

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

                    // Consulta corregida para ser más directa
                    var procedimiento = await context.Procedimientos
                        .AsNoTracking()
                        .FirstOrDefaultAsync(p => p.Codigo == "Cumple" && p.Activo == true, stoppingToken);

                    if (procedimiento != null && procedimiento.Activo)
                    {
                        await EjecutarProcesoConNotificaciones(scope);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error fatal en el ciclo del worker.");
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
}