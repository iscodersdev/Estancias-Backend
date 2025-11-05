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

    public EnvioDeResumenWorker(ILogger<EnvioDeResumenWorker> logger, IServiceScopeFactory scopeFactory)
    {
        var dnisConfig = new List<string>() { "37217944", "29129264", "30463400", "28437058", "17984862", "38157735", "38321219", "36141667" };    
        _logger = logger;
        _scopeFactory = scopeFactory;
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
        _logger.LogInformation("Conectando a la base de datos para obtener la lista de usuarios.");

        // NOTA: Se utiliza el 'scope' pasado como parámetro desde ExecuteAsync.
        var context = scope.ServiceProvider.GetRequiredService<EstanciasContext>();
        var viewEngine = scope.ServiceProvider.GetRequiredService<ICompositeViewEngine>();
        var serviceProvider = scope.ServiceProvider;

        // Obtener la lista de resúmenes para enviar
        var resumenes = await context.ResumenTarjeta
                                     .Include(x => x.Periodo)
                                     .Include(x => x.Usuario)
                                         .ThenInclude(x => x.Personas)
                                     .Where(x => x.PeriodoId == periodo.Id)
                                     .ToListAsync(stoppingToken);

        _logger.LogInformation($"Se encontraron {resumenes.Count} usuarios para enviar resúmenes.");
        DateTime fechaVencimiento = new DateTime(periodo.FechaVencimiento.Year, periodo.FechaVencimiento.Month, 10);

        foreach (var resu in resumenes)
        {
            if (stoppingToken.IsCancellationRequested) return;

            try
            {
                string mesNombre = ConvertirNumeroAMes(periodo.FechaHasta.Month);
                string asunto = $"Tu resumen del mes de {mesNombre} ya está disponible";

                // **1. Genera el PDF en bytes (utilizando el Adjunto pre-generado)**
                byte[] pdfBytes = resu.Adjunto;

                // Verificación importante: si no hay adjunto, omitimos el envío
                if (pdfBytes == null || pdfBytes.Length == 0)
                {
                    _logger.LogWarning($"El resumen para el usuario {resu.Usuario.UserName} no tiene un adjunto (PDF) generado. Se omite el envío.");
                    continue;
                }

                var detallesCuotasResumenDTO = new DetallesCuotasResumenDTO()
                {
                    Fecha = fechaVencimiento.ToString("dd/MM/yyyy"),
                    // Nota: Usando decimales correctos para la suma.
                    Monto = resu.Monto + resu.MontoAdeudado,
                };

                // **2. Renderiza la vista del correo electrónico**
                var viewHtml = await RenderViewToString(viewEngine, serviceProvider, "Home/MailResumen", detallesCuotasResumenDTO, mesNombre);

                // **3. Envía el email con el PDF adjunto**
                //await common.EnviarMailSendinBlueAdjunto(new MailAPI { Mail = resu.Usuario.UserName, Titulo = asunto, Html = viewHtml }, pdfBytes);
                await common.EnviarMailSendinBlueAdjunto(new MailAPI { Mail = "jorge.cutulli@iscoders.com.ar", Titulo = asunto, Html = viewHtml }, pdfBytes);
                // Si la línea de prueba está activa, también se envía:
                // await common.EnviarMailSendinBlueAdjunto(new MailAPI { Mail = "jorgecutuli@gmail.com", Titulo = asunto, Html = viewHtml }, pdfBytes);

                // **4. Guarda el registro de que el correo se envió**
                await GuardarRegistroCorreo(context, resu);
                _logger.LogInformation($"Resumen enviado exitosamente a: {resu.Usuario.UserName}");
            }
            catch (Exception ex)
            {
                // Captura errores de envío individual, permitiendo que el bucle continúe para otros usuarios.
                // Si hay un error aquí, el estado de persistencia en la BD no se ve afectado si otros envíos tienen éxito.
                _logger.LogError(ex, $"Fallo al enviar el resumen al usuario {resu.Usuario.UserName}.");
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
            // Opcional: Podrías querer guardar el error en otro campo de la BD (Ej: FechaUltimaEjecucionFallida)
            return false;
        }
    }
}