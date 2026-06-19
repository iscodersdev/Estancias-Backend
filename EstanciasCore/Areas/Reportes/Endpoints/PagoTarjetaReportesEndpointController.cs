using DAL.Data;
using DAL.DTOs.Reportes;
using DAL.Models.Core;
using EstanciasCore.API.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstanciasCore.Areas.Reportes.Endpoints
{
    [Area("Reportes")]
    [TypeFilter(typeof(EndpointUatAuthorizeAttribute))]
    [ApiController]
    [Route("reportes/endpoint/pago-tarjeta-reportes")]
    public class PagoTarjetaReportesEndpointController : ControllerBase
    {
        private readonly EstanciasContext _context;

        public PagoTarjetaReportesEndpointController(EstanciasContext context)
        {
            _context = context;
        }

        // GET: /reportes/endpoint/pago-tarjeta-reportes/todos
        [HttpGet("todos")]
        public async Task<IActionResult> GetAll( int pagina = 1, int cantidad = 50)
        {
            try
            {
                if (pagina < 1)
                {
                    pagina = 1;
                }
                if (cantidad < 1)
                {
                    cantidad = 50;
                }
                if(cantidad > 200)
                {
                    cantidad = 200;
                }

                IQueryable<PagoTarjeta> query = GetBaseQuery();

                List<PagoTarjeta> pagos = await query
                    .OrderByDescending(x => x.FechaComprobante)
                    .Skip((pagina-1)*cantidad)
                    .Take(cantidad)
                    .ToListAsync();

                int totalRegistros = await query.CountAsync();

                List<PagoTarjetaReporteDTO> data = pagos
                    .Select(x => MapPagoTarjetaReporteDTO(x))
                    .ToList();

                return Ok(new
                {
                    Status = 200,
                    Mensaje = "Listado completo de pagos con tarjeta obtenido correctamente.",
                    TotalRegistros = totalRegistros,
                    paginaActual = pagina,
                    cantidadPorPagina = cantidad,
                    totalPaginas = (int)Math.Ceiling(totalRegistros/(double)cantidad),
                    Data = data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = 500,
                    Mensaje = "Se produjo un error al obtener el listado completo de pagos con tarjeta: " + ex.Message,
                    TotalRegistros = 0,
                    Data = new List<PagoTarjetaReporteDTO>()
                });
            }
        }

        // GET: /reportes/endpoint/pago-tarjeta-reportes/filtros
        [HttpGet("filtros")]
        public IActionResult Filtros()
        {
            //var hoy = DateTime.Now.ToString("yyyy-MM-dd");

            return Ok(new FiltroPagosViewModel
            {
                FechaDesde = "",
                FechaHasta = "",
                EstadoId = 0,
                PersonaId = 0,
                NombrePersona = "",
                Monto = "",
                Start = 0,
                Length = 10,
                SearchValue = "",
                SortColumn = "",
                SortDirection = ""
            });
        }

        // POST: /reportes/endpoint/pago-tarjeta-reportes/filtrar-pagos-tarjeta
        [HttpPost("filtrar-pagos-tarjeta")]
        public async Task<IActionResult> FiltrarPagosTarjeta([FromForm] FiltroPagosViewModel model)
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

                IQueryable<PagoTarjeta> query = GetBaseQuery();

                int recordsTotal = await query.CountAsync();

                query = ApplyFilters(query, model);
                query = ApplyGlobalSearch(query, searchValue);

                int recordsFiltered = await query.CountAsync();

                query = ApplyOrder(query, sortColumnName, sortColumnDirection);

                List<PagoTarjeta> pagos = await query
                    .Skip(model.Start)
                    .Take(model.Length)
                    .ToListAsync();

                List<PagoTarjetaReporteDTO> data = pagos
                    .Select(p => MapPagoTarjetaReporteDTO(p))
                    .ToList();

                return Ok(new PagoTarjetaReporteDataTableResponseDTO
                {
                    draw = draw,
                    recordsTotal = recordsTotal,
                    recordsFiltered = recordsFiltered,
                    data = data
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    error = "Se produjo un error al procesar la solicitud."
                });
            }
        }

        // GET: /reportes/endpoint/pago-tarjeta-reportes/exportar?formato=xlsx
        // GET: /reportes/endpoint/pago-tarjeta-reportes/exportar?formato=csv
        // GET: /reportes/endpoint/pago-tarjeta-reportes/exportar?formato=txt
        [HttpGet("exportar")]
        public async Task<IActionResult> Exportar([FromQuery] FiltroPagosViewModel filtros, string formato, string downloadToken)
        {
            IQueryable<PagoTarjeta> query = GetBaseQuery();

            query = ApplyFilters(query, filtros);

            if (!string.IsNullOrWhiteSpace(filtros.SearchValue))
            {
                query = ApplyGlobalSearch(query, filtros.SearchValue);
            }

            List<PagoTarjeta> datos = await query
                .OrderByDescending(p => p.FechaComprobante)
                .ToListAsync();

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string fileName = $"PagosTarjeta_{timestamp}";

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
                        $"{fileName}.csv"
                    );

                case "txt":
                    return File(
                        GenerateTxtBytes(datos),
                        "text/plain",
                        $"{fileName}.txt"
                    );

                case "xlsx":
                default:
                    return File(
                        GenerateXlsxBytes(datos),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"{fileName}.xlsx"
                    );
            }
        }

        // GET: /reportes/endpoint/pago-tarjeta-reportes/nombre-apellido-combo-json?q=texto
        [HttpGet("nombre-apellido-combo-json")]
        public IActionResult NombreApellidoComboJson(string q)
        {
            string texto = q ?? "";

            var items = _context.Usuarios
                .Where(x =>
                    x.Personas != null &&
                    (
                        x.Personas.Nombres.Contains(texto) ||
                        x.Personas.Apellido.Contains(texto) ||
                        x.Personas.NroDocumento.Contains(texto)
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

        // GET: /reportes/endpoint/pago-tarjeta-reportes/comprobante/5
        [HttpGet("comprobante/{id:int}")]
        public async Task<IActionResult> Comprobante(int id)
        {
            PagoTarjeta pago = await _context.PagoTarjeta
                .Include(p => p.Persona)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pago == null)
            {
                return NotFound(new
                {
                    status = 404,
                    mensaje = "No se encontró el pago solicitado."
                });
            }

            if (pago.ComprobantePago == null || pago.ComprobantePago.Length == 0)
            {
                return NotFound(new
                {
                    status = 404,
                    mensaje = "El pago no tiene comprobante cargado."
                });
            }

            string nombrePersona = "cliente";

            if (pago.Persona != null)
            {
                nombrePersona = $"{pago.Persona.Apellido}_{pago.Persona.Nombres}".Replace(" ", "_");
            }

            string fileName = $"Comprobante_PagoTarjeta_{pago.Id}_{nombrePersona}.bin";

            return File(
                pago.ComprobantePago,
                "application/octet-stream",
                fileName
            );
        }

        private IQueryable<PagoTarjeta> GetBaseQuery()
        {
            return _context.PagoTarjeta
                .Include(p => p.Persona)
                .AsQueryable();
        }

        private IQueryable<PagoTarjeta> ApplyFilters(IQueryable<PagoTarjeta> query, FiltroPagosViewModel filtros)
        {
            if (filtros == null)
            {
                return query;
            }

            if (DateTime.TryParse(filtros.FechaDesde, out DateTime fechaDesde))
            {
                query = query.Where(p =>
                    p.FechaComprobante.HasValue &&
                    p.FechaComprobante.Value.Date >= fechaDesde.Date
                );
            }

            if (DateTime.TryParse(filtros.FechaHasta, out DateTime fechaHasta))
            {
                query = query.Where(p =>
                    p.FechaComprobante.HasValue &&
                    p.FechaComprobante.Value.Date <= fechaHasta.Date
                );
            }

            if (filtros.EstadoId > 0)
            {
                query = query.Where(p => (int)p.EstadoPago == filtros.EstadoId);
            }

            if (filtros.PersonaId > 0)
            {
                query = query.Where(p =>
                    p.Persona != null &&
                    p.Persona.Id == filtros.PersonaId
                );
            }

            if (!string.IsNullOrWhiteSpace(filtros.NombrePersona))
            {
                string textoPersona = filtros.NombrePersona.Trim().ToLower();

                query = query.Where(p =>
                    p.Persona != null &&
                    (
                        ((p.Persona.Apellido ?? "") + " " + (p.Persona.Nombres ?? "")).ToLower().Contains(textoPersona) ||
                        ((p.Persona.Nombres ?? "") + " " + (p.Persona.Apellido ?? "")).ToLower().Contains(textoPersona) ||
                        (p.Persona.NroDocumento ?? "").ToLower().Contains(textoPersona)
                    )
                );
            }

            decimal montoDecimal;

            if (TryParseDecimalFlexible(filtros.Monto, out montoDecimal))
            {
                query = query.Where(p =>
                    p.MontoAdeudado == montoDecimal ||
                    p.MontoInformado == montoDecimal
                );
            }

            return query;
        }

        private IQueryable<PagoTarjeta> ApplyGlobalSearch(IQueryable<PagoTarjeta> query, string searchValue)
        {
            if (string.IsNullOrWhiteSpace(searchValue))
            {
                return query;
            }

            string texto = searchValue.Trim().ToLower();

            decimal montoDecimal;
            bool buscaMonto = TryParseDecimalFlexible(texto, out montoDecimal);

            query = query.Where(p =>
                p.Persona != null &&
                (
                    ((p.Persona.Apellido ?? "") + " " + (p.Persona.Nombres ?? "")).ToLower().Contains(texto) ||
                    ((p.Persona.Nombres ?? "") + " " + (p.Persona.Apellido ?? "")).ToLower().Contains(texto) ||
                    (p.Persona.NroDocumento ?? "").ToLower().Contains(texto) ||
                    (p.NroTarjeta ?? "").ToLower().Contains(texto) ||
                    (p.Observacion ?? "").ToLower().Contains(texto) ||
                    p.EstadoPago.ToString().ToLower().Contains(texto) ||
                    (buscaMonto && (p.MontoAdeudado == montoDecimal || p.MontoInformado == montoDecimal))
                )
            );

            return query;
        }

        private IQueryable<PagoTarjeta> ApplyOrder(IQueryable<PagoTarjeta> query, string sortColumn, string sortDirection)
        {
            bool ascending = (sortDirection ?? "").ToLower() == "asc";
            string column = NormalizeColumnName(sortColumn);

            switch (column)
            {
                case "cliente":
                case "persona":
                case "persona.apellido":
                case "apellido":
                    return ascending
                        ? query.OrderBy(p => p.Persona.Apellido).ThenBy(p => p.Persona.Nombres)
                        : query.OrderByDescending(p => p.Persona.Apellido).ThenByDescending(p => p.Persona.Nombres);

                case "nrodocumento":
                case "persona.nrodocumento":
                    return ascending
                        ? query.OrderBy(p => p.Persona.NroDocumento)
                        : query.OrderByDescending(p => p.Persona.NroDocumento);

                case "fechavencimiento":
                    return ascending
                        ? query.OrderBy(p => p.FechaVencimiento)
                        : query.OrderByDescending(p => p.FechaVencimiento);

                case "fechacomprobante":
                    return ascending
                        ? query.OrderBy(p => p.FechaComprobante)
                        : query.OrderByDescending(p => p.FechaComprobante);

                case "estado":
                case "estadopago":
                    return ascending
                        ? query.OrderBy(p => p.EstadoPago)
                        : query.OrderByDescending(p => p.EstadoPago);

                case "montoadeudado":
                    return ascending
                        ? query.OrderBy(p => p.MontoAdeudado)
                        : query.OrderByDescending(p => p.MontoAdeudado);

                case "montoinformado":
                    return ascending
                        ? query.OrderBy(p => p.MontoInformado)
                        : query.OrderByDescending(p => p.MontoInformado);

                case "nrotarjeta":
                    return ascending
                        ? query.OrderBy(p => p.NroTarjeta)
                        : query.OrderByDescending(p => p.NroTarjeta);

                default:
                    return query.OrderByDescending(p => p.FechaComprobante);
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
                .ToLower();
        }

        private PagoTarjetaReporteDTO MapPagoTarjetaReporteDTO(PagoTarjeta p)
        {
            string apellido = "";
            string nombres = "";
            string nroDocumento = "";
            int personaId = 0;

            if (p.Persona != null)
            {
                personaId = p.Persona.Id;
                apellido = p.Persona.Apellido ?? "";
                nombres = p.Persona.Nombres ?? "";
                nroDocumento = p.Persona.NroDocumento ?? "";
            }

            string cliente = $"{apellido} {nombres}".Trim();

            if (string.IsNullOrWhiteSpace(cliente))
            {
                cliente = apellido;
            }

            bool tieneComprobante = p.ComprobantePago != null && p.ComprobantePago.Length > 0;

            return new PagoTarjetaReporteDTO
            {
                Id = p.Id,
                PersonaId = personaId,

                Cliente = cliente,
                Apellido = apellido,
                Nombres = nombres,
                NroDocumento = nroDocumento,

                NroTarjeta = p.NroTarjeta ?? "",
                Observacion = p.Observacion ?? "",

                FechaVencimiento = p.FechaVencimiento.HasValue ? p.FechaVencimiento.Value : DateTime.MinValue,
                FechaVencimientoTexto = p.FechaVencimiento.HasValue ? p.FechaVencimiento.Value.ToString("dd/MM/yyyy") : "",

                MontoAdeudado = p.MontoAdeudado,
                MontoAdeudadoTexto = p.MontoAdeudado.ToString("C2", CultureInfo.GetCultureInfo("es-AR")),

                MontoInformado = p.MontoInformado,
                MontoInformadoTexto = p.MontoInformado.ToString("C2", CultureInfo.GetCultureInfo("es-AR")),

                FechaPagoProximaCuota = p.FechaPagoProximaCuota.HasValue ? p.FechaPagoProximaCuota.Value : DateTime.MinValue,
                FechaPagoProximaCuotaTexto = p.FechaPagoProximaCuota.HasValue ? p.FechaPagoProximaCuota.Value.ToString("dd/MM/yyyy") : "",

                FechaComprobante = p.FechaComprobante.HasValue ? p.FechaComprobante.Value : DateTime.MinValue,
                FechaComprobanteTexto = p.FechaComprobante.HasValue ? p.FechaComprobante.Value.ToString("dd/MM/yyyy") : "",

                FechaDePago = p.FechaDePago.HasValue ? p.FechaDePago.Value : DateTime.MinValue,
                FechaDePagoTexto = p.FechaDePago.HasValue ? p.FechaDePago.Value.ToString("dd/MM/yyyy") : "",

                EstadoPago = p.EstadoPago.ToString(),
                EstadoPagoTexto = p.EstadoPago.ToString(),
                EstadoCssClass = GetEstadoCssClass(p.EstadoPago.ToString()),

                TieneComprobantePago = tieneComprobante,
                ComprobantePagoUrl = tieneComprobante
                    ? $"/reportes/endpoint/pago-tarjeta-reportes/comprobante/{p.Id}"
                    : "",

                Acciones = tieneComprobante
                    ? $"<a href=\"/reportes/endpoint/pago-tarjeta-reportes/comprobante/{p.Id}\" class=\"btn btn-warning btn-xs\" target=\"_blank\"><i class=\"fa fa-file-image-o\"></i></a>"
                    : ""
            };
        }

        private string GetEstadoCssClass(string estado)
        {
            string estadoNormalizado = (estado ?? "").ToLower();

            if (estadoNormalizado.Contains("pagado"))
            {
                return "badge badge-primary";
            }

            if (estadoNormalizado.Contains("pendiente"))
            {
                return "badge badge-warning";
            }

            if (estadoNormalizado.Contains("rechazado"))
            {
                return "badge badge-danger";
            }

            return "badge badge-secondary";
        }

        private byte[] GenerateXlsxBytes(List<PagoTarjeta> datos)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Pagos");

                var dataToExport = datos.Select(p => new PagoTarjetaExportDTO
                {
                    Cliente = p.Persona != null
                        ? $"{p.Persona.Apellido}, {p.Persona.Nombres}"
                        : "",
                    NroDocumento = p.Persona != null
                        ? p.Persona.NroDocumento ?? ""
                        : "",
                    FechaVencimiento = p.FechaVencimiento.HasValue
                        ? p.FechaVencimiento.Value.ToString("dd/MM/yyyy")
                        : "",
                    FechaComprobante = p.FechaComprobante.HasValue
                        ? p.FechaComprobante.Value.ToString("dd/MM/yyyy")
                        : "",
                    Estado = p.EstadoPago.ToString(),
                    MontoAdeudado = p.MontoAdeudado,
                    MontoInformado = p.MontoInformado
                }).ToList();

                worksheet.Cells.LoadFromCollection(dataToExport, true);

                worksheet.Column(6).Style.Numberformat.Format = "$ #,##0.00";
                worksheet.Column(7).Style.Numberformat.Format = "$ #,##0.00";

                if (worksheet.Dimension != null)
                {
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                }

                return package.GetAsByteArray();
            }
        }

        private byte[] GenerateCsvBytes(List<PagoTarjeta> datos)
        {
            var sb = new StringBuilder();
            char delimitador = ';';

            sb.AppendLine($"Cliente{delimitador}Nro Documento{delimitador}Fecha Vencimiento{delimitador}Fecha Comprobante{delimitador}Estado{delimitador}Monto Adeudado{delimitador}Monto Informado");

            foreach (PagoTarjeta p in datos)
            {
                string cliente = p.Persona != null
                    ? $"{p.Persona.Apellido}, {p.Persona.Nombres}"
                    : "";

                string nroDocumento = p.Persona != null
                    ? p.Persona.NroDocumento ?? ""
                    : "";

                string[] line =
                {
                    EscapeCsv(cliente),
                    EscapeCsv(nroDocumento),
                    p.FechaVencimiento.HasValue ? p.FechaVencimiento.Value.ToString("dd/MM/yyyy") : "",
                    p.FechaComprobante.HasValue ? p.FechaComprobante.Value.ToString("dd/MM/yyyy") : "",
                    EscapeCsv(p.EstadoPago.ToString()),
                    p.MontoAdeudado.ToString("F2", CultureInfo.GetCultureInfo("es-AR")),
                    p.MontoInformado.ToString("F2", CultureInfo.GetCultureInfo("es-AR"))
                };

                sb.AppendLine(string.Join(delimitador.ToString(), line));
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        private byte[] GenerateTxtBytes(List<PagoTarjeta> datos)
        {
            var sb = new StringBuilder();
            char delimitador = '\t';

            sb.AppendLine($"Cliente{delimitador}Nro_Documento{delimitador}Fecha_Vencimiento{delimitador}Fecha_Comprobante{delimitador}Estado{delimitador}Monto_Adeudado{delimitador}Monto_Informado");

            foreach (PagoTarjeta p in datos)
            {
                string cliente = p.Persona != null
                    ? $"{p.Persona.Apellido}, {p.Persona.Nombres}"
                    : "";

                string nroDocumento = p.Persona != null
                    ? p.Persona.NroDocumento ?? ""
                    : "";

                string[] line =
                {
                    cliente,
                    nroDocumento,
                    p.FechaVencimiento.HasValue ? p.FechaVencimiento.Value.ToString("dd/MM/yyyy") : "",
                    p.FechaComprobante.HasValue ? p.FechaComprobante.Value.ToString("dd/MM/yyyy") : "",
                    p.EstadoPago.ToString(),
                    p.MontoAdeudado.ToString("F2", CultureInfo.GetCultureInfo("es-AR")),
                    p.MontoInformado.ToString("F2", CultureInfo.GetCultureInfo("es-AR"))
                };

                sb.AppendLine(string.Join(delimitador.ToString(), line));
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        private string EscapeCsv(string value)
        {
            string texto = value ?? "";
            texto = texto.Replace("\"", "\"\"");
            return $"\"{texto}\"";
        }

        private bool TryParseDecimalFlexible(string value, out decimal result)
        {
            result = 0;

            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            string texto = value.Trim()
                .Replace("$", "")
                .Replace(" ", "");

            CultureInfo esAr = CultureInfo.GetCultureInfo("es-AR");
            CultureInfo invariant = CultureInfo.InvariantCulture;

            if (decimal.TryParse(texto, NumberStyles.Any, esAr, out result))
            {
                return true;
            }

            if (decimal.TryParse(texto, NumberStyles.Any, invariant, out result))
            {
                return true;
            }

            string normalizado = texto.Replace(".", "").Replace(",", ".");

            if (decimal.TryParse(normalizado, NumberStyles.Any, invariant, out result))
            {
                return true;
            }

            return false;
        }
    }
}