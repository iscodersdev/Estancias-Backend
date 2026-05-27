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
    public class CuponesReportesController : EstanciasCoreController
    {

        public CuponesReportesController(EstanciasContext context) : base(context)
        {
            breadcumb.Add(new Commons.Models.Message() { DisplayName = "Datos" });
        }

        public IActionResult Index()
        {
            breadcumb.Add(new Commons.Models.Message() { DisplayName = "Cupones Reportes" });
            ViewBag.Breadcrumb = breadcumb;
            return View();
        }
        [HttpGet]
        public IActionResult _Filtros()
        {
            return PartialView();
        }



        [HttpPost]
        public async Task<IActionResult> FiltrarCupones([FromForm] FiltroPagosViewModel model)
        {
            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var sortColumnName = Request.Form[$"columns[{sortColumnIndex}][name]"].FirstOrDefault();

                IQueryable<HistorialCanje> query = _getFilteredQuery(model);
                int recordsTotal = await _context.HistorialCanje.CountAsync();
                int recordsFiltered = await query.CountAsync();

                if (!(string.IsNullOrEmpty(sortColumnName) || string.IsNullOrEmpty(sortColumnDirection)))
                {
                    query = ApplyOrder(query, sortColumnName, sortColumnDirection);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Fecha);
                }

                var pagos = await query.Skip(model.Start).Take(model.Length).ToListAsync();

                var dataToReturn = pagos.Select(p => new
                {
                    Id = p.Id,
                    Cliente = p.Cliente != null ? new
                    {
                        Persona = p.Cliente.Persona != null ? new
                        {
                            Apellido = p.Cliente.Persona.Apellido,
                            Nombres = p.Cliente.Persona.Nombres,
                            NroDocumento = p.Cliente.Persona.NroDocumento
                        } : null
                    } : null,
                    Fecha = p.Fecha,
                    Premio = p.Premio != null ? new
                    {
                        Nombre = p.Premio.Nombre
                    } : null
                }).ToList();

                return Json(new
                {
                    draw = draw,
                    recordsTotal = recordsTotal,
                    recordsFiltered = recordsFiltered,
                    data = dataToReturn
                });
            }
            catch (Exception ex)
            {
                try
                {
                    string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error_log_cupones.txt");
                    System.IO.File.WriteAllText(path, ex.ToString());
                }
                catch { }

                string fullMsg = ex.Message;
                if (ex.InnerException != null)
                {
                    fullMsg += " | Inner: " + ex.InnerException.Message;
                }
                return StatusCode(500, new { error = "Se produjo un error al procesar la solicitud: " + fullMsg });
            }
        }


        [HttpGet]
        public async Task<IActionResult> Exportar([FromQuery] FiltroPagosViewModel filtros, string formato, string downloadToken)
        {
            var datos = await _getFilteredQuery(filtros)
                                .OrderByDescending(p => p.Fecha)
                                .ToListAsync();

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string fileName = $"CuponesCanjeados{timestamp}";
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

        public JsonResult NombreApellidoComboJson(string q)
        {
            var items = _context.Usuarios
                .Where(x => x.Personas.Nombres.Contains(q) || x.Personas.Apellido.Contains(q) || x.Personas.NroDocumento.Contains(q))
                .Select(x => new
                {
                    Text = $"{x.Personas.Apellido}, {x.Personas.Nombres} - {x.Personas.NroDocumento}",
                    Value = x.Personas.Id,
                    Subtext = $"{x.UserName}",
                    Icon = "fa fa-user"
                }).Take(10).ToArray();

            return Json(items);
        }



        private IQueryable<HistorialCanje> _getFilteredQuery(FiltroPagosViewModel filtros)
        {
            IQueryable<HistorialCanje> query = _context.HistorialCanje
                .Include(p => p.Cliente)
                    .ThenInclude(c => c.Persona)
                .Include(p => p.Premio);

            if (DateTime.TryParse(filtros.FechaDesde, out DateTime fechaDesde))
                query = query.Where(p => p.Fecha.Date >= fechaDesde.Date);

            if (DateTime.TryParse(filtros.FechaHasta, out DateTime fechaHasta))
                query = query.Where(p => p.Fecha.Date <= fechaHasta.Date);

            if (filtros.PersonaId > 0)
                query = query.Where(p => p.Cliente.Persona.Id == filtros.PersonaId);

            return query;
        }

        private IQueryable<HistorialCanje> ApplyOrder(IQueryable<HistorialCanje> query, string sortColumn, string sortDirection)
        {
            bool ascending = sortDirection == "asc";
            switch (sortColumn)
            {
                case "Fecha":
                case "FechaComprobante":
                    return ascending ? query.OrderBy(p => p.Fecha) : query.OrderByDescending(p => p.Fecha);
                case "Cliente.Persona.Apellido":
                case "Persona.Apellido":
                    return ascending ? query.OrderBy(p => p.Cliente.Persona.Apellido) : query.OrderByDescending(p => p.Cliente.Persona.Apellido);
                case "Premio.Nombre":
                    return ascending ? query.OrderBy(p => p.Premio.Nombre) : query.OrderByDescending(p => p.Premio.Nombre);
                default:
                    return query.OrderByDescending(p => p.Fecha);
            }
        }

        private byte[] _generateXlsxBytes(List<HistorialCanje> datos)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Pagos");
                var dataToExport = datos.Select(p => new
                {
                    Cliente = $"{p.Cliente?.Persona?.Apellido}, {p.Cliente?.Persona?.Nombres}",
                    NroDocumento = p.Cliente?.Persona?.NroDocumento,
                    Cupon = p.Premio?.Nombre,
                    Fecha = p.Fecha.ToString("dd/MM/yyyy") ?? ""
                }).ToList();

                worksheet.Cells.LoadFromCollection(dataToExport, true);
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                return package.GetAsByteArray();
            }
        }

        private byte[] _generateCsvBytes(List<HistorialCanje> datos)
        {
            var sb = new StringBuilder();
            char delimitador = ';';
            sb.AppendLine($"Cliente{delimitador}Nro Documento{delimitador}Cupón{delimitador}Fecha");
            foreach (var p in datos)
            {
                string[] line = {
                $"\"{p.Cliente?.Persona?.Apellido}, {p.Cliente?.Persona?.Nombres}\"",
                p.Cliente?.Persona?.NroDocumento,
                p.Premio?.Nombre,
                p.Fecha.ToString("dd/MM/yyyy") ?? ""
            };
                sb.AppendLine(string.Join(delimitador, line));
            }
            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        private byte[] _generateTxtBytes(List<HistorialCanje> datos)
        {
            var sb = new StringBuilder();
            char delimitador = '\t';
            sb.AppendLine($"Cliente{delimitador}Nro Documento{delimitador}Cupón{delimitador}Fecha");
            foreach (var p in datos)
            {
                string[] line = {
                $"\"{p.Cliente?.Persona?.Apellido}, {p.Cliente?.Persona?.Nombres}\"",
                p.Cliente?.Persona?.NroDocumento,
                p.Premio?.Nombre,
                p.Fecha.ToString("dd/MM/yyyy") ?? ""
            };
                sb.AppendLine(string.Join(delimitador, line));
            }
            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }

}