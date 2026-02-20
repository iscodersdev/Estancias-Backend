using Common;
using DAL.Data;
using DAL.DTOs.Reportes;
using DAL.Models;
using DAL.Models.Core;
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
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private readonly string[] _adminEmails;

    public EnvioDeResumenWorker(ILogger<EnvioDeResumenWorker> logger, IServiceScopeFactory scopeFactory, IConfiguration configuration)
    {
        var dnisConfig = new List<string>() { "37217944", "29129264", "30463400", "28437058", "17984862", "38157735", "38321219", "36141667" };    
        _logger = logger;
        _scopeFactory = scopeFactory;
        _configuration = configuration;
        var emails = _configuration["NotificationSettings:AdminEmails"];
        _adminEmails = emails?.Split(';', StringSplitOptions.RemoveEmptyEntries) ?? new string[0];
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker de Envío de Resúmenes a Usuarios iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    DateTime fecha = DateTime.Now.Date;
                    var context = scope.ServiceProvider.GetRequiredService<EstanciasContext>();

                    // NOTA: Quité AsNoTracking para poder modificar y guardar el procedimiento si es exitoso
                    var procedimiento = await context.Procedimientos
                                                     .FirstOrDefaultAsync(p => p.Codigo == "EnvioResumen" && p.Activo == true, stoppingToken);

                    // Reemplazamos la variable local por la fecha de la base de datos
                    bool debeEjecutar = procedimiento != null &&
                                        fecha.Day == procedimiento.DiaEjecucion &&
                                        (procedimiento.FechaUltimaEjecucionExitosa == null || procedimiento.FechaUltimaEjecucionExitosa.Value.Date != DateTime.Today); // <--- CAMBIO CLAVE

                    if (debeEjecutar)
                    {
                        var periodo = await context.Periodo.AsNoTracking().FirstOrDefaultAsync(p => p.FechaVencimiento.Date == new DateTime(fecha.Year, fecha.Month, 15).Date, stoppingToken);

                        if (periodo != null)
                        {
                            _logger.LogInformation("Iniciando la tarea de envío de resúmenes mensuales.");

                            // Llamamos a un nuevo método que envuelve la ejecución y la persistencia del estado
                            bool exito = await ProcesarYActualizarEstado(scope, procedimiento, periodo, stoppingToken);

                            if (exito)
                            {
                                _logger.LogInformation("Worker de envío de resúmenes: Tarea completada con éxito y estado persistido.");
                                await EnviarNotificacionAsync(
                                   "Proceso de Resúmenes Finalizado con Éxito",
                                   $"La ejecución ha concluido correctamente a las {DateTime.Now:G}. Todos los correos procesados."
                                );
                            }
                            else
                            {
                                await EnviarNotificacionAsync(
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
                _logger.LogError(ex, "Ocurrió un error fatal en el ciclo del worker.");
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    private async Task ProcesarYEnviarResumenes(CancellationToken stoppingToken, Periodo periodo, IServiceScope scope)
    {
        _logger.LogInformation("Conectando a la base de datos para obtener la lista de usuarios (MODO LIGERO).");

        var context = scope.ServiceProvider.GetRequiredService<EstanciasContext>();
        var viewEngine = scope.ServiceProvider.GetRequiredService<ICompositeViewEngine>();
        var serviceProvider = scope.ServiceProvider;

        // 1. OPTIMIZACIÓN: Usar AsNoTracking y Select para NO traer el campo 'Adjunto' (BLOB) todavía.
        // Esto hace que la consulta baje de segundos/minutos a milisegundos.
        var resumenesLigeros = await context.ResumenTarjeta
            .AsNoTracking() // Importante: No necesitamos rastrear cambios en esta lista
            .Where(x => x.PeriodoId == periodo.Id).Where(x=>x.Usuario.RecibirResumen==true)
            .Select(x => new
            {
                x.Id,
                x.Monto,
                x.MontoAdeudado,
                UsuarioUserName = x.Usuario.UserName,
                // Agrega aquí otros campos de Usuario/Persona si los usas en el log o validaciones
                // x.Usuario.Personas... 
            })
            .ToListAsync(stoppingToken);

        _logger.LogInformation($"Se encontraron {resumenesLigeros.Count} usuarios para procesar.");

        DateTime fechaVencimiento = new DateTime(periodo.FechaVencimiento.Year, periodo.FechaVencimiento.Month, 10);
        string mesNombre = ConvertirNumeroAMes(periodo.FechaHasta.Month);
        string asunto = $"Tu resumen de Tarjeta Estancias ya está disponible";

        foreach (var resuInfo in resumenesLigeros)
        {
            if (stoppingToken.IsCancellationRequested) return;

            try
            {
                // 2. OPTIMIZACIÓN: Obtener el PDF bajo demanda (Lazy Loading manual)
                // Solo traemos el PDF de ESTE usuario específico.
                var pdfBytes = await context.ResumenTarjeta
                    .Where(x => x.Id == resuInfo.Id)
                    .Select(x => x.Adjunto)
                    .FirstOrDefaultAsync(stoppingToken);

                // Verificación importante
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

                // Renderiza la vista
                var viewHtml = await RenderViewToString(viewEngine, serviceProvider, "Home/MailResumen", detallesCuotasResumenDTO, mesNombre);

                // Envía el email

                await common.EnviarMailSendinBlueAdjunto(new MailAPI { Mail = resuInfo.UsuarioUserName.Trim(), Titulo = asunto, Html = viewHtml }, pdfBytes);
                //await common.EnviarMailSendinBlueAdjunto(new MailAPI { Mail = "jorgecutuli@gmail.com", Titulo = asunto, Html = viewHtml }, pdfBytes);

                // 3. Guarda el registro
                // Nota: Como 'resuInfo' es un objeto anónimo, necesitamos instanciar la entidad o usar el ID para guardar el log.
                // Asumo que tu método GuardarRegistroCorreo espera la entidad completa. 
                // Si puedes cambiarlo para que acepte solo el ID sería mejor, si no, puedes hacer un "Fake" attach o buscarlo.

                // Opción A: Modificar GuardarRegistroCorreo para recibir solo IDs.
                // Opción B (Rápida aquí): Crear un objeto dummy solo con el ID si tu logica lo permite, 
                // o si necesitas la entidad completa para el log, recupérala sin el adjunto.
                var resumenParaLog = new ResumenTarjeta { Id = resuInfo.Id, Usuario = new Usuario { UserName = resuInfo.UsuarioUserName } };
                await GuardarRegistroCorreo(context, resumenParaLog);

                _logger.LogInformation($"Resumen enviado exitosamente a: {resuInfo.UsuarioUserName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Fallo al enviar el resumen al usuario {resuInfo.UsuarioUserName}.");
            }
        }
    }
    //private async Task ProcesarYEnviarResumenes(CancellationToken stoppingToken, Periodo periodo, IServiceScope scope)
    //{
    //    _logger.LogInformation("Conectando a la base de datos para obtener la lista de usuarios.");

    //    // NOTA: Se utiliza el 'scope' pasado como parámetro desde ExecuteAsync.
    //    var context = scope.ServiceProvider.GetRequiredService<EstanciasContext>();
    //    var viewEngine = scope.ServiceProvider.GetRequiredService<ICompositeViewEngine>();
    //    var serviceProvider = scope.ServiceProvider;

    //    // Obtener la lista de resúmenes para enviar
    //    var resumenes = await context.ResumenTarjeta
    //                                 .Include(x => x.Periodo)
    //                                 .Include(x => x.Usuario)
    //                                 .Where(x => x.PeriodoId == periodo.Id)
    //                                 .ToListAsync(stoppingToken);

    //    _logger.LogInformation($"Se encontraron {resumenes.Count} usuarios para enviar resúmenes.");
    //    DateTime fechaVencimiento = new DateTime(periodo.FechaVencimiento.Year, periodo.FechaVencimiento.Month, 10);

    //    foreach (var resu in resumenes)
    //    {
    //        if (stoppingToken.IsCancellationRequested) return;

    //        try
    //        {
    //            string mesNombre = ConvertirNumeroAMes(periodo.FechaHasta.Month);
    //            string asunto = $"Tu resumen de Tarjeta Estancias ya está disponible";

    //            // **1. Genera el PDF en bytes (utilizando el Adjunto pre-generado)**
    //            byte[] pdfBytes = resu.Adjunto;

    //            // Verificación importante: si no hay adjunto, omitimos el envío
    //            if (pdfBytes == null || pdfBytes.Length == 0)
    //            {
    //                _logger.LogWarning($"El resumen para el usuario {resu.Usuario.UserName} no tiene un adjunto (PDF) generado. Se omite el envío.");
    //                continue;
    //            }

    //            var detallesCuotasResumenDTO = new DetallesCuotasResumenDTO()
    //            {
    //                Fecha = fechaVencimiento.ToString("dd/MM"),
    //                // Nota: Usando decimales correctos para la suma.
    //                Monto = resu.Monto + resu.MontoAdeudado,
    //            };

    //            // **2. Renderiza la vista del correo electrónico**
    //            var viewHtml = await RenderViewToString(viewEngine, serviceProvider, "Home/MailResumen", detallesCuotasResumenDTO, mesNombre);

    //            // **3. Envía el email con el PDF adjunto**
    //            //await common.EnviarMailSendinBlueAdjunto(new MailAPI { Mail = resu.Usuario.UserName, Titulo = asunto, Html = viewHtml }, pdfBytes);
    //            //await common.EnviarMailSendinBlueAdjunto(new MailAPI { Mail = "jorge.cutulli@iscoders.com.ar", Titulo = asunto, Html = viewHtml }, pdfBytes);
    //            // Si la línea de prueba está activa, también se envía:
    //            await common.EnviarMailSendinBlueAdjunto(new MailAPI { Mail = "jorgecutuli@gmail.com", Titulo = asunto, Html = viewHtml }, pdfBytes);

    //            // **4. Guarda el registro de que el correo se envió**
    //            await GuardarRegistroCorreo(context, resu);
    //            _logger.LogInformation($"Resumen enviado exitosamente a: {resu.Usuario.UserName}");
    //        }
    //        catch (Exception ex)
    //        {
    //            // Captura errores de envío individual, permitiendo que el bucle continúe para otros usuarios.
    //            // Si hay un error aquí, el estado de persistencia en la BD no se ve afectado si otros envíos tienen éxito.
    //            _logger.LogError(ex, $"Fallo al enviar el resumen al usuario {resu.Usuario.UserName}.");
    //        }
    //    }
    //}

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

            var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model
            };

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
        var registro = new DistribucionResumen
        {
            ResumenTarjeta = resumen,
            Fecha = DateTime.Now,
            Estado = "Enviado",
            Usuario = resumen.Usuario,
            Periodo = resumen.Periodo,
            CanalesDistribucion = "Email"
        };

        context.DistribucionResumen.Add(registro);
        await context.SaveChangesAsync();
    }

    private byte[] GenerarPdfDelResumen(ResumenTarjeta resumen)
    {
        // **IMPORTANTE: Esta es una función de ejemplo. Debes reemplazarla con tu lógica real.**
        string contenido = $"Este es un PDF de prueba para el resumen del usuario {resumen.Usuario.UserName} con un monto de {resumen.Monto}.";
        return System.Text.Encoding.UTF8.GetBytes(contenido);
    }

    private async Task<bool> ProcesarYActualizarEstado(IServiceScope scope, Procedimientos procedimiento, Periodo periodo, CancellationToken stoppingToken)
    {
        try
        {
            // 1. NOTIFICACIÓN DE INICIO
            await EnviarNotificacionAsync(
                "Inicio del Proceso de Envío de Resumen",
                $"El proceso ha comenzado a las {DateTime.Now:G}."
            );

            // 1. Ejecutar la lógica principal de envío
            await ProcesarYEnviarResumenes(stoppingToken, periodo, scope); // Se pasa el scope para reutilizarlo en la actualización

            // 2. Si llegamos aquí, asumimos que el envío general fue exitoso (o al menos lo suficientemente bueno para no repetir)

            // 3. Persistir el estado de éxito en la BD
            procedimiento.FechaUltimaEjecucionExitosa = DateTime.Now;
            var context = scope.ServiceProvider.GetRequiredService<EstanciasContext>();
            await context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ProcesarYActualizarEstado. El estado de ejecución NO se actualizará.");

            await EnviarNotificacionAsync(
                "ERROR CRÍTICO: El proceso de Resúmenes falló",
                $"Se produjo un error que detuvo el proceso a las {DateTime.Now:G}.<br/><br/><strong>Detalle del error:</strong> {ex.Message} <br/> {ex.StackTrace}"
            );
            return false;
        }
    }

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
}