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

            DateTime fechaActual = DateTime.Now.AddMonths(1);

            List<ResumenTarjeta> movimientos = _context.ResumenTarjeta.Where(x => x.Usuario.Id == usuario.Id && x.Periodo != null).Where(e => e.Periodo.FechaHasta<fechaActual).ToList();

            List<ResumenTarjetaDTO> resumenes = movimientos.GroupBy(g => g.Periodo)
                .Select(g => new ResumenTarjetaDTO
                {
                    UsuarioId = usuario.Id,
                    PeriodoId = g.Key.Id,
                    Periodo = g.Key.Descripcion,
                    FechaVencimiento = g.Key.FechaVencimiento.ToString("dd/MM/yyyy"),
                    Monto = g.Sum(m => m.Monto),
                    Punitorios = g.Sum(m => m.MontoAdeudado)
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
    public async Task<IActionResult> DescargarResumen(string Id)
    {
        if (string.IsNullOrEmpty(Id))
        {
            return BadRequest("El ID no puede ser nulo o vacío.");
        }

        string[] partes = Id.Split(',');
        if (partes.Length != 2)
        {
            return BadRequest("El formato del ID es incorrecto. Se esperaba 'PeriodoId,UsuarioId'.");
        }

        if (!int.TryParse(partes[0], out int periodoId))
        {
            return BadRequest("El PeriodoId proporcionado no es un número válido.");
        }

        string usuarioId = partes[1];

        try
        {
            var resumen = await _context.ResumenTarjeta
                .FirstOrDefaultAsync(x => x.UsuarioId == usuarioId && x.PeriodoId == periodoId);

            if (resumen == null)
            {
                return NotFound("No se encontró un resumen para el período y usuario especificados.");
            }

            if (resumen.Adjunto == null || resumen.Adjunto.Length == 0)
            {
                return NotFound("El resumen fue encontrado pero no contiene un archivo adjunto.");
            }

            string base64String = Convert.ToBase64String(resumen.Adjunto);
            return PartialView("_VerResumen", base64String);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Ocurrió un error interno al procesar la solicitud.");
        }
    }

    [HttpGet]
    public IActionResult _Filtros()
    {
        return PartialView();
    }

    
}