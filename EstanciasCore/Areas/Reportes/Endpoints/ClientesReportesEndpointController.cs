using DAL.Data;
using DAL.DTOs.Reportes;
using DAL.Models;
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
    [Route("reportes/endpoint/clientes-reportes")]
    public class ClientesReportesEndpointController : ControllerBase
    {
        private readonly EstanciasContext _context;

        public ClientesReportesEndpointController(EstanciasContext context)
        {
            _context = context;
        }

        // GET: /reportes/endpoint/clientes-reportes/todos?pagina=1&cantidad=50&buscar=
        [HttpGet("todos")]
        public async Task<IActionResult> GetAll(int pagina = 1, int cantidad = 50, string buscar = "")
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

                // Evita que el endpoint vuelva a intentar traer miles de registros juntos.
                if (cantidad > 200)
                {
                    cantidad = 200;
                }

                buscar = buscar ?? "";
                string texto = buscar.Trim().ToLower();

                IQueryable<Clientes> query = _context.Clientes
                    .AsNoTracking()
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(texto))
                {
                    query = query.Where(c =>
                        (
                            c.Usuario != null &&
                            (c.Usuario.UserName ?? "").ToLower().Contains(texto)
                        ) ||
                        (
                            c.Persona != null &&
                            (
                                (c.Persona.NroDocumento ?? "").ToLower().Contains(texto) ||
                                (c.Persona.NroTarjeta ?? "").ToLower().Contains(texto) ||
                                ((c.Persona.Apellido ?? "") + " " + (c.Persona.Nombres ?? "")).ToLower().Contains(texto) ||
                                ((c.Persona.Nombres ?? "") + " " + (c.Persona.Apellido ?? "")).ToLower().Contains(texto)
                            )
                        )
                    );
                }

                int totalRegistros = await query.CountAsync();

                var registros = await query
                    .OrderBy(c => c.FechaIngreso)
                    .Skip((pagina - 1) * cantidad)
                    .Take(cantidad)
                    .Select(c => new
                    {
                        Id = c.Id,
                        PersonaId = c.Persona != null ? c.Persona.Id : 0,
                        UsuarioId = c.Usuario != null ? c.Usuario.Id : "",
                        Mail = c.Usuario != null ? c.Usuario.UserName : null,
                        NroDocumento = c.Persona != null ? c.Persona.NroDocumento : null,
                        Apellido = c.Persona != null ? c.Persona.Apellido : null,
                        Nombres = c.Persona != null ? c.Persona.Nombres : null,
                        NroTarjeta = c.Persona != null ? c.Persona.NroTarjeta : null,
                        FechaIngreso = c.FechaIngreso
                    })
                    .ToListAsync();

                List<ClienteReporteDTO> data = registros
                    .Select(c =>
                    {
                        string apellido = c.Apellido ?? "";
                        string nombres = c.Nombres ?? "";
                        string nombreCompleto = (apellido + " " + nombres).Trim();

                        return new ClienteReporteDTO
                        {
                            Id = c.Id,
                            PersonaId = c.PersonaId,
                            UsuarioId = c.UsuarioId ?? "",
                            Mail = c.Mail ?? "Sin Datos",
                            NroDocumento = c.NroDocumento ?? "Sin Datos",
                            NombreCompleto = string.IsNullOrWhiteSpace(nombreCompleto) ? "Sin Datos" : nombreCompleto,
                            NroTarjeta = c.NroTarjeta ?? "Sin Datos",
                            FechaIngreso = c.FechaIngreso,
                            FechaIngresoTexto = c.FechaIngreso.ToString("dd/MM/yyyy")
                        };
                    })
                    .ToList();

                return Ok(new
                {
                    status = 200,
                    mensaje = "Listado de clientes obtenido correctamente.",
                    totalRegistros = totalRegistros,
                    paginaActual = pagina,
                    cantidadPorPagina = cantidad,
                    totalPaginas = (int)Math.Ceiling(totalRegistros / (double)cantidad),
                    data = data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    mensaje = "Se produjo un error al obtener el listado de clientes.",
                    error = ex.Message
                });
            }
        }

        // GET: /reportes/endpoint/clientes-reportes/filtros
        [HttpGet("filtros")]
        public IActionResult Filtros()
        {
            return Ok(new FiltroClientesReporteViewModel
            {
                FechaIngresoDesdeFiltro = DateTime.Today.AddYears(-3).ToString("yyyy-MM-dd"),
                FechaIngresoHastaFiltro = DateTime.Today.ToString("yyyy-MM-dd"),
                Start = 0,
                Length = 10,
                SearchValue = "",
                SortColumn = "",
                SortDirection = ""
            });
        }

        // POST: /reportes/endpoint/clientes-reportes/listado-clientes
        [HttpPost("listado-clientes")]
        public async Task<IActionResult> ListadoClientes([FromForm] FiltroClientesReporteViewModel model)
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

                IQueryable<Clientes> query = GetBaseQuery();

                int recordsTotal = await query.CountAsync();

                query = ApplyFilters(query, model);
                query = ApplyGlobalSearch(query, searchValue);

                int recordsFiltered = await query.CountAsync();

                query = ApplyOrder(query, sortColumnName, sortColumnDirection);

                List<Clientes> clientes = await query
                    .Skip(model.Start)
                    .Take(model.Length)
                    .ToListAsync();

                List<ClienteReporteDTO> data = clientes
                    .Select(c => MapClienteReporteDTO(c))
                    .ToList();

                return Ok(new ClienteReporteDataTableResponseDTO
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
                    error = "Se produjo un error al procesar la solicitud. " + ex.Message
                });
            }
        }

        // GET: /reportes/endpoint/clientes-reportes/exportar?formato=xlsx
        // GET: /reportes/endpoint/clientes-reportes/exportar?formato=csv
        // GET: /reportes/endpoint/clientes-reportes/exportar?formato=txt
        [HttpGet("exportar")]
        public async Task<IActionResult> Exportar([FromQuery] FiltroClientesReporteViewModel filtros, string formato, string downloadToken)
        {
            IQueryable<Clientes> query = GetBaseQuery();

            query = ApplyFilters(query, filtros);

            if (!string.IsNullOrWhiteSpace(filtros.SearchValue))
            {
                query = ApplyGlobalSearch(query, filtros.SearchValue);
            }

            List<Clientes> datos = await query
                .OrderBy(c => c.FechaIngreso)
                .ToListAsync();

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string fileName = "Clientes_" + timestamp;

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
                        "text/plain",
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

        // Alias para replicar nombres originales del MVC
        // GET: /reportes/endpoint/clientes-reportes/exportar-excel
        [HttpGet("exportar-excel")]
        public async Task<IActionResult> ExportarExcel(string fechaIngresoDesdeFiltro, string fechaIngresoHastaFiltro)
        {
            var filtros = new FiltroClientesReporteViewModel
            {
                FechaIngresoDesdeFiltro = fechaIngresoDesdeFiltro ?? "",
                FechaIngresoHastaFiltro = fechaIngresoHastaFiltro ?? "",
                SearchValue = ""
            };

            IQueryable<Clientes> query = GetBaseQuery();
            query = ApplyFilters(query, filtros);

            List<Clientes> datos = await query
                .OrderBy(c => c.FechaIngreso)
                .ToListAsync();

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            return File(
                GenerateXlsxBytes(datos),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Clientes_" + timestamp + ".xlsx"
            );
        }

        // Alias para replicar nombres originales del MVC
        // GET: /reportes/endpoint/clientes-reportes/exportar-csv
        [HttpGet("exportar-csv")]
        public async Task<IActionResult> ExportarCSV(string fechaIngresoDesdeFiltro, string fechaIngresoHastaFiltro)
        {
            var filtros = new FiltroClientesReporteViewModel
            {
                FechaIngresoDesdeFiltro = fechaIngresoDesdeFiltro ?? "",
                FechaIngresoHastaFiltro = fechaIngresoHastaFiltro ?? "",
                SearchValue = ""
            };

            IQueryable<Clientes> query = GetBaseQuery();
            query = ApplyFilters(query, filtros);

            List<Clientes> datos = await query
                .OrderBy(c => c.FechaIngreso)
                .ToListAsync();

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            return File(
                GenerateCsvBytes(datos),
                "text/plain",
                "Clientes_" + timestamp + ".csv"
            );
        }

        // Alias para replicar nombres originales del MVC
        // GET: /reportes/endpoint/clientes-reportes/exportar-txt
        [HttpGet("exportar-txt")]
        public async Task<IActionResult> ExportarTXT(string fechaIngresoDesdeFiltro, string fechaIngresoHastaFiltro)
        {
            var filtros = new FiltroClientesReporteViewModel
            {
                FechaIngresoDesdeFiltro = fechaIngresoDesdeFiltro ?? "",
                FechaIngresoHastaFiltro = fechaIngresoHastaFiltro ?? "",
                SearchValue = ""
            };

            IQueryable<Clientes> query = GetBaseQuery();
            query = ApplyFilters(query, filtros);

            List<Clientes> datos = await query
                .OrderBy(c => c.FechaIngreso)
                .ToListAsync();

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            return File(
                GenerateTxtBytes(datos),
                "text/plain",
                "Clientes_" + timestamp + ".txt"
            );
        }

        // GET: /reportes/endpoint/clientes-reportes/destinatarios-combo-json?q=texto
        [HttpGet("destinatarios-combo-json")]
        public IActionResult DestinatariosComboJson(string q)
        {
            string texto = q ?? "";

            var items = _context.Usuarios
                .Where(x =>
                    x.Personas != null &&
                    (
                        (x.Personas.NroDocumento ?? "").Contains(texto) ||
                        (x.Personas.NroTarjeta ?? "").Contains(texto)
                    )
                )
                .Select(x => new PersonaComboReporteDTO
                {
                    Text = x.Personas.Apellido + ", " + x.Personas.Nombres,
                    Value = x.Personas.Id,
                    Subtext = x.UserName,
                    Icon = "fa fa-user"
                })
                .Take(10)
                .ToArray();

            return Ok(items);
        }

        private IQueryable<Clientes> GetBaseQuery()
        {
            return _context.Clientes
                .Include(c => c.Persona)
                .Include(c => c.Usuario)
                .AsQueryable();
        }

        private IQueryable<Clientes> ApplyFilters(IQueryable<Clientes> query, FiltroClientesReporteViewModel filtros)
        {
            if (filtros == null)
            {
                return query;
            }

            if (DateTime.TryParse(filtros.FechaIngresoDesdeFiltro, out DateTime fechaIngresoDesde))
            {
                query = query.Where(c =>
                    c.FechaIngreso.Date >= fechaIngresoDesde.Date
                );
            }

            if (DateTime.TryParse(filtros.FechaIngresoHastaFiltro, out DateTime fechaIngresoHasta))
            {
                query = query.Where(c =>
                    c.FechaIngreso.Date <= fechaIngresoHasta.Date
                );
            }

            return query;
        }

        private IQueryable<Clientes> ApplyGlobalSearch(IQueryable<Clientes> query, string searchValue)
        {
            if (string.IsNullOrWhiteSpace(searchValue))
            {
                return query;
            }

            string texto = searchValue.Trim().ToLower();

            query = query.Where(c =>
                (
                    c.Usuario != null &&
                    (c.Usuario.UserName ?? "").ToLower().Contains(texto)
                ) ||
                (
                    c.Persona != null &&
                    (
                        (c.Persona.NroDocumento ?? "").ToLower().Contains(texto) ||
                        (c.Persona.NroTarjeta ?? "").ToLower().Contains(texto) ||
                        ((c.Persona.Apellido ?? "") + " " + (c.Persona.Nombres ?? "")).ToLower().Contains(texto) ||
                        ((c.Persona.Nombres ?? "") + " " + (c.Persona.Apellido ?? "")).ToLower().Contains(texto)
                    )
                )
            );

            return query;
        }

        private IQueryable<Clientes> ApplyOrder(IQueryable<Clientes> query, string sortColumn, string sortDirection)
        {
            bool ascending = (sortDirection ?? "").ToLower() == "asc";
            string column = NormalizeColumnName(sortColumn);

            switch (column)
            {
                case "mail":
                case "usuario":
                case "username":
                    return ascending
                        ? query.OrderBy(c => c.Usuario.UserName)
                        : query.OrderByDescending(c => c.Usuario.UserName);

                case "nrodocumento":
                case "documento":
                case "persona.nrodocumento":
                    return ascending
                        ? query.OrderBy(c => c.Persona.NroDocumento)
                        : query.OrderByDescending(c => c.Persona.NroDocumento);

                case "nombrecompleto":
                case "apellidoynombre":
                case "nombre":
                case "cliente":
                    return ascending
                        ? query.OrderBy(c => c.Persona.Apellido).ThenBy(c => c.Persona.Nombres)
                        : query.OrderByDescending(c => c.Persona.Apellido).ThenByDescending(c => c.Persona.Nombres);

                case "nrotarjeta":
                case "tarjeta":
                case "persona.nrotarjeta":
                    return ascending
                        ? query.OrderBy(c => c.Persona.NroTarjeta)
                        : query.OrderByDescending(c => c.Persona.NroTarjeta);

                case "fechaingreso":
                case "fecha":
                    return ascending
                        ? query.OrderBy(c => c.FechaIngreso)
                        : query.OrderByDescending(c => c.FechaIngreso);

                default:
                    return query.OrderBy(c => c.FechaIngreso);
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

        private ClienteReporteDTO MapClienteReporteDTO(Clientes cliente)
        {
            string mail = "Sin Datos";
            string nroDocumento = "Sin Datos";
            string nombreCompleto = "Sin Datos";
            string nroTarjeta = "Sin Datos";
            int personaId = 0;
            string usuarioId = "";

            if (cliente.Usuario != null)
            {
                usuarioId = cliente.Usuario.Id;
                mail = cliente.Usuario.UserName ?? "Sin Datos";
            }

            if (cliente.Persona != null)
            {
                personaId = cliente.Persona.Id;
                nroDocumento = cliente.Persona.NroDocumento ?? "Sin Datos";
                nroTarjeta = cliente.Persona.NroTarjeta ?? "Sin Datos";

                string apellido = cliente.Persona.Apellido ?? "";
                string nombres = cliente.Persona.Nombres ?? "";
                string nombreArmado = (apellido + " " + nombres).Trim();

                if (!string.IsNullOrWhiteSpace(nombreArmado))
                {
                    nombreCompleto = nombreArmado;
                }
            }

            return new ClienteReporteDTO
            {
                Id = cliente.Id,
                PersonaId = personaId,
                UsuarioId = usuarioId,
                Mail = mail,
                NroDocumento = nroDocumento,
                NombreCompleto = nombreCompleto,
                NroTarjeta = nroTarjeta,
                FechaIngreso = cliente.FechaIngreso,
                FechaIngresoTexto = cliente.FechaIngreso.ToString("dd/MM/yyyy")
            };
        }

        private List<Clientes> FiltrarClientesFunction(string fechaIngresoDesdeFiltro, string fechaIngresoHastaFiltro)
        {
            try
            {
                IQueryable<Clientes> query = GetBaseQuery();

                if (DateTime.TryParse(fechaIngresoDesdeFiltro, out DateTime parsedFechaIngresoDesdeFiltro))
                {
                    query = query.Where(c =>
                        c.FechaIngreso.Date >= parsedFechaIngresoDesdeFiltro.Date
                    );
                }

                if (DateTime.TryParse(fechaIngresoHastaFiltro, out DateTime parsedFechaIngresoHastaFiltro))
                {
                    query = query.Where(c =>
                        c.FechaIngreso.Date <= parsedFechaIngresoHastaFiltro.Date
                    );
                }

                return query.ToList();
            }
            catch
            {
                return new List<Clientes>();
            }
        }

        private byte[] GenerateXlsxBytes(List<Clientes> datos)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Clientes");

                worksheet.Cells[1, 1].Value = "Mail";
                worksheet.Cells[1, 2].Value = "Nro Documento";
                worksheet.Cells[1, 3].Value = "Apellido y Nombre";
                worksheet.Cells[1, 4].Value = "Nro Tarjeta";
                worksheet.Cells[1, 5].Value = "Fecha de Ingreso";

                int linea = 2;

                foreach (Clientes renglon in datos)
                {
                    worksheet.Cells[linea, 1].Value = GetMail(renglon);
                    worksheet.Cells[linea, 2].Value = GetNroDocumento(renglon);
                    worksheet.Cells[linea, 3].Value = GetNombreCompleto(renglon);
                    worksheet.Cells[linea, 4].Value = GetNroTarjeta(renglon);
                    worksheet.Cells[linea, 5].Value = renglon.FechaIngreso.ToString("dd/MM/yyyy");

                    linea++;
                }

                if (worksheet.Dimension != null)
                {
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                }

                return package.GetAsByteArray();
            }
        }

        private byte[] GenerateCsvBytes(List<Clientes> datos)
        {
            var sb = new StringBuilder();

            sb.AppendLine("Mail;Nro Documento;Apellido y Nombre;Nro Tarjeta;Fecha de Ingreso");

            foreach (Clientes renglon in datos)
            {
                string mail = GetMail(renglon);
                string nroDocumento = GetNroDocumento(renglon);
                string apellidoYNombre = GetNombreCompleto(renglon);
                string nroTarjeta = GetNroTarjeta(renglon);
                string fechaIngresoStr = renglon.FechaIngreso.ToString("dd/MM/yyyy");

                sb.AppendLine(
                    EscapeCsvField(mail) + ";" +
                    EscapeCsvField(nroDocumento) + ";" +
                    EscapeCsvField(apellidoYNombre) + ";" +
                    EscapeCsvField(nroTarjeta) + ";" +
                    EscapeCsvField(fechaIngresoStr)
                );
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        private byte[] GenerateTxtBytes(List<Clientes> datos)
        {
            var sb = new StringBuilder();
            char delimitador = '\t';

            sb.AppendLine("Mail" + delimitador + "Nro Documento" + delimitador + "Apellido y Nombre" + delimitador + "Nro Tarjeta" + delimitador + "Fecha de Ingreso");

            foreach (Clientes renglon in datos)
            {
                string mail = GetMail(renglon);
                string nroDocumento = GetNroDocumento(renglon);
                string apellidoYNombre = GetNombreCompleto(renglon);
                string nroTarjeta = GetNroTarjeta(renglon);
                string fechaIngresoStr = renglon.FechaIngreso.ToString("dd/MM/yyyy");

                sb.AppendLine(
                    mail + delimitador +
                    nroDocumento + delimitador +
                    apellidoYNombre + delimitador +
                    nroTarjeta + delimitador +
                    fechaIngresoStr
                );
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        private string GetMail(Clientes cliente)
        {
            if (cliente.Usuario != null && !string.IsNullOrWhiteSpace(cliente.Usuario.UserName))
            {
                return cliente.Usuario.UserName;
            }

            return "Sin Datos";
        }

        private string GetNroDocumento(Clientes cliente)
        {
            if (cliente.Persona != null && !string.IsNullOrWhiteSpace(cliente.Persona.NroDocumento))
            {
                return cliente.Persona.NroDocumento;
            }

            return "Sin Datos";
        }

        private string GetNombreCompleto(Clientes cliente)
        {
            if (cliente.Persona != null)
            {
                string apellido = cliente.Persona.Apellido ?? "";
                string nombres = cliente.Persona.Nombres ?? "";
                string nombreCompleto = (apellido + " " + nombres).Trim();

                if (!string.IsNullOrWhiteSpace(nombreCompleto))
                {
                    return nombreCompleto;
                }
            }

            return "Sin Datos";
        }

        private string GetNroTarjeta(Clientes cliente)
        {
            if (cliente.Persona != null && !string.IsNullOrWhiteSpace(cliente.Persona.NroTarjeta))
            {
                return cliente.Persona.NroTarjeta;
            }

            return "Sin Datos";
        }

        private string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
            {
                return "";
            }

            if (field.Contains("\"") || field.Contains(";") || field.Contains("\n") || field.Contains("\r"))
            {
                return "\"" + field.Replace("\"", "\"\"") + "\"";
            }

            return field;
        }
    }
}