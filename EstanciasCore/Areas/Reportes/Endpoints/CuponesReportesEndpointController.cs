using DAL.Data;
using DAL.DTOs.Reportes;
using DAL.Models;
using DAL.Models.Core;
using EstanciasCore.API.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstanciasCore.Areas.Reportes.Endpoints
{
    [Area("Reportes")]
    [TypeFilter(typeof(EndpointUatAuthorizeAttribute))]
    [ApiController]
    [Route("reportes/endpoint/cupones-reportes")]
    public class CuponesReportesEndpointController : ControllerBase
    {
        private readonly EstanciasContext _context;

        public CuponesReportesEndpointController(EstanciasContext context)
        {
            _context = context;
        }

        // GET: /reportes/endpoint/cupones-reportes
        [HttpGet]
        public IActionResult Index()
        {
            return Ok(new
            {
                status = 200,
                mensaje = "API Reportes - Cupones Reportes",
                endpoints = new
                {
                    filtros = "/reportes/endpoint/cupones-reportes/filtros",
                    filtrarCupones = "/reportes/endpoint/cupones-reportes/filtrar-cupones",
                    exportar = "/reportes/endpoint/cupones-reportes/exportar",
                    nombreApellidoComboJson = "/reportes/endpoint/cupones-reportes/nombre-apellido-combo-json"
                }
            });
        }

        // GET: /reportes/endpoint/cupones-reportes/filtros
        [HttpGet("filtros")]
        public IActionResult Filtros()
        {
            string hoy = DateTime.Today.ToString("yyyy-MM-dd");

            return Ok(new FiltroCuponesReportesViewModel
            {
                FechaDesde = hoy,
                FechaHasta = hoy,
                PersonaId = 0,
                NombrePersona = "",
                Start = 0,
                Length = 10,
                SearchValue = "",
                SortColumn = "",
                SortDirection = ""
            });
        }

        // POST: /reportes/endpoint/cupones-reportes/filtrar-cupones
        [HttpPost("filtrar-cupones")]
        public async Task<IActionResult> FiltrarCupones([FromForm] FiltroCuponesReportesViewModel model)
        {
            try
            {
                string draw = Request.Form["draw"].FirstOrDefault() ?? "1";

                string startRaw = Request.Form["start"].FirstOrDefault() ?? "";
                string lengthRaw = Request.Form["length"].FirstOrDefault() ?? "";

                if (int.TryParse(startRaw, out int start))
                {
                    model.Start = start;
                }

                if (int.TryParse(lengthRaw, out int length))
                {
                    model.Length = length;
                }

                if (model.Length <= 0)
                {
                    model.Length = 10;
                }

                string searchValue = Request.Form["search[value]"].FirstOrDefault() ?? model.SearchValue ?? "";
                string sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault() ?? "";
                string sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault() ?? model.SortDirection ?? "";
                string sortColumnName = "";

                if (!string.IsNullOrWhiteSpace(sortColumnIndex))
                {
                    sortColumnName = Request.Form[$"columns[{sortColumnIndex}][name]"].FirstOrDefault() ?? "";
                }

                if (string.IsNullOrWhiteSpace(sortColumnName) && !string.IsNullOrWhiteSpace(sortColumnIndex))
                {
                    sortColumnName = Request.Form[$"columns[{sortColumnIndex}][data]"].FirstOrDefault() ?? "";
                }

                if (string.IsNullOrWhiteSpace(sortColumnName))
                {
                    sortColumnName = model.SortColumn ?? "";
                }

                IQueryable<HistorialCanje> query = GetBaseQuery();

                int recordsTotal = await query.CountAsync();

                query = ApplyFilters(query, model);
                query = ApplyGlobalSearch(query, searchValue);

                int recordsFiltered = await query.CountAsync();

                query = ApplyOrder(query, sortColumnName, sortColumnDirection);

                List<HistorialCanje> cupones = await query
                    .Skip(model.Start)
                    .Take(model.Length)
                    .ToListAsync();

                List<CuponesReportesDTO> data = cupones
                    .Select(c => MapCuponReporteDTO(c))
                    .ToList();

                return Ok(new CuponesReportesDataTableResponseDTO
                {
                    draw = draw,
                    recordsTotal = recordsTotal,
                    recordsFiltered = recordsFiltered,
                    data = data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<object>(),
                    error = "Se produjo un error al procesar la solicitud: " + ex.Message
                });
            }
        }

        // GET: /reportes/endpoint/cupones-reportes/exportar?formato=xlsx
        // GET: /reportes/endpoint/cupones-reportes/exportar?formato=csv
        // GET: /reportes/endpoint/cupones-reportes/exportar?formato=txt
        [HttpGet("exportar")]
        public async Task<IActionResult> Exportar([FromQuery] FiltroCuponesReportesViewModel filtros, string formato, string downloadToken)
        {
            IQueryable<HistorialCanje> query = GetBaseQuery();

            query = ApplyFilters(query, filtros);

            if (!string.IsNullOrWhiteSpace(filtros.SearchValue))
            {
                query = ApplyGlobalSearch(query, filtros.SearchValue);
            }

            List<HistorialCanje> datos = await query
                .OrderByDescending(p => p.Fecha)
                .ToListAsync();

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string fileName = "CuponesCanjeados_" + timestamp;

            if (!string.IsNullOrEmpty(downloadToken))
            {
                Response.Cookies.Append(downloadToken, "true", new CookieOptions
                {
                    Path = "/",
                    HttpOnly = false
                });
            }

            switch ((formato ?? "xlsx").ToLower())
            {
                case "csv":
                    return File(
                        GenerateCsvBytes(datos),
                        "text/csv",
                        fileName + ".csv"
                    );

                case "txt":
                    return File(
                        GenerateTxtBytes(datos),
                        "text/plain",
                        fileName + ".txt"
                    );

                case "xlsx":
                default:
                    return File(
                        GenerateXlsxBytes(datos),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName + ".xlsx"
                    );
            }
        }

        // Alias opcional para Excel
        // GET: /reportes/endpoint/cupones-reportes/exportar-excel
        [HttpGet("exportar-excel")]
        public async Task<IActionResult> ExportarExcel(string fechaDesde, string fechaHasta, int personaId)
        {
            var filtros = new FiltroCuponesReportesViewModel
            {
                FechaDesde = fechaDesde ?? "",
                FechaHasta = fechaHasta ?? "",
                PersonaId = personaId,
                NombrePersona = "",
                SearchValue = ""
            };

            IQueryable<HistorialCanje> query = GetBaseQuery();
            query = ApplyFilters(query, filtros);

            List<HistorialCanje> datos = await query
                .OrderByDescending(p => p.Fecha)
                .ToListAsync();

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            return File(
                GenerateXlsxBytes(datos),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "CuponesCanjeados_" + timestamp + ".xlsx"
            );
        }

        // Alias opcional para CSV
        // GET: /reportes/endpoint/cupones-reportes/exportar-csv
        [HttpGet("exportar-csv")]
        public async Task<IActionResult> ExportarCsv(string fechaDesde, string fechaHasta, int personaId)
        {
            var filtros = new FiltroCuponesReportesViewModel
            {
                FechaDesde = fechaDesde ?? "",
                FechaHasta = fechaHasta ?? "",
                PersonaId = personaId,
                NombrePersona = "",
                SearchValue = ""
            };

            IQueryable<HistorialCanje> query = GetBaseQuery();
            query = ApplyFilters(query, filtros);

            List<HistorialCanje> datos = await query
                .OrderByDescending(p => p.Fecha)
                .ToListAsync();

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            return File(
                GenerateCsvBytes(datos),
                "text/csv",
                "CuponesCanjeados_" + timestamp + ".csv"
            );
        }

        // Alias opcional para TXT
        // GET: /reportes/endpoint/cupones-reportes/exportar-txt
        [HttpGet("exportar-txt")]
        public async Task<IActionResult> ExportarTxt(string fechaDesde, string fechaHasta, int personaId)
        {
            var filtros = new FiltroCuponesReportesViewModel
            {
                FechaDesde = fechaDesde ?? "",
                FechaHasta = fechaHasta ?? "",
                PersonaId = personaId,
                NombrePersona = "",
                SearchValue = ""
            };

            IQueryable<HistorialCanje> query = GetBaseQuery();
            query = ApplyFilters(query, filtros);

            List<HistorialCanje> datos = await query
                .OrderByDescending(p => p.Fecha)
                .ToListAsync();

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            return File(
                GenerateTxtBytes(datos),
                "text/plain",
                "CuponesCanjeados_" + timestamp + ".txt"
            );
        }

        // GET: /reportes/endpoint/cupones-reportes/nombre-apellido-combo-json?q=texto
        [HttpGet("nombre-apellido-combo-json")]
        public IActionResult NombreApellidoComboJson(string q)
        {
            string texto = q ?? "";

            var items = _context.Usuarios
                .Where(x =>
                    x.Personas != null &&
                    (
                        (x.Personas.Nombres ?? "").Contains(texto) ||
                        (x.Personas.Apellido ?? "").Contains(texto) ||
                        (x.Personas.NroDocumento ?? "").Contains(texto)
                    )
                )
                .Select(x => new PersonaComboReporteDTO
                {
                    Text = $"{x.Personas.Apellido}, {x.Personas.Nombres} - {x.Personas.NroDocumento}",
                    Value = x.Personas.Id,
                    Subtext = $"{x.UserName}",
                    Icon = "fa fa-user"
                })
                .Take(10)
                .ToArray();

            return Ok(items);
        }

        private IQueryable<HistorialCanje> GetBaseQuery()
        {
            return _context.HistorialCanje
                .Include(p => p.Cliente)
                    .ThenInclude(c => c.Persona)
                .Include(p => p.Premio)
                .AsQueryable();
        }

        private IQueryable<HistorialCanje> ApplyFilters(IQueryable<HistorialCanje> query, FiltroCuponesReportesViewModel filtros)
        {
            if (filtros == null)
            {
                return query;
            }

            if (DateTime.TryParse(filtros.FechaDesde, out DateTime fechaDesde))
            {
                query = query.Where(p => p.Fecha.Date >= fechaDesde.Date);
            }

            if (DateTime.TryParse(filtros.FechaHasta, out DateTime fechaHasta))
            {
                query = query.Where(p => p.Fecha.Date <= fechaHasta.Date);
            }

            if (filtros.PersonaId > 0)
            {
                query = query.Where(p =>
                    p.Cliente != null &&
                    p.Cliente.Persona != null &&
                    p.Cliente.Persona.Id == filtros.PersonaId
                );
            }

            if (!string.IsNullOrWhiteSpace(filtros.NombrePersona))
            {
                string textoPersona = filtros.NombrePersona.Trim().ToLower();

                query = query.Where(p =>
                    p.Cliente != null &&
                    p.Cliente.Persona != null &&
                    (
                        ((p.Cliente.Persona.Apellido ?? "") + " " + (p.Cliente.Persona.Nombres ?? "")).ToLower().Contains(textoPersona) ||
                        ((p.Cliente.Persona.Nombres ?? "") + " " + (p.Cliente.Persona.Apellido ?? "")).ToLower().Contains(textoPersona) ||
                        (p.Cliente.Persona.NroDocumento ?? "").ToLower().Contains(textoPersona)
                    )
                );
            }

            return query;
        }

        private IQueryable<HistorialCanje> ApplyGlobalSearch(IQueryable<HistorialCanje> query, string searchValue)
        {
            if (string.IsNullOrWhiteSpace(searchValue))
            {
                return query;
            }

            string texto = searchValue.Trim().ToLower();

            query = query.Where(p =>
                (
                    p.Cliente != null &&
                    p.Cliente.Persona != null &&
                    (
                        ((p.Cliente.Persona.Apellido ?? "") + " " + (p.Cliente.Persona.Nombres ?? "")).ToLower().Contains(texto) ||
                        ((p.Cliente.Persona.Nombres ?? "") + " " + (p.Cliente.Persona.Apellido ?? "")).ToLower().Contains(texto) ||
                        (p.Cliente.Persona.NroDocumento ?? "").ToLower().Contains(texto)
                    )
                ) ||
                (
                    p.Premio != null &&
                    (p.Premio.Nombre ?? "").ToLower().Contains(texto)
                )
            );

            return query;
        }

        private IQueryable<HistorialCanje> ApplyOrder(IQueryable<HistorialCanje> query, string sortColumn, string sortDirection)
        {
            bool ascending = (sortDirection ?? "").ToLower() == "asc";
            string column = NormalizeColumnName(sortColumn);

            switch (column)
            {
                case "cliente":
                case "persona":
                case "personaapellido":
                case "cliente.persona.apellido":
                    return ascending
                        ? query.OrderBy(p => p.Cliente.Persona.Apellido).ThenBy(p => p.Cliente.Persona.Nombres)
                        : query.OrderByDescending(p => p.Cliente.Persona.Apellido).ThenByDescending(p => p.Cliente.Persona.Nombres);

                case "nrodocumento":
                case "documento":
                case "cliente.persona.nrodocumento":
                    return ascending
                        ? query.OrderBy(p => p.Cliente.Persona.NroDocumento)
                        : query.OrderByDescending(p => p.Cliente.Persona.NroDocumento);

                case "fecha":
                case "fechacomprobante":
                    return ascending
                        ? query.OrderBy(p => p.Fecha)
                        : query.OrderByDescending(p => p.Fecha);

                case "cupon":
                case "cupón":
                case "premio":
                case "premionombre":
                case "premio.nombre":
                    return ascending
                        ? query.OrderBy(p => p.Premio.Nombre)
                        : query.OrderByDescending(p => p.Premio.Nombre);

                default:
                    return query.OrderByDescending(p => p.Fecha);
            }
        }

        private string NormalizeColumnName(string sortColumn)
        {
            if (string.IsNullOrWhiteSpace(sortColumn))
            {
                return "";
            }

            return sortColumn
                .Trim()
                .Replace(" ", "")
                .Replace("_", "")
                .Replace("-", "")
                .ToLower();
        }

        private CuponesReportesDTO MapCuponReporteDTO(HistorialCanje historial)
        {
            int clienteId = 0;
            int personaId = 0;
            int premioId = 0;

            string apellido = "";
            string nombres = "";
            string nroDocumento = "";
            string cliente = "";
            string cupon = "";

            if (historial.Cliente != null)
            {
                clienteId = historial.Cliente.Id;

                if (historial.Cliente.Persona != null)
                {
                    personaId = historial.Cliente.Persona.Id;
                    apellido = historial.Cliente.Persona.Apellido ?? "";
                    nombres = historial.Cliente.Persona.Nombres ?? "";
                    nroDocumento = historial.Cliente.Persona.NroDocumento ?? "";
                    cliente = (apellido + ", " + nombres).Trim().Trim(',');
                }
            }

            if (historial.Premio != null)
            {
                premioId = historial.Premio.Id;
                cupon = historial.Premio.Nombre ?? "";
            }

            return new CuponesReportesDTO
            {
                Id = historial.Id,
                ClienteId = clienteId,
                PersonaId = personaId,
                PremioId = premioId,

                Cliente = cliente,
                Apellido = apellido,
                Nombres = nombres,
                NroDocumento = nroDocumento,

                Fecha = historial.Fecha,
                FechaTexto = historial.Fecha.ToString("dd/MM/yyyy"),

                Cupon = cupon,
                PremioNombre = cupon
            };
        }

        private byte[] GenerateXlsxBytes(List<HistorialCanje> datos)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Cupones");

                var dataToExport = datos.Select(p => new CuponesReportesExportDTO
                {
                    Cliente = GetCliente(p),
                    NroDocumento = GetNroDocumento(p),
                    Fecha = p.Fecha.ToString("dd/MM/yyyy"),
                    Cupon = GetCupon(p)
                }).ToList();

                worksheet.Cells.LoadFromCollection(dataToExport, true);

                if (worksheet.Dimension != null)
                {
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                }

                return package.GetAsByteArray();
            }
        }

        private byte[] GenerateCsvBytes(List<HistorialCanje> datos)
        {
            var sb = new StringBuilder();
            char delimitador = ';';

            sb.AppendLine($"Cliente{delimitador}Nro Documento{delimitador}Fecha{delimitador}Cupon");

            foreach (HistorialCanje p in datos)
            {
                string[] line =
                {
                    EscapeCsv(GetCliente(p)),
                    EscapeCsv(GetNroDocumento(p)),
                    p.Fecha.ToString("dd/MM/yyyy"),
                    EscapeCsv(GetCupon(p))
                };

                sb.AppendLine(string.Join(delimitador.ToString(), line));
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        private byte[] GenerateTxtBytes(List<HistorialCanje> datos)
        {
            var sb = new StringBuilder();
            char delimitador = '\t';

            sb.AppendLine($"Cliente{delimitador}Nro Documento{delimitador}Fecha{delimitador}Cupon");

            foreach (HistorialCanje p in datos)
            {
                string[] line =
                {
                    GetCliente(p),
                    GetNroDocumento(p),
                    p.Fecha.ToString("dd/MM/yyyy"),
                    GetCupon(p)
                };

                sb.AppendLine(string.Join(delimitador.ToString(), line));
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        private string GetCliente(HistorialCanje historial)
        {
            if (historial.Cliente != null && historial.Cliente.Persona != null)
            {
                string apellido = historial.Cliente.Persona.Apellido ?? "";
                string nombres = historial.Cliente.Persona.Nombres ?? "";
                string nombreCompleto = (apellido + ", " + nombres).Trim().Trim(',');

                if (!string.IsNullOrWhiteSpace(nombreCompleto))
                {
                    return nombreCompleto;
                }
            }

            return "";
        }

        private string GetNroDocumento(HistorialCanje historial)
        {
            if (historial.Cliente != null &&
                historial.Cliente.Persona != null &&
                !string.IsNullOrWhiteSpace(historial.Cliente.Persona.NroDocumento))
            {
                return historial.Cliente.Persona.NroDocumento;
            }

            return "";
        }

        private string GetCupon(HistorialCanje historial)
        {
            if (historial.Premio != null && !string.IsNullOrWhiteSpace(historial.Premio.Nombre))
            {
                return historial.Premio.Nombre;
            }

            return "";
        }

        private string EscapeCsv(string value)
        {
            string texto = value ?? "";
            texto = texto.Replace("\"", "\"\"");
            return "\"" + texto + "\"";
        }
    }
}