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

    private DateTime? _ultimaEjecucionExitosa = null;

    public EnvioDeResumenWorker(ILogger<EnvioDeResumenWorker> logger, IServiceScopeFactory scopeFactory)
    {
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
                    //DateTime fecha = new DateTime(2025, 10, 01);
                    DateTime fechaActual = new DateTime(fecha.Year, fecha.Month, 15);
                    var context = scope.ServiceProvider.GetRequiredService<EstanciasContext>();
                    var procedimiento = await context.Procedimientos.AsNoTracking().FirstOrDefaultAsync(p => p.Codigo == "EnvioResumen", stoppingToken);

                    if (procedimiento != null && fecha.Day == procedimiento.DiaEjecucion && (_ultimaEjecucionExitosa == null || _ultimaEjecucionExitosa.Value.Date != DateTime.Today))
                    {
                        var periodo = await context.Periodo.AsNoTracking().FirstOrDefaultAsync(p => p.FechaVencimiento.Date == fechaActual.Date, stoppingToken);
                        _logger.LogInformation("Iniciando la tarea de envío de resúmenes mensuales.");
                        await ProcesarYEnviarResumenes(stoppingToken, periodo);
                        _ultimaEjecucionExitosa = DateTime.Today;
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

    private async Task ProcesarYEnviarResumenes(CancellationToken stoppingToken, Periodo periodo)
    {
        _logger.LogInformation("Conectando a la base de datos para obtener la lista de usuarios.");

        using (var scope = _scopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<EstanciasContext>();
            var viewEngine = scope.ServiceProvider.GetRequiredService<ICompositeViewEngine>();
            var serviceProvider = scope.ServiceProvider;

            var resumenes = await context.ResumenTarjeta.Include(x => x.Periodo).Include(x=>x.Usuario).ThenInclude(x => x.Personas).Where(x=> x.PeriodoId==periodo.Id && x.Usuario.Personas.NroDocumento=="18548722")
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

                    // **Genera el PDF en bytes (debes reemplazar esta lógica)**
                    byte[] pdfBytes = resu.Adjunto;

                    var detallesCuotasResumenDTO = new DetallesCuotasResumenDTO()
                    {
                        Fecha = fechaVencimiento.ToString("dd/MM/yyyy"),
                        Monto = resu.Monto,
                    };

                    var viewHtml = await RenderViewToString(viewEngine, serviceProvider, "Home/MailResumen", detallesCuotasResumenDTO, mesNombre);

                    // **Llama al método de envío con el PDF adjunto**
                    //await common.EnviarMailSendinBlueAdjunto(new MailAPI { Mail = resu.Usuario.UserName, Titulo = asunto, Html = viewHtml }, pdfBytes);
                    await common.EnviarMailSendinBlueAdjunto(new MailAPI { Mail = "jorgecutuli@gmail.com", Titulo = asunto, Html = viewHtml }, pdfBytes);

                    await GuardarRegistroCorreo(context, resu);
                    _logger.LogInformation($"Resumen enviado exitosamente a: {resu.Usuario.UserName}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Fallo al enviar el resumen al usuario {resu.Usuario.UserName}.");
                }
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
}