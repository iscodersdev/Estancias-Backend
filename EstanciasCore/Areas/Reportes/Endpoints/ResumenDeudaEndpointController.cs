using DAL.Data;
using DAL.DTOs.Reportes;
using DAL.Mobile;
using DAL.Models;
using EstanciasCore.API.Filters;
using EstanciasCore.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.Areas.Reportes.Endpoints
{
    [Area("Reportes")]
    [TypeFilter(typeof(EndpointUatAuthorizeAttribute))]
    [ApiController]
    [Route("reportes/endpoint/resumen-deuda")]
    public class ResumenDeudaEndpointController : ControllerBase
    {
        private readonly EstanciasContext _context;
        private readonly IDatosTarjetaService _datosServices;
        private readonly ICompositeViewEngine _viewEngine;

        public ResumenDeudaEndpointController(
            EstanciasContext context,
            IDatosTarjetaService datosServices,
            ICompositeViewEngine viewEngine)
        {
            _context = context;
            _datosServices = datosServices;
            _viewEngine = viewEngine;
        }

        // GET: /reportes/endpoint/resumen-deuda
        [HttpGet]
        public IActionResult Index()
        {
            return Ok(new
            {
                status = 200,
                mensaje = "API Reportes - Resumen de Deuda",
                endpoints = new
                {
                    filtros = "/reportes/endpoint/resumen-deuda/filtros",
                    listadoDeuda = "/reportes/endpoint/resumen-deuda/listado-deuda"
                }
            });
        }

        // GET: /reportes/endpoint/resumen-deuda/filtros
        [HttpGet("filtros")]
        public IActionResult Filtros()
        {
            return Ok(new FiltroResumenTarjetaRequestDTO
            {
                NroTarjetaFiltro = "",
                NroDocumentoFiltro = ""
            });
        }

        // POST: /reportes/endpoint/resumen-deuda/listado-deuda
        [HttpPost("listado-deuda")]
        public async Task<IActionResult> ListadoDeuda([FromForm] FiltroResumenTarjetaRequestDTO filtros)
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
                    return Ok(new ResumenDeudaResponseDTO
                    {
                        Status = 200,
                        Mensaje = "",
                        UsuarioId = "",
                        NroDocumento = "",
                        NroTarjeta = "",
                        MontoDisponible = "0",
                        FechaVencimiento = "",
                        MontoPunitoriosTotal = 0,
                        Movimientos = new List<MovimientoResumenDeudaDTO>(),
                        DetallesCuotas = new List<DetalleCuotaConSolicitudDTO>()
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
                    return Ok(new ResumenDeudaResponseDTO
                    {
                        Status = 404,
                        Mensaje = "No se encontró ningún usuario con los datos proporcionados.",
                        UsuarioId = "",
                        NroDocumento = "",
                        NroTarjeta = "",
                        MontoDisponible = "0",
                        FechaVencimiento = "",
                        MontoPunitoriosTotal = 0,
                        Movimientos = new List<MovimientoResumenDeudaDTO>(),
                        DetallesCuotas = new List<DetalleCuotaConSolicitudDTO>()
                    });
                }

                if (usuario.Personas == null)
                {
                    return Ok(new ResumenDeudaResponseDTO
                    {
                        Status = 404,
                        Mensaje = "El usuario encontrado no tiene una persona asociada.",
                        UsuarioId = usuario.Id,
                        NroDocumento = "",
                        NroTarjeta = "",
                        MontoDisponible = "0",
                        FechaVencimiento = "",
                        MontoPunitoriosTotal = 0,
                        Movimientos = new List<MovimientoResumenDeudaDTO>(),
                        DetallesCuotas = new List<DetalleCuotaConSolicitudDTO>()
                    });
                }

                DatosEstructura empresa = await _context.DatosEstructura.FirstOrDefaultAsync();

                if (empresa == null)
                {
                    return StatusCode(500, new ResumenDeudaResponseDTO
                    {
                        Status = 500,
                        Mensaje = "No se encontraron los datos de estructura para consultar el servicio.",
                        UsuarioId = usuario.Id,
                        NroDocumento = usuario.Personas.NroDocumento ?? "",
                        NroTarjeta = usuario.Personas.NroTarjeta ?? "",
                        MontoDisponible = "0",
                        FechaVencimiento = "",
                        MontoPunitoriosTotal = 0,
                        Movimientos = new List<MovimientoResumenDeudaDTO>(),
                        DetallesCuotas = new List<DetalleCuotaConSolicitudDTO>()
                    });
                }

                DateTime fechaActual = DateTime.Now;
                DateTime fechaVencimiento = ObtenerFechaCalculada(fechaActual);

                List<MovimientoResumenDeudaDTO> comprasAgrupadas = new List<MovimientoResumenDeudaDTO>();
                List<DetalleCuotaConSolicitudDTO> detallesCuotas = new List<DetalleCuotaConSolicitudDTO>();

                decimal montoPunitoriosTotal = 0;
                string montoDisponible = "0";

                var datosMovimientos = await _datosServices.ConsultarMovimientos(
                    empresa.UsernameWS.ToLower(),
                    empresa.PasswordWS,
                    usuario.Personas.NroDocumento,
                    Convert.ToInt64(usuario.Personas.NroTarjeta),
                    10,
                    0
                );

                if (datosMovimientos != null &&
                    datosMovimientos.Detalle != null &&
                    datosMovimientos.Detalle.Resultado == "EXITO")
                {
                    montoDisponible = datosMovimientos.Detalle.MontoDisponible ?? "0";

                    comprasAgrupadas = datosMovimientos.Movimientos
                        .Where(x => x.Descripcion == "PAGOS DE CUOTA REGULAR")
                        .GroupBy(m => new { m.Descripcion, m.Fecha })
                        .Select(g =>
                        {
                            decimal monto = g.Sum(m => ParseDecimalServicio(m.Monto) + ParseDecimalServicio(m.Recargo));

                            return new MovimientoResumenDeudaDTO
                            {
                                Fecha = g.Key.Fecha.Date.ToString("dd/MM/yyyy"),
                                TipoMovimiento = g.Key.Descripcion ?? "",
                                Monto = FormatDecimalServicio(monto)
                            };
                        })
                        .ToList();

                    comprasAgrupadas.AddRange(datosMovimientos.Movimientos
                        .Where(x => x.Descripcion != "PAGOS DE CUOTA REGULAR")
                        .Select(g => new MovimientoResumenDeudaDTO
                        {
                            Fecha = g.Fecha.Date.ToString("dd/MM/yyyy"),
                            TipoMovimiento = g.Descripcion ?? "",
                            Monto = FormatDecimalServicio(ParseDecimalServicio(g.Monto))
                        })
                        .ToList());

                    detallesCuotas = datosMovimientos.DetallesSolicitud
                        .Where(result => result != null && result.DetallesCuota != null)
                        .SelectMany(
                            result => result.DetallesCuota,
                            (result, detalle) => new DetalleCuotaConSolicitudDTO
                            {
                                NroSolicitud = result.NumeroSolicitud ?? "",
                                NroCuota = detalle.NumeroCuota ?? "",
                                Monto = detalle.Monto ?? "",
                                Fecha = detalle.Fecha ?? ""
                            })
                        .Where(x => ConvertirFechaServicio(x.Fecha) <= fechaVencimiento)
                        .ToList();

                    montoPunitoriosTotal = await _datosServices.CalcularPunitorios(datosMovimientos.DetallesSolicitud);
                }

                return Ok(new ResumenDeudaResponseDTO
                {
                    Status = 200,
                    Mensaje = "",
                    UsuarioId = usuario.Id,
                    NroDocumento = usuario.Personas.NroDocumento ?? "",
                    NroTarjeta = usuario.Personas.NroTarjeta ?? "",
                    MontoDisponible = montoDisponible,
                    FechaVencimiento = fechaVencimiento.ToString("dd/MM/yyyy"),
                    MontoPunitoriosTotal = montoPunitoriosTotal,
                    Movimientos = comprasAgrupadas,
                    DetallesCuotas = detallesCuotas
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResumenDeudaResponseDTO
                {
                    Status = 500,
                    Mensaje = "Se produjo un error al procesar la solicitud: " + ex.Message,
                    UsuarioId = "",
                    NroDocumento = "",
                    NroTarjeta = "",
                    MontoDisponible = "0",
                    FechaVencimiento = "",
                    MontoPunitoriosTotal = 0,
                    Movimientos = new List<MovimientoResumenDeudaDTO>(),
                    DetallesCuotas = new List<DetalleCuotaConSolicitudDTO>()
                });
            }
        }

        private DateTime ObtenerFechaCalculada(DateTime fechaActual)
        {
            return fechaActual;
        }

        private DateTime ConvertirFechaServicio(string fecha)
        {
            if (string.IsNullOrWhiteSpace(fecha))
            {
                return DateTime.MinValue;
            }

            DateTime resultado;

            if (DateTime.TryParseExact(fecha, "dd/MM/yyyy", CultureInfo.GetCultureInfo("es-AR"), DateTimeStyles.None, out resultado))
            {
                return resultado;
            }

            if (DateTime.TryParse(fecha, out resultado))
            {
                return resultado;
            }

            return DateTime.MinValue;
        }

        private decimal ParseDecimalServicio(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return 0;
            }

            string texto = value.Trim()
                .Replace("$", "")
                .Replace(" ", "");

            decimal result;

            if (decimal.TryParse(texto, NumberStyles.Any, CultureInfo.GetCultureInfo("es-AR"), out result))
            {
                return result;
            }

            string normalizado = texto.Replace(",", ".");

            if (decimal.TryParse(normalizado, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                return result;
            }

            return 0;
        }

        private string FormatDecimalServicio(decimal value)
        {
            return value.ToString("0.00", CultureInfo.GetCultureInfo("es-AR"));
        }
    }
}