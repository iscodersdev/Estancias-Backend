using DAL.Data;
using DAL.DTOs.Reportes;
using DAL.Models;
using EstanciasCore.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.Areas.Reportes.Endpoints
{
    [Area("Reportes")]
    [ApiController]
    [Route("reportes/endpoint/resumen-tarjeta-reportes")]
    public class ResumenTarjetaReportesEndpointController : ControllerBase
    {
        private readonly EstanciasContext _context;
        private readonly IDatosTarjetaService _datosServices;
        private readonly ICompositeViewEngine _viewEngine;

        public ResumenTarjetaReportesEndpointController(
            EstanciasContext context,
            IDatosTarjetaService datosServices,
            ICompositeViewEngine viewEngine)
        {
            _context = context;
            _datosServices = datosServices;
            _viewEngine = viewEngine;
        }

        // GET: /reportes/endpoint/resumen-tarjeta-reportes
        [HttpGet]
        public IActionResult Index()
        {
            return Ok(new
            {
                status = 200,
                mensaje = "API Reportes - Resumen de Tarjeta",
                endpoints = new
                {
                    filtros = "/reportes/endpoint/resumen-tarjeta-reportes/filtros",
                    listadoResumenes = "/reportes/endpoint/resumen-tarjeta-reportes/listado-resumenes",
                    descargarResumen = "/reportes/endpoint/resumen-tarjeta-reportes/descargar-resumen?Id=PeriodoId,UsuarioId",
                    descargarResumenArchivo = "/reportes/endpoint/resumen-tarjeta-reportes/descargar-resumen-archivo?Id=PeriodoId,UsuarioId"
                }
            });
        }

        // GET: /reportes/endpoint/resumen-tarjeta-reportes/filtros
        [HttpGet("filtros")]
        public IActionResult Filtros()
        {
            return Ok(new FiltroResumenTarjetaRequestDTO
            {
                NroTarjetaFiltro = "",
                NroDocumentoFiltro = ""
            });
        }

        // POST: /reportes/endpoint/resumen-tarjeta-reportes/listado-resumenes
        [HttpPost("listado-resumenes")]
        public async Task<IActionResult> ListadoResumenes([FromForm] FiltroResumenTarjetaRequestDTO filtros)
        {
            try
            {
                if (filtros == null)
                {
                    filtros = new FiltroResumenTarjetaRequestDTO();
                }

                string nroTarjetaFiltro = filtros.NroTarjetaFiltro ?? "";
                string nroDocumentoFiltro = filtros.NroDocumentoFiltro ?? "";

                if (string.IsNullOrWhiteSpace(nroTarjetaFiltro) && string.IsNullOrWhiteSpace(nroDocumentoFiltro))
                {
                    return Ok(new ResumenTarjetaListadoResponseDTO
                    {
                        Status = 200,
                        Mensaje = "",
                        UsuarioId = "",
                        Data = new List<ResumenTarjetaDTO>()
                    });
                }

                IQueryable<Usuario> query = _context.Usuarios
                    .Include(x => x.Personas)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(nroTarjetaFiltro))
                {
                    query = query.Where(x =>
                        x.Personas != null &&
                        x.Personas.NroTarjeta == nroTarjetaFiltro
                    );
                }

                if (!string.IsNullOrWhiteSpace(nroDocumentoFiltro))
                {
                    query = query.Where(x =>
                        x.Personas != null &&
                        x.Personas.NroDocumento == nroDocumentoFiltro
                    );
                }

                Usuario usuario = await query.FirstOrDefaultAsync();

                if (usuario == null)
                {
                    return Ok(new ResumenTarjetaListadoResponseDTO
                    {
                        Status = 404,
                        Mensaje = "No se encontró ningún usuario con los datos proporcionados.",
                        UsuarioId = "",
                        Data = new List<ResumenTarjetaDTO>()
                    });
                }

                DateTime fechaActual = DateTime.Now.AddMonths(1);

                List<ResumenTarjeta> movimientos = await _context.ResumenTarjeta
                    .Include(x => x.Usuario)
                    .Include(x => x.Periodo)
                    .Where(x =>
                        x.Usuario.Id == usuario.Id &&
                        x.Periodo != null &&
                        x.Periodo.FechaHasta < fechaActual
                    )
                    .ToListAsync();

                List<ResumenTarjetaDTO> resumenes = movimientos
                    .GroupBy(g => g.Periodo)
                    .Select(g =>
                    {
                        decimal monto = g.Sum(m => m.Monto);
                        decimal punitorios = g.Sum(m => m.MontoAdeudado);
                        decimal montoTotal = monto + punitorios;

                        int periodoId = g.Key != null ? g.Key.Id : 0;
                        string usuarioId = usuario.Id;

                        return new ResumenTarjetaDTO
                        {
                            Id = periodoId,
                            NroTarjeta = usuario.Personas != null ? usuario.Personas.NroTarjeta ?? "" : "",
                            UsuarioId = usuarioId,
                            PeriodoId = periodoId,
                            Periodo = g.Key != null ? g.Key.Descripcion ?? "" : "",
                            FechaVencimiento = g.Key != null ? g.Key.FechaVencimiento.ToString("dd/MM/yyyy") : "",
                            Monto = monto,
                            Punitorios = punitorios,
                            MontoTotal = montoTotal,
                            MontoTexto = monto.ToString("C2"),
                            PunitoriosTexto = punitorios.ToString("C2"),
                            MontoTotalTexto = montoTotal.ToString("C2"),
                            DescargarResumenUrl = $"/reportes/endpoint/resumen-tarjeta-reportes/descargar-resumen?Id={periodoId},{usuarioId}",
                            DescargarResumenArchivoUrl = $"/reportes/endpoint/resumen-tarjeta-reportes/descargar-resumen-archivo?Id={periodoId},{usuarioId}",
                            Accion = $"<a href=\"/reportes/endpoint/resumen-tarjeta-reportes/descargar-resumen?Id={periodoId},{usuarioId}\" class=\"btn btn-warning btn-xs\" target=\"_blank\"><i class=\"fa fa-file-pdf-o\"></i></a>"
                        };
                    })
                    .OrderByDescending(r => r.FechaVencimiento)
                    .ToList();

                return Ok(new ResumenTarjetaListadoResponseDTO
                {
                    Status = 200,
                    Mensaje = "",
                    UsuarioId = usuario.Id,
                    Data = resumenes
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResumenTarjetaListadoResponseDTO
                {
                    Status = 500,
                    Mensaje = "Se produjo un error al procesar la solicitud: " + ex.Message,
                    UsuarioId = "",
                    Data = new List<ResumenTarjetaDTO>()
                });
            }
        }

        // GET: /reportes/endpoint/resumen-tarjeta-reportes/descargar-resumen?Id=PeriodoId,UsuarioId
        // Replica el comportamiento original que devolvía la partial _VerResumen con el base64.
        // En API devuelve JSON con el base64.
        [HttpGet("descargar-resumen")]
        public async Task<IActionResult> DescargarResumen(string Id)
        {
            if (string.IsNullOrWhiteSpace(Id))
            {
                return BadRequest(new ResumenArchivoDTO
                {
                    Status = 400,
                    Mensaje = "El ID no puede ser nulo o vacío.",
                    PeriodoId = 0,
                    UsuarioId = "",
                    Base64 = "",
                    ContentType = "",
                    FileName = ""
                });
            }

            string[] partes = Id.Split(',');

            if (partes.Length != 2)
            {
                return BadRequest(new ResumenArchivoDTO
                {
                    Status = 400,
                    Mensaje = "El formato del ID es incorrecto. Se esperaba 'PeriodoId,UsuarioId'.",
                    PeriodoId = 0,
                    UsuarioId = "",
                    Base64 = "",
                    ContentType = "",
                    FileName = ""
                });
            }

            if (!int.TryParse(partes[0], out int periodoId))
            {
                return BadRequest(new ResumenArchivoDTO
                {
                    Status = 400,
                    Mensaje = "El PeriodoId proporcionado no es un número válido.",
                    PeriodoId = 0,
                    UsuarioId = "",
                    Base64 = "",
                    ContentType = "",
                    FileName = ""
                });
            }

            string usuarioId = partes[1];

            try
            {
                ResumenTarjeta resumen = await _context.ResumenTarjeta
                    .FirstOrDefaultAsync(x => x.UsuarioId == usuarioId && x.PeriodoId == periodoId);

                if (resumen == null)
                {
                    return NotFound(new ResumenArchivoDTO
                    {
                        Status = 404,
                        Mensaje = "No se encontró un resumen para el período y usuario especificados.",
                        PeriodoId = periodoId,
                        UsuarioId = usuarioId,
                        Base64 = "",
                        ContentType = "",
                        FileName = ""
                    });
                }

                if (resumen.Adjunto == null || resumen.Adjunto.Length == 0)
                {
                    return NotFound(new ResumenArchivoDTO
                    {
                        Status = 404,
                        Mensaje = "El resumen fue encontrado pero no contiene un archivo adjunto.",
                        PeriodoId = periodoId,
                        UsuarioId = usuarioId,
                        Base64 = "",
                        ContentType = "",
                        FileName = ""
                    });
                }

                string base64String = Convert.ToBase64String(resumen.Adjunto);

                return Ok(new ResumenArchivoDTO
                {
                    Status = 200,
                    Mensaje = "",
                    PeriodoId = periodoId,
                    UsuarioId = usuarioId,
                    Base64 = base64String,
                    ContentType = "application/pdf",
                    FileName = $"ResumenTarjeta_{periodoId}_{usuarioId}.pdf"
                });
            }
            catch
            {
                return StatusCode(500, new ResumenArchivoDTO
                {
                    Status = 500,
                    Mensaje = "Ocurrió un error interno al procesar la solicitud.",
                    PeriodoId = periodoId,
                    UsuarioId = usuarioId,
                    Base64 = "",
                    ContentType = "",
                    FileName = ""
                });
            }
        }

        // GET: /reportes/endpoint/resumen-tarjeta-reportes/descargar-resumen-archivo?Id=PeriodoId,UsuarioId
        // Endpoint extra para descargar directamente el archivo.
        [HttpGet("descargar-resumen-archivo")]
        public async Task<IActionResult> DescargarResumenArchivo(string Id)
        {
            if (string.IsNullOrWhiteSpace(Id))
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

            ResumenTarjeta resumen = await _context.ResumenTarjeta
                .FirstOrDefaultAsync(x => x.UsuarioId == usuarioId && x.PeriodoId == periodoId);

            if (resumen == null)
            {
                return NotFound("No se encontró un resumen para el período y usuario especificados.");
            }

            if (resumen.Adjunto == null || resumen.Adjunto.Length == 0)
            {
                return NotFound("El resumen fue encontrado pero no contiene un archivo adjunto.");
            }

            return File(
                resumen.Adjunto,
                "application/pdf",
                $"ResumenTarjeta_{periodoId}_{usuarioId}.pdf"
            );
        }
    }
}