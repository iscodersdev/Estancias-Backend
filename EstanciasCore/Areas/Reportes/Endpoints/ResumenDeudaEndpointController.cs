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

        private class UsuarioResumenDeudaConsultaDTO
        {
            public string UsuarioId { get; set; }
            public string NroDocumento { get; set; }
            public string NroTarjeta { get; set; }
        }

        // GET: /reportes/endpoint/resumen-deuda/filtros
        [HttpGet("filtros")]
        public IActionResult Filtros()
        {
            return Ok(new FiltroResumenTarjetaRequestDTO
            {
                NroTarjetaFiltro = "",
                NroDocumentoFiltro = "",
                Pagina = 1,
                Cantidad = 50,
                Buscar = ""
            });
        }

        private ResumenDeudaResponseDTO CrearResumenVacio(
            int status,
            string mensaje,
            string usuarioId = "",
            string nroDocumento = "",
            string nroTarjeta = "")
        {
            return new ResumenDeudaResponseDTO
            {
                Status = status,
                Mensaje = mensaje,
                UsuarioId = usuarioId ?? "",
                NroDocumento = nroDocumento ?? "",
                NroTarjeta = nroTarjeta ?? "",
                MontoDisponible = "0",
                FechaVencimiento = "",
                MontoPunitoriosTotal = 0,
                Movimientos = new List<MovimientoResumenDeudaDTO>(),
                DetallesCuotas = new List<DetalleCuotaConSolicitudDTO>()
            };
        }

        private async Task<ResumenDeudaResponseDTO> ObtenerResumenDeudaPorDatosUsuario(
            string usuarioId,
            string nroDocumento,
            string nroTarjeta,
            DatosEstructura empresa)
        {
            try
            {
                usuarioId = usuarioId ?? "";
                nroDocumento = nroDocumento ?? "";
                nroTarjeta = nroTarjeta ?? "";

                if (string.IsNullOrWhiteSpace(nroDocumento) || string.IsNullOrWhiteSpace(nroTarjeta))
                {
                    return CrearResumenVacio(
                        400,
                        "El usuario no tiene número de documento o número de tarjeta.",
                        usuarioId,
                        nroDocumento,
                        nroTarjeta);
                }

                if (!long.TryParse(nroTarjeta, out long nroTarjetaLong))
                {
                    return CrearResumenVacio(
                        400,
                        "El número de tarjeta no tiene un formato válido.",
                        usuarioId,
                        nroDocumento,
                        nroTarjeta);
                }

                DateTime fechaActual = DateTime.Now;
                DateTime fechaVencimiento = ObtenerFechaCalculada(fechaActual);

                List<MovimientoResumenDeudaDTO> comprasAgrupadas = new List<MovimientoResumenDeudaDTO>();
                List<DetalleCuotaConSolicitudDTO> detallesCuotas = new List<DetalleCuotaConSolicitudDTO>();

                decimal montoPunitoriosTotal = 0;
                string montoDisponible = "0";

                var datosMovimientos = await _datosServices.ConsultarMovimientos(
                    empresa.UsernameWS != null ? empresa.UsernameWS.ToLower() : "",
                    empresa.PasswordWS,
                    nroDocumento,
                    nroTarjetaLong,
                    10,
                    0
                );

                if (datosMovimientos != null &&
                    datosMovimientos.Detalle != null &&
                    datosMovimientos.Detalle.Resultado == "EXITO")
                {
                    montoDisponible = datosMovimientos.Detalle.MontoDisponible ?? "0";

                    if (datosMovimientos.Movimientos != null)
                    {
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
                    }

                    if (datosMovimientos.DetallesSolicitud != null)
                    {
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
                }

                return new ResumenDeudaResponseDTO
                {
                    Status = 200,
                    Mensaje = "",
                    UsuarioId = usuarioId,
                    NroDocumento = nroDocumento,
                    NroTarjeta = nroTarjeta,
                    MontoDisponible = montoDisponible,
                    FechaVencimiento = fechaVencimiento.ToString("dd/MM/yyyy"),
                    MontoPunitoriosTotal = montoPunitoriosTotal,
                    Movimientos = comprasAgrupadas,
                    DetallesCuotas = detallesCuotas
                };
            }
            catch (Exception ex)
            {
                return CrearResumenVacio(
                    500,
                    "Se produjo un error al procesar el usuario: " + ex.Message,
                    usuarioId,
                    nroDocumento,
                    nroTarjeta);
            }
        }

        // GET: /reportes/endpoint/resumen-deuda/todos?pagina=1&cantidad=50&buscar=123
        [HttpGet("todos")]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pagina = 1,
            [FromQuery] int cantidad = 50,
            [FromQuery] string buscar = "")
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

                if (cantidad > 200)
                {
                    cantidad = 200;
                }

                buscar = buscar ?? "";

                DatosEstructura empresa = await _context.DatosEstructura
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (empresa == null)
                {
                    return StatusCode(500, new ResumenDeudaListadoResponseDTO
                    {
                        Status = 500,
                        Mensaje = "No se encontraron los datos de estructura para consultar el servicio.",
                        TotalRegistros = 0,
                        PaginaActual = pagina,
                        CantidadPorPagina = cantidad,
                        TotalPaginas = 0,
                        Data = new List<ResumenDeudaResponseDTO>()
                    });
                }

                IQueryable<Usuario> query = _context.Usuarios
                    .AsNoTracking()
                    .Where(x =>
                        x.Personas != null &&
                        x.Personas.NroDocumento != null &&
                        x.Personas.NroDocumento != "" &&
                        x.Personas.NroTarjeta != null &&
                        x.Personas.NroTarjeta != ""
                    );

                if (!string.IsNullOrWhiteSpace(buscar))
                {
                    query = query.Where(x =>
                        x.Personas.NroDocumento.Contains(buscar) ||
                        x.Personas.NroTarjeta.Contains(buscar)
                    );
                }

                int totalRegistros = await query.CountAsync();
                int totalPaginas = (int)Math.Ceiling(totalRegistros / (double)cantidad);

                List<UsuarioResumenDeudaConsultaDTO> usuarios = await query
                    .OrderBy(x => x.Personas.NroDocumento)
                    .Skip((pagina - 1) * cantidad)
                    .Take(cantidad)
                    .Select(x => new UsuarioResumenDeudaConsultaDTO
                    {
                        UsuarioId = x.Id,
                        NroDocumento = x.Personas.NroDocumento,
                        NroTarjeta = x.Personas.NroTarjeta
                    })
                    .ToListAsync();

                List<ResumenDeudaResponseDTO> data = new List<ResumenDeudaResponseDTO>();

                foreach (UsuarioResumenDeudaConsultaDTO usuario in usuarios)
                {
                    ResumenDeudaResponseDTO resumen = await ObtenerResumenDeudaPorDatosUsuario(
                        usuario.UsuarioId,
                        usuario.NroDocumento,
                        usuario.NroTarjeta,
                        empresa);

                    data.Add(resumen);
                }

                return Ok(new ResumenDeudaListadoResponseDTO
                {
                    Status = 200,
                    Mensaje = "Listado de resumen de deuda obtenido correctamente.",
                    TotalRegistros = totalRegistros,
                    PaginaActual = pagina,
                    CantidadPorPagina = cantidad,
                    TotalPaginas = totalPaginas,
                    Data = data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResumenDeudaListadoResponseDTO
                {
                    Status = 500,
                    Mensaje = "Se produjo un error al obtener el listado de resumen de deuda: " + ex.Message,
                    TotalRegistros = 0,
                    PaginaActual = pagina,
                    CantidadPorPagina = cantidad,
                    TotalPaginas = 0,
                    Data = new List<ResumenDeudaResponseDTO>()
                });
            }
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
                    return Ok(CrearResumenVacio(200, ""));
                }

                IQueryable<Usuario> query = _context.Usuarios
                    .AsNoTracking()
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

                UsuarioResumenDeudaConsultaDTO usuario = await query
                    .Select(x => new UsuarioResumenDeudaConsultaDTO
                    {
                        UsuarioId = x.Id,
                        NroDocumento = x.Personas != null ? x.Personas.NroDocumento : "",
                        NroTarjeta = x.Personas != null ? x.Personas.NroTarjeta : ""
                    })
                    .FirstOrDefaultAsync();

                if (usuario == null)
                {
                    return Ok(CrearResumenVacio(
                        404,
                        "No se encontró ningún usuario con los datos proporcionados."));
                }

                if (string.IsNullOrWhiteSpace(usuario.NroDocumento) || string.IsNullOrWhiteSpace(usuario.NroTarjeta))
                {
                    return Ok(CrearResumenVacio(
                        404,
                        "El usuario encontrado no tiene una persona asociada.",
                        usuario.UsuarioId));
                }

                DatosEstructura empresa = await _context.DatosEstructura
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (empresa == null)
                {
                    return StatusCode(500, CrearResumenVacio(
                        500,
                        "No se encontraron los datos de estructura para consultar el servicio.",
                        usuario.UsuarioId,
                        usuario.NroDocumento,
                        usuario.NroTarjeta));
                }

                ResumenDeudaResponseDTO resumen = await ObtenerResumenDeudaPorDatosUsuario(
                    usuario.UsuarioId,
                    usuario.NroDocumento,
                    usuario.NroTarjeta,
                    empresa);

                return Ok(resumen);
            }
            catch (Exception ex)
            {
                return StatusCode(500, CrearResumenVacio(
                    500,
                    "Se produjo un error al procesar la solicitud: " + ex.Message));
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
