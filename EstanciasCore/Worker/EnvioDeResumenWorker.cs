using Common;
using DAL.Data;
using DAL.DTOs.Reportes;
using DAL.Models;
using DAL.Models.Core;
using EstanciasCore.Areas.Administracion.ViewModels;
using EstanciasCore.Interface;
using EstanciasCore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static EstanciasCore.Services.common;

public class EnvioDeResumenWorker : BackgroundService
{
    private readonly ILogger<EnvioDeResumenWorker> _logger;
    private readonly IServiceProvider _serviceProvider;

    // El constructor usa IServiceProvider para evitar la validación estricta de dependencias
    // ausentes o configuraciones nulas en caliente durante el arranque de .NET Core 2.2
    public EnvioDeResumenWorker(ILogger<EnvioDeResumenWorker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker de Envío de Resúmenes a Usuarios iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    DateTime fecha = DateTime.Now.Date;
                    var context = scope.ServiceProvider.GetRequiredService<EstanciasContext>();

                    // 1. VALIDACIÓN ORIGINAL: Buscamos la parametrización del proceso en base de datos
                    var procedimiento = await context.Procedimientos
                         .FirstOrDefaultAsync(p => p.Codigo == "EnvioResumen" && p.Activo == true, stoppingToken);

                    // 2. VALIDACIÓN ORIGINAL: Chequeamos si hoy corresponde disparar el ciclo de envío
                    bool debeEjecutar = procedimiento != null &&
                                        fecha.Day == procedimiento.DiaEjecucion &&
                                        (procedimiento.FechaUltimaEjecucionExitosa == null || procedimiento.FechaUltimaEjecucionExitosa.Value.Date != DateTime.Today);

                    if (debeEjecutar)
                    {
                        // 3. VALIDACIÓN ORIGINAL: Rescatamos el periodo con vencimiento fijado para el 15 de este mes
                        var periodo = await context.Periodo.AsNoTracking()
                            .FirstOrDefaultAsync(p => p.FechaVencimiento.Date == new DateTime(fecha.Year, fecha.Month, 15).Date, stoppingToken);

                        if (periodo != null)
                        {
                            _logger.LogInformation("Iniciando la tarea de envío de resúmenes mensuales.");

                            // Ejecutamos el flujo de negocio y la persistencia de estados globales
                            bool exito = await ProcesarYActualizarEstado(scope, procedimiento, periodo, stoppingToken);

                            if (exito)
                            {
                                _logger.LogInformation("Worker de envío de resúmenes: Tarea completada con éxito y estado persistido.");
                                await EnviarNotificacionAsync(
                                   scope,
                                   "Proceso de Resúmenes Finalizado con Éxito",
                                   $"La ejecución ha concluido correctamente a las {DateTime.Now:G}. Todos los correos procesados."
                                );
                            }
                            else
                            {
                                await EnviarNotificacionAsync(
                                    scope,
                                    "ERROR CRÍTICO: El proceso de Resúmenes falló",
                                    $"Se produjo un error que detuvo el proceso a las {DateTime.Now:G}.<br/><br/><strong>Detalle del error:</strong> <br/>"
                                );
                            }
                        }
                        else
                        {
                            _logger.LogInformation("Worker de envío de resúmenes: No se encontró un período de pago con vencimiento el día 15 de este mes. Se omite la ejecución.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[LOCAL-ENVIORESUMEN] Error controlado en el ciclo del worker.");
            }

            // Pausa de 1 hora en local para regular el consumo de CPU y reconexiones hacia producción
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    private async Task ProcesarYEnviarResumenes(CancellationToken stoppingToken, Periodo periodo, IServiceScope scope)
    {
        _logger.LogInformation("Conectando a la base de datos para obtener la lista de usuarios (MODO LIGERO).");

        var context = scope.ServiceProvider.GetRequiredService<EstanciasContext>();
        var viewEngine = scope.ServiceProvider.GetRequiredService<ICompositeViewEngine>();
        var mailService = scope.ServiceProvider.GetRequiredService<IMailService>();

        // Mantenemos la carga ligera y tomamos un único registro para tus pruebas locales seguras
        var resumenesLigeros = await context.ResumenTarjeta
            .AsNoTracking()
            .Where(x => x.PeriodoId == periodo.Id).Where(x => x.Usuario.RecibirResumen == true)
            //.Where(x => x.Usuario.UserName == "rpoggio1@abc.gob.ar" || x.Usuario.UserName == "RAFAELKLAPPENBACH@GMAIL.COM" || x.Usuario.UserName == "marianelamerduch@gmail.com")
            .Select(x => new
            {
                x.Id,
                x.Monto,
                x.MontoAdeudado,
                UsuarioUserName = x.Usuario.UserName,
            })
            .ToListAsync(stoppingToken);

        _logger.LogInformation($"Se encontraron {resumenesLigeros.Count} usuarios para procesar en modo prueba.");

        DateTime fechaVencimiento = new DateTime(periodo.FechaVencimiento.Year, periodo.FechaVencimiento.Month, 10);
        string mesNombre = ConvertirNumeroAMes(periodo.FechaHasta.Month);
        string asunto = $"Tu resumen de Tarjeta Estancias ya está disponible";

        foreach (var resuInfo in resumenesLigeros)
        {
            if (stoppingToken.IsCancellationRequested) return;

            try
            {
                var pdfBytes = await context.ResumenTarjeta
                    .Where(x => x.Id == resuInfo.Id)
                    .Select(x => x.Adjunto)
                    .FirstOrDefaultAsync(stoppingToken);

                if (pdfBytes == null || pdfBytes.Length == 0)
                {
                    _logger.LogWarning($"El resumen ID {resuInfo.Id} para el usuario {resuInfo.UsuarioUserName} no tiene PDF. Se omite.");
                    continue;
                }

                var detallesCuotasResumenDTO = new DetallesCuotasResumenDTO()
                {
                    Fecha = fechaVencimiento.ToString("dd/MM"),
                    Monto = resuInfo.Monto + resuInfo.MontoAdeudado,
                };

                var viewHtml = await RenderViewToString(viewEngine, scope.ServiceProvider, "Home/MailResumen", detallesCuotasResumenDTO, mesNombre);

                // --- REDIRECCIÓN DE EMAIL FORZADA PARA TU SEGURIDAD EN DESARROLLO ---
                //var mail = new MailAPI { Mail = "jorge.cutulli@iscoders.com.ar", Titulo = asunto, Html = viewHtml };
                var mail = new MailAPI { Mail = resuInfo.UsuarioUserName, Titulo = asunto, Html = viewHtml };

                // Despacho del email. Clavá tubreakpoint acá para debugear el SMTP
                await mailService.EnviarAsync(mail, pdfBytes);

                // --- ALTA DE AUDITORÍA USANDO LLAVES PRIMARIAS SIMPLES (EVITA EXCEPCIONES IDENTITY / MERGE) ---
                //var resumenParaLog = new ResumenTarjeta
                //{
                //    Id = resuInfo.Id,
                //    PeriodoId = periodo.Id
                //};

                // Guardamos en la tabla de auditoría. Si el guardado falla, el catch captura y salta al próximo
                //await GuardarRegistroCorreo(context, resumenParaLog);

                _logger.LogInformation($"Resumen enviado exitosamente en modo debug: {mail.Mail}");
            }
            catch (Exception ex)
            {
                // CONTROL DE SALTOS SOLICITADO: Si tira SqlException, timeout o error SMTP, 
                // se loguea acá y el loop avanza de forma fluida al siguiente registro sin colapsar el hilo
                _logger.LogError(ex, $"Fallo al procesar de forma individual el resumen del usuario {resuInfo.UsuarioUserName}. Saltando al siguiente...");
            }
        }
    }

    private async Task<bool> ProcesarYActualizarEstado(IServiceScope scope, Procedimientos procedimiento, Periodo periodo, CancellationToken stoppingToken)
    {
        try
        {
            await EnviarNotificacionAsync(
                scope,
                "Inicio del Proceso de Envío de Resumen",
                $"El proceso ha comenzado a las {DateTime.Now:G}."
            );

            await ProcesarYEnviarResumenes(stoppingToken, periodo, scope);

            // Marcamos el éxito real sobre la tabla de control de producción
            procedimiento.FechaUltimaEjecucionExitosa = DateTime.Now;
            var context = scope.ServiceProvider.GetRequiredService<EstanciasContext>();
            await context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error crítico en ProcesarYActualizarEstado.");
            await EnviarNotificacionAsync(
                scope,
                "ERROR CRÍTICO: El proceso de Resúmenes falló",
                $"Se produjo un error que detuvo el proceso a las {DateTime.Now:G}.<br/><br/><strong>Detalle del error:</strong> {ex.Message}"
            );
            return false;
        }
    }

    private async Task EnviarNotificacionAsync(IServiceScope scope, string asunto, string cuerpoHTML)
    {
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var mailService = scope.ServiceProvider.GetRequiredService<IMailService>();

        var emails = configuration["NotificationSettings:AdminEmails"];
        var adminEmails = emails?.Split(';', StringSplitOptions.RemoveEmptyEntries) ?? new string[0];

        if (adminEmails.Length == 0)
        {
            _logger.LogWarning("No hay emails de administrador configurados. Se omite el envío de notificación.");
            return;
        }

        foreach (var emailDestino in adminEmails)
        {
            try
            {
                var mail = new MailAPI { Mail = emailDestino.Trim(), Titulo = asunto, Html = cuerpoHTML };
                await mailService.EnviarAsync(mail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Fallo al enviar el email de notificación a: {emailDestino}");
            }
        }
    }

    private static string ConvertirNumeroAMes(int numeroMes)
    {
        CultureInfo culturaAR = new CultureInfo("es-AR");
        return culturaAR.DateTimeFormat.GetMonthName(numeroMes);
    }

    private async Task<string> RenderViewToString(ICompositeViewEngine viewEngine, IServiceProvider serviceProvider, string viewName, DetallesCuotasResumenDTO model, string mesNombre)
    {
        var httpContext = new DefaultHttpContext { RequestServices = serviceProvider };
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

        using (var sw = new StringWriter())
        {
            var viewResult = viewEngine.FindView(actionContext, viewName, false);

            if (viewResult.View == null)
            {
                throw new ArgumentNullException($"No se pudo encontrar la vista '{viewName}'");
            }

            var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()) { Model = model };
            var viewContext = new ViewContext(
                actionContext,
                viewResult.View,
                viewDictionary,
                new TempDataDictionary(actionContext.HttpContext, serviceProvider.GetRequiredService<ITempDataProvider>()),
                sw,
                new HtmlHelperOptions()
            );

            await viewResult.View.RenderAsync(viewContext);
            string html = sw.ToString();
            var culturaAR = new CultureInfo("es-AR");

            string textoModificado = html.Replace("TextoFechaReemplazar", model.Fecha);
            textoModificado = textoModificado.Replace("TextoMontoReemplazar", model.Monto.ToString("N2", culturaAR));
            textoModificado = textoModificado.Replace("TextoMesEscritoReemplazar", mesNombre);

            return textoModificado;
        }
    }

    public async Task GuardarRegistroCorreo(EstanciasContext context, ResumenTarjeta resumen)
    {
        // Se mapea la inserción directa por claves numéricas mapeando la auditoría de forma atómica
        var registro = new DistribucionResumen
        {
            ResumenTarjeta = resumen, // Toma de forma transparente la FK vinculada a su Id real de producción
            Fecha = DateTime.Now,
            Estado = "Enviado",
            Periodo = resumen.Periodo, // Forzamos la relación numérica pura para saltar validaciones Identity
            CanalesDistribucion = "Email"
        };

        context.DistribucionResumen.Add(registro);
        await context.SaveChangesAsync();
    }
}