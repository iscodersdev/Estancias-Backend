using DAL.Data;
using DAL.DTOs;
using EstanciasCore.API.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.Endpoints
{
    [TypeFilter(typeof(EndpointUatAuthorizeAttribute))]
    [Route("endpoint/validacion-cupones")]
    [ApiController]
    public class ValidacionCuponesEndpointController : ControllerBase
    {
        private readonly EstanciasContext _context;

        public ValidacionCuponesEndpointController(EstanciasContext context)
        {
            _context = context;
        }

        // GET: endpoint/validacion-cupones/historial-reciente
        [HttpGet("historial-reciente")]
        public async Task<IActionResult> HistorialReciente()
        {
            try
            {
                var historial = await _context.HistorialCanje
                    .Include(x => x.Premio)
                    .Include(x => x.Cliente)
                    .Include(x => x.Cliente.Persona)
                    .Where(x => !x.Activo)
                    .OrderByDescending(x => x.Fecha)
                    .Take(50)
                    .ToListAsync();

                var data = historial.Select(x => MapearCupon(x)).ToList();

                return Ok(new ValidacionCuponListadoResponseDTO
                {
                    Ok = true,
                    Status = 200,
                    Mensaje = "Historial reciente de canjes obtenido correctamente.",
                    EsBusqueda = false,
                    Cupones = data
                });
            }
            catch
            {
                return StatusCode(500, new ValidacionCuponListadoResponseDTO
                {
                    Ok = false,
                    Status = 500,
                    Mensaje = "Ocurrió un error al obtener el historial reciente.",
                    EsBusqueda = false
                });
            }
        }

        // GET: endpoint/validacion-cupones/buscar?codigo=ABCD-1234
        // También sirve para DNI:
        // GET: endpoint/validacion-cupones/buscar?codigo=12345678
        [HttpGet("buscar")]
        public async Task<IActionResult> Buscar([FromQuery] string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                return BadRequest(new ValidacionCuponListadoResponseDTO
                {
                    Ok = false,
                    Status = 400,
                    Mensaje = "Debe ingresar un código de cupón o DNI.",
                    EsBusqueda = true
                });
            }

            codigo = codigo.Trim().ToUpper();

            try
            {
                var cupones = await _context.HistorialCanje
                    .Include(x => x.Premio)
                    .Include(x => x.Cliente)
                    .Include(x => x.Cliente.Persona)
                    .Where(x =>
                        x.CodigoCupon == codigo ||
                        x.Cliente.Persona.NroDocumento == codigo
                    )
                    .OrderBy(x => x.FechaVencimientoCupon)
                    .ToListAsync();

                if (!cupones.Any())
                {
                    return NotFound(new ValidacionCuponListadoResponseDTO
                    {
                        Ok = false,
                        Status = 404,
                        Mensaje = "No existe el cupón ingresado o el cliente no tiene cupones registrados.",
                        EsBusqueda = true
                    });
                }

                var cuponesDisponibles = cupones
                    .Where(x => x.Activo && x.FechaVencimientoCupon.Date >= DateTime.Now.Date)
                    .ToList();

                var cuponesCanjeados = cupones
                    .Where(x => !x.Activo)
                    .ToList();

                string mensaje;

                if (cuponesDisponibles.Any())
                {
                    if (cuponesDisponibles.Count == 1 && cuponesDisponibles.First().CodigoCupon == codigo)
                    {
                        mensaje = $"Se encontró el cupón {codigo}.";
                    }
                    else
                    {
                        mensaje = $"Se encontraron {cuponesDisponibles.Count} cupon(es) disponible(s).";
                    }
                }
                else if (cuponesCanjeados.Any() && cuponesCanjeados.Any(c => c.CodigoCupon == codigo))
                {
                    var cuponCanjeado = cuponesCanjeados.First(c => c.CodigoCupon == codigo);
                    mensaje = $"El cupón {codigo} ya fue canjeado el {cuponCanjeado.Fecha:dd/MM/yyyy}.";
                }
                else if (cuponesCanjeados.Any())
                {
                    mensaje = "No hay cupones disponibles, pero el cliente tiene cupones canjeados en su historial.";
                }
                else
                {
                    mensaje = "Se encontraron cupones, pero se encuentran vencidos.";
                }

                var data = cupones.Select(x => MapearCupon(x)).ToList();

                return Ok(new ValidacionCuponListadoResponseDTO
                {
                    Ok = true,
                    Status = 200,
                    Mensaje = mensaje,
                    EsBusqueda = true,
                    Cupones = data
                });
            }
            catch
            {
                return StatusCode(500, new ValidacionCuponListadoResponseDTO
                {
                    Ok = false,
                    Status = 500,
                    Mensaje = "Ocurrió un error al buscar.",
                    EsBusqueda = true
                });
            }
        }

        // GET: endpoint/validacion-cupones/buscar-por-cupon/ABCD-1234
        [HttpGet("buscar-por-cupon/{codigo}")]
        public async Task<IActionResult> BuscarPorCupon(string codigo)
        {
            return await Buscar(codigo);
        }

        // GET: endpoint/validacion-cupones/buscar-por-dni/12345678
        [HttpGet("buscar-por-dni/{dni}")]
        public async Task<IActionResult> BuscarPorDni(string dni)
        {
            return await Buscar(dni);
        }

        // POST: endpoint/validacion-cupones/validar
        [HttpPost("validar")]
        public async Task<IActionResult> Validar([FromBody] ValidarCuponRequestDTO request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Codigo))
            {
                return BadRequest(new ValidarCuponResponseDTO
                {
                    Ok = false,
                    Status = 400,
                    Mensaje = "Debe ingresar un código de cupón."
                });
            }

            var codigo = request.Codigo.Trim().ToUpper();

            try
            {
                var cupon = await _context.HistorialCanje
                    .Include(x => x.Premio)
                    .Include(x => x.Cliente)
                    .Include(x => x.Cliente.Persona)
                    .FirstOrDefaultAsync(x => x.CodigoCupon == codigo);

                if (cupon == null)
                {
                    return NotFound(new ValidarCuponResponseDTO
                    {
                        Ok = false,
                        Status = 404,
                        Mensaje = "No existe el cupón ingresado."
                    });
                }

                if (!cupon.Activo)
                {
                    return BadRequest(new ValidarCuponResponseDTO
                    {
                        Ok = false,
                        Status = 400,
                        Mensaje = "El cupón ya fue utilizado o ha sido anulado.",
                        Cupon = MapearCupon(cupon)
                    });
                }

                if (cupon.FechaVencimientoCupon.Date < DateTime.Now.Date)
                {
                    return BadRequest(new ValidarCuponResponseDTO
                    {
                        Ok = false,
                        Status = 400,
                        Mensaje = "El cupón ingresado se encuentra vencido.",
                        Cupon = MapearCupon(cupon)
                    });
                }

                cupon.Activo = false;
                cupon.Fecha = DateTime.Now;

                _context.HistorialCanje.Update(cupon);
                await _context.SaveChangesAsync();

                return Ok(new ValidarCuponResponseDTO
                {
                    Ok = true,
                    Status = 200,
                    Mensaje = $"Cupón validado con éxito. Premio: {cupon.Premio?.Nombre}",
                    Cupon = MapearCupon(cupon)
                });
            }
            catch
            {
                return StatusCode(500, new ValidarCuponResponseDTO
                {
                    Ok = false,
                    Status = 500,
                    Mensaje = "Ocurrió un error al intentar validar el cupón."
                });
            }
        }

        private static ValidacionCuponDetalleDTO MapearCupon(dynamic x)
        {
            var vencido = x.FechaVencimientoCupon.Date < DateTime.Now.Date;

            string estado;

            if (!x.Activo)
            {
                estado = "Canjeado";
            }
            else if (vencido)
            {
                estado = "Vencido";
            }
            else
            {
                estado = "Disponible";
            }

            var apellido = x.Cliente?.Persona?.Apellido ?? "";
            var nombres = x.Cliente?.Persona?.Nombres ?? "";
            var cliente = $"{apellido}, {nombres}".Trim();

            if (cliente == ",")
            {
                cliente = "";
            }

            return new ValidacionCuponDetalleDTO
            {
                Id = x.Id,
                Cliente = cliente,
                NroDocumento = x.Cliente?.Persona?.NroDocumento,
                Premio = x.Premio?.Nombre,
                CodigoCupon = x.CodigoCupon,
                Fecha = x.Fecha,
                FechaVencimientoCupon = x.FechaVencimientoCupon,
                Activo = x.Activo,
                Vencido = vencido,
                Estado = estado
            };
        }
    }
}