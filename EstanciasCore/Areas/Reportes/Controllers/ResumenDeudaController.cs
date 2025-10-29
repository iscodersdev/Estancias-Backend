using Commons.Models;
using DAL.Data;
using DAL.DTOs.Reportes;
using DAL.Mobile;
using DAL.Models;
using EstanciasCore.Controllers; // Tu controlador base
using EstanciasCore.Interface; // Para IDatosTarjetaService
using EstanciasCore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

public class DetalleCuotaConSolicitud
{
    public string NroSolicitud { get; set; } 
    public string Fecha { get; set; } 
    public string Monto { get; set; } 
    public string NroCuota { get; set; } 
}


[Area("Reportes")]
public class ResumenDeudaController : EstanciasCoreController
{
    private readonly IDatosTarjetaService _datosServices;
    private readonly ICompositeViewEngine _viewEngine;

    public ResumenDeudaController(EstanciasContext context, IDatosTarjetaService datosServices, ICompositeViewEngine viewEngine) : base(context)
    {
        breadcumb.Add(new Message() { DisplayName = "Reportes" });
        _datosServices = datosServices;
        _viewEngine = viewEngine;
    }

    public IActionResult Index()
    {
        breadcumb.Add(new Message() { DisplayName = "Resumen de Deuda" });
        ViewBag.Breadcrumb = breadcumb;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> _ListadoDeuda(string nroTarjetaFiltro, string nroDocumentoFiltro)
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



            int LoanPersonaId = 0;
            decimal totalMontoCuota = 0;
            decimal montoPunitoriosTotal = 0;
            var fechaActual = DateTime.Now;
            //var fechaActual = new DateTime(2025, 07, 4);
            List<MovimientoTarjetaDTO> comprasAgrupadas = new List<MovimientoTarjetaDTO>();
            var fechaVentimiento = common.ObtenerFechaCalculada(fechaActual);        
            DatosEstructura empresa = _context.DatosEstructura.FirstOrDefault();


            var datosMovimientos = _datosServices.ConsultarMovimientos(empresa.UsernameWS.ToLower(), empresa.PasswordWS, usuario.Personas.NroDocumento, Convert.ToInt64(usuario.Personas.NroTarjeta), 10, 0).Result;
            var montoDisponible = "0";
            if (datosMovimientos.Detalle.Resultado=="EXITO")
            {
                montoDisponible = datosMovimientos.Detalle.MontoDisponible;

                comprasAgrupadas = datosMovimientos.Movimientos.Where(x => x.Descripcion=="PAGOS DE CUOTA REGULAR")
                .GroupBy(m => new { m.Descripcion, m.Fecha })
                .Select(g => new MovimientoTarjetaDTO
                {
                    Monto =  (g.Sum(m => Convert.ToDecimal(m.Monto.Replace(",", ".")) + Convert.ToDecimal(m.Recargo.Replace(",", "."))).ToString().Replace(".", ","))==null ? g.Sum(m => Convert.ToDecimal(m.Monto.Replace(",", "."))).ToString().Replace(".", ",") : (g.Sum(m => Convert.ToDecimal(m.Monto.Replace(",", ".")) + Convert.ToDecimal(m.Recargo.Replace(",", "."))).ToString().Replace(".", ",")),
                    TipoMovimiento = g.Key.Descripcion,
                    Fecha = g.Key.Fecha.Date.ToString("dd/MM/yyyy")
                })
                .ToList();

                comprasAgrupadas.AddRange(datosMovimientos.Movimientos.Where(x => x.Descripcion!="PAGOS DE CUOTA REGULAR")
                .Select(g => new MovimientoTarjetaDTO
                {
                    Monto = g.Monto.Replace(",", ".").ToString().Replace(".", ","),
                    TipoMovimiento = g.Descripcion,
                    Fecha = g.Fecha.Date.ToString("dd/MM/yyyy")
                }).ToList());

                var totalDetallesCuota = datosMovimientos.DetallesSolicitud
                    .Where(result => result?.DetallesCuota != null)
                    .SelectMany(result => result.DetallesCuota,
                                (result, detalle) => new DetalleCuotaConSolicitud
                                {
                                    NroSolicitud = result.NumeroSolicitud,
                                    NroCuota = detalle.NumeroCuota,
                                    Monto = detalle.Monto,
                                    Fecha = detalle.Fecha,
                                })
                    .Where(x => common.ConvertirFecha(x.Fecha) <= common.ConvertirFecha(fechaVentimiento))
                    .ToList(); 

                //totalMontoCuota = totalDetallesCuota.Sum(e => Convert.ToDecimal(e.monto.Replace(".", ",")));
                //CultureInfo.CurrentCulture = new CultureInfo("es-AR");
                montoPunitoriosTotal = await _datosServices.CalcularPunitorios(datosMovimientos.DetallesSolicitud);
            }

            ViewBag.UsuarioId = usuario.Id;
            return PartialView();
        }
        catch (Exception ex)
        {
            return Json(new { error = "Se produjo un error al procesar la solicitud: " + ex.Message });
        }
    }

    //[HttpGet]
    //public async Task<IActionResult> DescargarResumen(int periodoId, string usuarioId)
    //{
    //    try
    //    {
    //        var datosParaElResumen = await _datosServices.PrepararDatosDTO(periodoId, usuarioId);
    //        var datosParaElResumen = null;
    //        if (datosParaElResumen == null)
    //        {
    //            return NotFound("No se encontraron datos para generar el resumen.");
    //        }

    //        string html = await _datosServices.RenderViewToStringAsync("ResumenBancarioTemplate", datosParaElResumen);
    //        string html = null;

    //        byte[] pdfBytes;
    //        using (var memoryStream = new MemoryStream())
    //        {
    //            HtmlConverter.ConvertToPdf(html, memoryStream);
    //            pdfBytes = memoryStream.ToArray();
    //        }

    //        string nombreArchivo = $"Resumen_{datosParaElResumen.Periodo.Descripcion.Replace(" ", "_")}.pdf";
    //        return File(pdfBytes, "application/pdf", nombreArchivo);
    //    }
    //    catch (Exception ex)
    //    {
    //        return BadRequest("Ocurrió un error al generar el resumen: " + ex.Message);
    //    }
    //}



    /// <summary>
    /// Acción para renderizar la plantilla HTML en el navegador y facilitar el diseño.
    /// </summary>
    //[HttpGet]
    //public async Task<IActionResult> VistaPreviaResumen(int periodoId, string usuarioId)
    //{
    //    try
    //    {
    //        1.Prepara los datos exactamente igual que para el PDF
    //        var datosParaElResumen = await _datosServices.PrepararDatosDTO(periodoId, usuarioId);
    //        if (datosParaElResumen == null)
    //        {
    //            return NotFound("No se encontraron datos para la vista previa.");
    //        }

    //        2.Devuelve la vista directamente, pasándole el modelo.
    //         El navegador la renderizará como una página web normal.
    //        return View("ResumenBancarioTemplate", datosParaElResumen);
    //    }
    //    catch (Exception ex)
    //    {
    //        return Content("Ocurrió un error al generar la vista previa: " + ex.Message);
    //    }
    //}


    [HttpGet]
    public IActionResult _Filtros()
    {
        return PartialView();
    }

    
}