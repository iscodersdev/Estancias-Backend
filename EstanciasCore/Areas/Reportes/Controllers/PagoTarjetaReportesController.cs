using Commons.Models;
using DAL.Data;
using DAL.DTOs.Reportes;
using DAL.Mobile;
using DAL.Models;
using DAL.Models.Core;
using EstanciasCore.Controllers; 
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
namespace EstanciasCore.Areas.Reportes.Controllers
{
    [Area("Reportes")]
    public class PagoTarjetaReportesController : EstanciasCoreController
    {

        public PagoTarjetaReportesController(EstanciasContext context) : base(context)
        {
            breadcumb.Add(new Commons.Models.Message() { DisplayName = "Datos" });
        }

        public IActionResult Index()
        {
            breadcumb.Add(new Commons.Models.Message() { DisplayName = "Pago Tarjeta Reportes" }); 
            ViewBag.Breadcrumb = breadcumb;
            return View();
        }
                [HttpGet]
        public IActionResult _Filtros()
        {
            return PartialView();
        }



        [HttpPost]
        public async Task<IActionResult> FiltrarPagosTarjeta([FromForm] FiltroPagosViewModel model)
        {
            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var sortColumnName = Request.Form[$"columns[{sortColumnIndex}][name]"].FirstOrDefault();

                IQueryable<PagoTarjeta> query = _getFilteredQuery(model);
                int recordsTotal = await _context.PagoTarjeta.CountAsync();
                int recordsFiltered = await query.CountAsync();

                if (!(string.IsNullOrEmpty(sortColumnName) || string.IsNullOrEmpty(sortColumnDirection)))
                {
                    query = ApplyOrder(query, sortColumnName, sortColumnDirection);
                }
                else
                {
                    query = query.OrderByDescending(p => p.FechaComprobante);
                }

                var pagos = await query.Skip(model.Start).Take(model.Length).ToListAsync();

                return Json(new
                {
                    draw = draw,
                    recordsTotal = recordsTotal,
                    recordsFiltered = recordsFiltered,
                    data = pagos
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Se produjo un error al procesar la solicitud." });
            }
        }


        [HttpGet]
        public async Task<IActionResult> Exportar([FromQuery] FiltroPagosViewModel filtros, string formato, string downloadToken)
        {
            var datos = await _getFilteredQuery(filtros)
                                .OrderByDescending(p => p.FechaComprobante)
                                .ToListAsync();

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string fileName = $"PagosTarjeta_{timestamp}";
            if (!string.IsNullOrEmpty(downloadToken))
            {
                Response.Cookies.Append(downloadToken, "true", new CookieOptions { Path = "/", HttpOnly = false });
            }
            switch (formato?.ToLower())
            {
                case "csv":
                    return File(_generateCsvBytes(datos), "text/csv", $"{fileName}.csv");
                case "txt":
                    return File(_generateTxtBytes(datos), "text/plain", $"{fileName}.txt");
                case "xlsx":
                default:
                    return File(_generateXlsxBytes(datos), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{fileName}.xlsx");
            }
        }



        private IQueryable<PagoTarjeta> _getFilteredQuery(FiltroPagosViewModel filtros)
        {
            IQueryable<PagoTarjeta> query = _context.PagoTarjeta.Include(p => p.Persona);

            if (DateTime.TryParse(filtros.FechaDesde, out DateTime fechaDesde))
                query = query.Where(p => p.FechaComprobante.HasValue && p.FechaComprobante.Value.Date >= fechaDesde.Date);
            if (DateTime.TryParse(filtros.FechaHasta, out DateTime fechaHasta))
                query = query.Where(p => p.FechaComprobante.HasValue && p.FechaComprobante.Value.Date <= fechaHasta.Date);
            if (filtros.EstadoId.HasValue)
                query = query.Where(p => (int)p.EstadoPago == filtros.EstadoId.Value);
            if (filtros.PersonaId.HasValue && filtros.PersonaId > 0)
                query = query.Where(p => p.Persona.Id == filtros.PersonaId.Value);
            if (!string.IsNullOrEmpty(filtros.Monto) && decimal.TryParse(filtros.Monto, out decimal montoDecimal))
                query = query.Where(p => p.MontoAdeudado == montoDecimal);

            return query;
        }

        private IQueryable<PagoTarjeta> ApplyOrder(IQueryable<PagoTarjeta> query, string sortColumn, string sortDirection)
        {
            bool ascending = sortDirection == "asc";
            switch (sortColumn)
            {
                case "FechaComprobante":
                    return ascending ? query.OrderBy(p => p.FechaComprobante) : query.OrderByDescending(p => p.FechaComprobante);
                case "Persona.Apellido":
                    return ascending ? query.OrderBy(p => p.Persona.Apellido) : query.OrderByDescending(p => p.Persona.Apellido);
                default:
                    return query.OrderByDescending(p => p.FechaComprobante);
            }
        }

        private byte[] _generateXlsxBytes(List<PagoTarjeta> datos)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Pagos");
                var dataToExport = datos.Select(p => new
                {
                    Cliente = $"{p.Persona?.Apellido}, {p.Persona?.Nombres}",
                    NroDocumento = p.Persona?.NroDocumento,
                    FechaVencimiento = p.FechaVencimiento?.ToString("dd/MM/yyyy") ?? "",
                    FechaComprobante = p.FechaComprobante?.ToString("dd/MM/yyyy") ?? "",
                    MontoAdeudado = p.MontoAdeudado,
                    MontoInformado = p.MontoInformado,
                    Estado = p.EstadoPago.ToString()
                }).ToList();

                worksheet.Cells.LoadFromCollection(dataToExport, true);
                worksheet.Column(5).Style.Numberformat.Format = "$ #,##0.00";
                worksheet.Column(6).Style.Numberformat.Format = "$ #,##0.00";
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                return package.GetAsByteArray();
            }
        }

