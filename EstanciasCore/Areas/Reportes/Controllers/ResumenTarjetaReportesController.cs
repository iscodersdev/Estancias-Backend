using Commons.Models;
using DAL.Data;
using DAL.DTOs.Reportes;
using DAL.Models;
using EstanciasCore.Controllers; // Tu controlador base
using EstanciasCore.Interface; // Para IDatosTarjetaService
using EstanciasCore.Services;
using iText.Html2pdf;
using iText.Kernel.Pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
public class DatosParaResumenDTO
{
    public Usuario Usuario { get; set; }
    public Periodo Periodo { get; set; }
    public List<MovimientoTarjeta> Movimientos { get; set; }
    public decimal SaldoAnterior { get; set; }
    public decimal Pagos { get; set; }
    public decimal Intereses { get; set; }
    public decimal Impuestos { get; set; }
    public decimal SaldoActual { get; set; }
    public decimal PagoMinimo { get; set; }
    public string Domicilio { get; set; }
}


[Area("Reportes")]
public class ResumenTarjetaReportesController : EstanciasCoreController
{
    private readonly IDatosTarjetaService _datosServices;
    private readonly ICompositeViewEngine _viewEngine;

    public ResumenTarjetaReportesController(EstanciasContext context, IDatosTarjetaService datosServices, ICompositeViewEngine viewEngine) : base(context)
    {
        breadcumb.Add(new Message() { DisplayName = "Reportes" });
        _datosServices = datosServices;
        _viewEngine = viewEngine;
    }

    public IActionResult Index()
    {
        breadcumb.Add(new Message() { DisplayName = "Resumen de Tarjeta" });
        ViewBag.Breadcrumb = breadcumb;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> _ListadoResumenes(string nroTarjetaFiltro, string nroDocumentoFiltro)
    {
        try
        {
            if (string.IsNullOrEmpty(nroTarjetaFiltro) && string.IsNullOrEmpty(nroDocumentoFiltro))
                return PartialView(new List<ResumenTarjetaDTO>());

            // Búsqueda de usuario optimizada y asíncrona
            IQueryable<Usuario> query = _context.Usuarios;
            if (!string.IsNullOrEmpty(nroTarjetaFiltro))
                query = query.Where(x => x.Personas.NroTarjeta == nroTarjetaFiltro);
            if (!string.IsNullOrEmpty(nroDocumentoFiltro))
                query = query.Where(x => x.Personas.NroDocumento == nroDocumentoFiltro);

            var usuario = await query.FirstOrDefaultAsync();

            if (usuario == null)
            {
                ViewBag.ErrorMessage = "No se encontró ningún usuario con los datos proporcionados.";
                return PartialView(new List<ResumenTarjetaDTO>());
            }

            // Sincroniza los movimientos desde el servicio externo
            //await ActualizarMovimientosAsync(usuario);

            DateTime fechaActual = DateTime.Now;

            List<MovimientoTarjeta> movimientos = _context.MovimientoTarjeta.Where(x => x.Usuario.Id == usuario.Id && x.Periodo != null).Where(e=>e.Periodo.FechaHasta<fechaActual).ToList();

            List<ResumenTarjetaDTO> resumenes = movimientos.GroupBy(g => g.Periodo)
                .Select(g => new ResumenTarjetaDTO
                {
                    UsuarioId = usuario.Id,
                    PeriodoId = g.Key.Id,
                    Periodo = g.Key.Descripcion,
                    FechaVencimiento = g.Key.FechaVencimiento.ToString("dd/MM/yyyy"),
                    MontoAdeudado = g.Sum(m => m.Monto)
                })
                .OrderByDescending(r => r.FechaVencimiento)
                .ToList();

            ViewBag.UsuarioId = usuario.Id;
            return PartialView(resumenes);
        }
        catch (Exception ex)
        {
            return Json(new { error = "Se produjo un error al procesar la solicitud: " + ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> DescargarResumen(int periodoId, string usuarioId)
    {
        try
        {
            var datosParaElResumen = await _datosServices.PrepararDatosDTO(periodoId, usuarioId);
            if (datosParaElResumen == null)
            {
                return NotFound("No se encontraron datos para generar el resumen.");
            }

            string html = await _datosServices.RenderViewToStringAsync("ResumenBancarioTemplate", datosParaElResumen);

            byte[] pdfBytes;
            using (var memoryStream = new MemoryStream())
            {
                HtmlConverter.ConvertToPdf(html, memoryStream);
                pdfBytes = memoryStream.ToArray();
            }

            string nombreArchivo = $"Resumen_{datosParaElResumen.Periodo.Descripcion.Replace(" ", "_")}.pdf";
            return File(pdfBytes, "application/pdf", nombreArchivo);
        }
        catch (Exception ex)
        {
            return BadRequest("Ocurrió un error al generar el resumen: " + ex.Message);
        }
    }

    

    /// <summary>
    /// Acción para renderizar la plantilla HTML en el navegador y facilitar el diseño.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> VistaPreviaResumen(int periodoId, string usuarioId)
    {
        try
        {
            // 1. Prepara los datos exactamente igual que para el PDF
            var datosParaElResumen = await _datosServices.PrepararDatosDTO(periodoId, usuarioId);
            if (datosParaElResumen == null)
            {
                return NotFound("No se encontraron datos para la vista previa.");
            }

            // 2. Devuelve la vista directamente, pasándole el modelo.
            // El navegador la renderizará como una página web normal.
            return View("ResumenBancarioTemplate", datosParaElResumen);
        }
        catch (Exception ex)
        {
            return Content("Ocurrió un error al generar la vista previa: " + ex.Message);
        }
    }


    [HttpGet]
    public IActionResult _Filtros()
    {
        return PartialView();
    }

    
}