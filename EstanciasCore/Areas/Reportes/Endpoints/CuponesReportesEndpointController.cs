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

        // GET: /reportes/endpoint/cupones-reportes/todos
        [HttpGet("todos")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                List<HistorialCanje> cupones = await GetBaseQuery()
                    .AsNoTracking()
                    .OrderByDescending(p => p.Fecha)
                    .ToListAsync();

                List<CuponesReportesDTO> data = cupones
                    .Select(MapCuponReporteDTO)
                    .ToList();

                return Ok(new
                {
                    status = 200,
                    mensaje = "Listado completo de cupones obtenido correctamente.",
                    totalRegistros = data.Count,
                    data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    mensaje = "Se produjo un error al obtener el listado completo de cupones.",
                    error = ex.Message
                });
            }
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
                if (model == null)
                {
                    model = new FiltroCuponesReportesViewModel();
                }

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

                if (model.Start < 0)
                {
                    model.Start = 0;
                }

                if (model.Length <= 0)
                {
                    model.Length = 10;
                }

                string searchValue = Request.Form["search[value]"].FirstOrDefault()
                    ?? model.SearchValue
                    ?? "";

                string sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault() ?? "";
                string sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault()
                    ?? model.SortDirection
                    ?? "";

                string sortColumnName = "";

                if (!string.IsNullOrWhiteSpace(sortColumnIndex))
                {
                    sortColumnName = Request.Form[$"columns[{sortColumnIndex}][name]"]
                        .FirstOrDefault() ?? "";
                }

                if (string.IsNullOrWhiteSpace(sortColumnName) &&
                    !string.IsNullOrWhiteSpace(sortColumnIndex))
                {
                    sortColumnName = Request.Form[$"columns[{sortColumnIndex}][data]"]
                        .FirstOrDefault() ?? "";
                }

                if (string.IsNullOrWhiteSpace(sortColumnName))
                {
                    sortColumnName = model.SortColumn ?? "";
                }

                IQueryable<HistorialCanje> query = GetBaseQuery()
                    .AsNoTracking();

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
                    .Select(MapCuponReporteDTO)
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

        // GET unificado:
        // /reportes/endpoint/cupones-reportes/exportar?formato=xlsx
        // /reportes/endpoint/cupones-reportes/exportar?formato=csv
        // /reportes/endpoint/cupones-reportes/exportar?formato=txt
        //
        // Alias compatibles:
        // /reportes/endpoint/cupones-reportes/exportar-excel
        // /reportes/endpoint/cupones-reportes/exportar-csv
        // /reportes/endpoint/cupones-reportes/exportar-txt
        [HttpGet("exportar")]
        [HttpGet("exportar-{tipo}")]
        public Task<IActionResult> ExportarGet(
            [FromQuery] FiltroCuponesReportesViewModel filtros,
            [FromRoute] string tipo = "",
            [FromQuery] string formato = "xlsx",
            [FromQuery] string downloadToken = "")
        {
            string formatoFinal = !string.IsNullOrWhiteSpace(tipo)
                ? tipo
                : formato;

            return ExportarArchivo(filtros, formatoFinal, downloadToken);
        }

        // POST unificado, compatible con multipart/form-data o x-www-form-urlencoded:
        // /reportes/endpoint/cupones-reportes/exportar
        // /reportes/endpoint/cupones-reportes/exportar-excel
        // /reportes/endpoint/cupones-reportes/exportar-csv
        // /reportes/endpoint/cupones-reportes/exportar-txt
        [HttpPost("exportar")]
        [HttpPost("exportar-{tipo}")]
        public Task<IActionResult> ExportarPost(
            [FromForm] FiltroCuponesReportesViewModel filtros,
            [FromRoute] string tipo = "",
            [FromForm] string formato = "xlsx",
            [FromForm] string downloadToken = "")
        {
            string formatoFinal = !string.IsNullOrWhiteSpace(tipo)
                ? tipo
                : formato;

            return ExportarArchivo(filtros, formatoFinal, downloadToken);
        }

        // GET: /reportes/endpoint/cupones-reportes/nombre-apellido-combo-json?q=texto
        [HttpGet("nombre-apellido-combo-json")]
        public IActionResult NombreApellidoComboJson(string q)
        {
            string texto = q ?? "";

            PersonaComboReporteDTO[] items = _context.Usuarios
                .AsNoTracking()
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

        private async Task<IActionResult> ExportarArchivo(
            FiltroCuponesReportesViewModel filtros,
            string formato,
            string downloadToken)
        {
            try
            {
                if (filtros == null)
                {
                    filtros = new FiltroCuponesReportesViewModel();
                }

                string formatoNormalizado = NormalizeExportFormat(formato);

                if (string.IsNullOrWhiteSpace(formatoNormalizado))
                {
                    return BadRequest(new
                    {
                        status = 400,
                        mensaje = "El formato solicitado no es válido.",
                        formatosPermitidos = new[]
                        {
                            "xlsx",
                            "excel",
                            "csv",
                            "txt"
                        }
                    });
                }

                IQueryable<HistorialCanje> query = GetBaseQuery()
                    .AsNoTracking();

                query = ApplyFilters(query, filtros);

                if (!string.IsNullOrWhiteSpace(filtros.SearchValue))
                {
                    query = ApplyGlobalSearch(query, filtros.SearchValue);
                }

                List<HistorialCanje> datos = await query
                    .OrderByDescending(p => p.Fecha)
                    .ToListAsync();

                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string nombreBase = "CuponesCanjeados_" + timestamp;

                byte[] archivo;
                string contentType;
                string extension;

                switch (formatoNormalizado)
                {
                    case "csv":
                        archivo = GenerateCsvBytes(datos);
                        contentType = "text/csv; charset=utf-8";
                        extension = ".csv";
                        break;

                    case "txt":
                        archivo = GenerateTxtBytes(datos);
                        contentType = "text/plain; charset=utf-8";
                        extension = ".txt";
                        break;

                    case "xlsx":
                        archivo = GenerateXlsxBytes(datos);
                        contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        extension = ".xlsx";
                        break;

                    default:
                        return BadRequest(new
                        {
                            status = 400,
                            mensaje = "Formato de exportación no válido."
                        });
                }

                SetDownloadToken(downloadToken);

                Response.Headers["X-Total-Registros"] = datos.Count.ToString();
                Response.Headers["X-Formato-Exportacion"] = formatoNormalizado;

                return File(
                    archivo,
                    contentType,
                    nombreBase + extension
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    mensaje = "Se produjo un error al exportar el reporte de cupones.",
                    error = ex.Message
                });
            }
        }

        private IQueryable<HistorialCanje> GetBaseQuery()
        {
            return _context.HistorialCanje
                .Include(p => p.Cliente)
                    .ThenInclude(c => c.Persona)
                .Include(p => p.Premio)
                .AsQueryable();
        }

        private IQueryable<HistorialCanje> ApplyFilters(
            IQueryable<HistorialCanje> query,
            FiltroCuponesReportesViewModel filtros)
        {
            if (filtros == null)
            {
                return query;
            }

            if (DateTime.TryParse(filtros.FechaDesde, out DateTime fechaDesde))
            {
                DateTime desde = fechaDesde.Date;
                query = query.Where(p => p.Fecha >= desde);
            }

            if (DateTime.TryParse(filtros.FechaHasta, out DateTime fechaHasta))
            {
                DateTime hastaExclusivo = fechaHasta.Date.AddDays(1);
                query = query.Where(p => p.Fecha < hastaExclusivo);
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
                        ((p.Cliente.Persona.Apellido ?? "") + " " +
                         (p.Cliente.Persona.Nombres ?? ""))
                            .ToLower()
                            .Contains(textoPersona) ||

                        ((p.Cliente.Persona.Nombres ?? "") + " " +
                         (p.Cliente.Persona.Apellido ?? ""))
                            .ToLower()
                            .Contains(textoPersona) ||

                        (p.Cliente.Persona.NroDocumento ?? "")
                            .ToLower()
                            .Contains(textoPersona)
                    )
                );
            }

            return query;
        }

        private IQueryable<HistorialCanje> ApplyGlobalSearch(
            IQueryable<HistorialCanje> query,
            string searchValue)
        {
            if (string.IsNullOrWhiteSpace(searchValue))
            {
                return query;
            }

            string texto = searchValue.Trim().ToLower();

            return query.Where(p =>
                (
                    p.Cliente != null &&
                    p.Cliente.Persona != null &&
                    (
                        ((p.Cliente.Persona.Apellido ?? "") + " " +
                         (p.Cliente.Persona.Nombres ?? ""))
                            .ToLower()
                            .Contains(texto) ||

                        ((p.Cliente.Persona.Nombres ?? "") + " " +
                         (p.Cliente.Persona.Apellido ?? ""))
                            .ToLower()
                            .Contains(texto) ||

                        (p.Cliente.Persona.NroDocumento ?? "")
                            .ToLower()
                            .Contains(texto)
                    )
                ) ||
                (
                    p.Premio != null &&
                    (p.Premio.Nombre ?? "")
                        .ToLower()
                        .Contains(texto)
                )
            );
        }

        private IQueryable<HistorialCanje> ApplyOrder(
            IQueryable<HistorialCanje> query,
            string sortColumn,
            string sortDirection)
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
                        ? query.OrderBy(p => p.Cliente.Persona.Apellido)
                            .ThenBy(p => p.Cliente.Persona.Nombres)
                        : query.OrderByDescending(p => p.Cliente.Persona.Apellido)
                            .ThenByDescending(p => p.Cliente.Persona.Nombres);

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

        private string NormalizeExportFormat(string formato)
        {
            string valor = (formato ?? "")
                .Trim()
                .TrimStart('.')
                .ToLower();

            switch (valor)
            {
                case "":
                case "excel":
                case "xlsx":
                    return "xlsx";

                case "csv":
                    return "csv";

                case "txt":
                case "texto":
                    return "txt";

                default:
                    return "";
            }
        }

        private void SetDownloadToken(string downloadToken)
        {
            if (string.IsNullOrWhiteSpace(downloadToken))
            {
                return;
            }

            Response.Cookies.Append(
                downloadToken,
                "true",
                new CookieOptions
                {
                    Path = "/",
                    HttpOnly = false,
                    SameSite = SameSiteMode.Lax,
                    Secure = Request.IsHttps
                }
            );

            Response.Headers["X-Download-Token"] = downloadToken;
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
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Cupones");

                worksheet.Cells[1, 1].Value = "Cliente";
                worksheet.Cells[1, 2].Value = "Nro Documento";
                worksheet.Cells[1, 3].Value = "Fecha";
                worksheet.Cells[1, 4].Value = "Cupón";

                worksheet.Cells[1, 1, 1, 4].Style.Font.Bold = true;

                int fila = 2;

                foreach (HistorialCanje cupon in datos)
                {
                    worksheet.Cells[fila, 1].Value = GetCliente(cupon);
                    worksheet.Cells[fila, 2].Value = GetNroDocumento(cupon);
                    worksheet.Cells[fila, 3].Value = cupon.Fecha;
                    worksheet.Cells[fila, 3].Style.Numberformat.Format = "dd/MM/yyyy";
                    worksheet.Cells[fila, 4].Value = GetCupon(cupon);

                    fila++;
                }

                worksheet.View.FreezePanes(2, 1);

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
            const char delimitador = ';';

            string[] encabezados =
            {
                EscapeCsv("Cliente"),
                EscapeCsv("Nro Documento"),
                EscapeCsv("Fecha"),
                EscapeCsv("Cupón")
            };

            sb.AppendLine(string.Join(
                delimitador.ToString(),
                encabezados
            ));

            foreach (HistorialCanje cupon in datos)
            {
                string[] linea =
                {
                    EscapeCsv(GetCliente(cupon)),
                    EscapeCsv(GetNroDocumento(cupon)),
                    EscapeCsv(cupon.Fecha.ToString("dd/MM/yyyy")),
                    EscapeCsv(GetCupon(cupon))
                };

                sb.AppendLine(string.Join(
                    delimitador.ToString(),
                    linea
                ));
            }

            return GenerateUtf8BomBytes(sb.ToString());
        }

        private byte[] GenerateTxtBytes(List<HistorialCanje> datos)
        {
            var sb = new StringBuilder();
            const char delimitador = '\t';

            sb.AppendLine(
                "Cliente" + delimitador +
                "Nro Documento" + delimitador +
                "Fecha" + delimitador +
                "Cupón"
            );

            foreach (HistorialCanje cupon in datos)
            {
                string[] linea =
                {
                    SanitizeTxt(GetCliente(cupon)),
                    SanitizeTxt(GetNroDocumento(cupon)),
                    cupon.Fecha.ToString("dd/MM/yyyy"),
                    SanitizeTxt(GetCupon(cupon))
                };

                sb.AppendLine(string.Join(
                    delimitador.ToString(),
                    linea
                ));
            }

            return GenerateUtf8BomBytes(sb.ToString());
        }

        private byte[] GenerateUtf8BomBytes(string contenido)
        {
            var encoding = new UTF8Encoding(true);

            byte[] preambulo = encoding.GetPreamble();
            byte[] contenidoBytes = encoding.GetBytes(contenido ?? "");
            byte[] resultado = new byte[preambulo.Length + contenidoBytes.Length];

            Buffer.BlockCopy(
                preambulo,
                0,
                resultado,
                0,
                preambulo.Length
            );

            Buffer.BlockCopy(
                contenidoBytes,
                0,
                resultado,
                preambulo.Length,
                contenidoBytes.Length
            );

            return resultado;
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
            if (historial.Premio != null &&
                !string.IsNullOrWhiteSpace(historial.Premio.Nombre))
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

        private string SanitizeTxt(string value)
        {
            return (value ?? "")
                .Replace("\t", " ")
                .Replace("\r", " ")
                .Replace("\n", " ")
                .Trim();
        }
    }
}