        private byte[] _generateCsvBytes(List<PagoTarjeta> datos)
        {
            var sb = new StringBuilder();
            char delimitador = ';';
            sb.AppendLine($"Cliente{delimitador}Nro Documento{delimitador}Fecha Vencimiento{delimitador}Fecha Comprobante{delimitador}Monto Adeudado{delimitador}Monto Informado{delimitador}Estado");
            foreach (var p in datos)
            {
                string[] line = {
                $"\"{p.Persona?.Apellido}, {p.Persona?.Nombres}\"",
                p.Persona?.NroDocumento,
                p.FechaVencimiento?.ToString("dd/MM/yyyy") ?? "",
                p.FechaComprobante?.ToString("dd/MM/yyyy") ?? "",
                p.MontoAdeudado.ToString("F2", CultureInfo.GetCultureInfo("es-AR")),
                p.MontoInformado.ToString("F2", CultureInfo.GetCultureInfo("es-AR")),
                p.EstadoPago.ToString()
            };
                sb.AppendLine(string.Join(delimitador, line));
            }
            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        private byte[] _generateTxtBytes(List<PagoTarjeta> datos)
        {
            var sb = new StringBuilder();
            char delimitador = '\t';
            sb.AppendLine($"Cliente{delimitador}Nro_Documento{delimitador}Fecha_Vencimiento{delimitador}Fecha_Comprobante{delimitador}Monto_Adeudado{delimitador}Monto_Informado{delimitador}Estado");
            foreach (var p in datos)
            {
                string[] line = {
                $"{p.Persona?.Apellido}, {p.Persona?.Nombres}",
                p.Persona?.NroDocumento,
                p.FechaVencimiento?.ToString("dd/MM/yyyy") ?? "",
                p.FechaComprobante?.ToString("dd/MM/yyyy") ?? "",
                p.MontoAdeudado.ToString("F2", CultureInfo.GetCultureInfo("es-AR")),
                p.MontoInformado.ToString("F2", CultureInfo.GetCultureInfo("es-AR")),
                p.EstadoPago.ToString()
            };
                sb.AppendLine(string.Join(delimitador, line));
            }
            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }

}