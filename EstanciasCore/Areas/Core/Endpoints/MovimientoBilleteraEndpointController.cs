using DAL.Data;
using DAL.DTOs;
using DAL.Models.Core;
using EstanciasCore.API.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.Endpoints
{
    [TypeFilter(typeof(EndpointUatAuthorizeAttribute))]
    [Route("endpoint/movimientos-billetera")]
    [ApiController]
    public class MovimientoBilleteraEndpointController : ControllerBase
    {
        private readonly EstanciasContext _context;

        public MovimientoBilleteraEndpointController(EstanciasContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var movimientos = await _context.MovimientosBilletera
                .Select(m => new MovimientoBilleteraDTO
                {
                    Id = m.Id,
                    Fecha = m.Fecha,
                    TipoMovimientoId = m.TipoMovimiento != null ? m.TipoMovimiento.Id : 0,
                    TipoMovimientoNombre = m.TipoMovimiento != null ? m.TipoMovimiento.Nombre : null,
                    Monto = m.Monto,
                    QR = m.QR,
                    CBU = m.CBU
                })
                .ToListAsync();

            return Ok(new
            {
                ok = true,
                data = movimientos
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var movimiento = await _context.MovimientosBilletera
                .Where(m => m.Id == id)
                .Select(m => new MovimientoBilleteraDTO
                {
                    Id = m.Id,
                    Fecha = m.Fecha,
                    TipoMovimientoId = m.TipoMovimiento != null ? m.TipoMovimiento.Id : 0,
                    TipoMovimientoNombre = m.TipoMovimiento != null ? m.TipoMovimiento.Nombre : null,
                    Monto = m.Monto,
                    QR = m.QR,
                    CBU = m.CBU
                })
                .FirstOrDefaultAsync();

            if (movimiento == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el movimiento de billetera."
                });
            }

            return Ok(new
            {
                ok = true,
                data = movimiento
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MovimientoBilleteraCreateRequest request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Los datos del movimiento de billetera son obligatorios."
                });
            }

            if (request.TipoMovimientoId == 0)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Debe seleccionar un tipo de movimiento."
                });
            }

            if (request.Monto <= 0)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "El monto debe ser mayor a cero."
                });
            }

            var tipoMovimiento = await _context.TipoMovimientoBilletera
                .FirstOrDefaultAsync(t => t.Id == request.TipoMovimientoId);

            if (tipoMovimiento == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el tipo de movimiento seleccionado."
                });
            }

            var movimiento = new MovimientoBilletera
            {
                Fecha = request.Fecha,
                TipoMovimiento = tipoMovimiento,
                Monto = request.Monto,
                QR = request.QR,
                CBU = request.CBU
            };

            await _context.MovimientosBilletera.AddAsync(movimiento);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Movimiento de billetera creado correctamente.",
                data = new MovimientoBilleteraDTO
                {
                    Id = movimiento.Id,
                    Fecha = movimiento.Fecha,
                    TipoMovimientoId = tipoMovimiento.Id,
                    TipoMovimientoNombre = tipoMovimiento.Nombre,
                    Monto = movimiento.Monto,
                    QR = movimiento.QR,
                    CBU = movimiento.CBU
                }
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] MovimientoBilleteraUpdateRequest request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Los datos del movimiento de billetera son obligatorios."
                });
            }

            if (request.TipoMovimientoId == 0)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Debe seleccionar un tipo de movimiento."
                });
            }

            if (request.Monto <= 0)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "El monto debe ser mayor a cero."
                });
            }

            var movimiento = await _context.MovimientosBilletera
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movimiento == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el movimiento de billetera."
                });
            }

            var tipoMovimiento = await _context.TipoMovimientoBilletera
                .FirstOrDefaultAsync(t => t.Id == request.TipoMovimientoId);

            if (tipoMovimiento == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el tipo de movimiento seleccionado."
                });
            }

            movimiento.Fecha = request.Fecha;
            movimiento.TipoMovimiento = tipoMovimiento;
            movimiento.Monto = request.Monto;
            movimiento.QR = request.QR;
            movimiento.CBU = request.CBU;

            _context.MovimientosBilletera.Update(movimiento);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Movimiento de billetera actualizado correctamente.",
                data = new MovimientoBilleteraDTO
                {
                    Id = movimiento.Id,
                    Fecha = movimiento.Fecha,
                    TipoMovimientoId = tipoMovimiento.Id,
                    TipoMovimientoNombre = tipoMovimiento.Nombre,
                    Monto = movimiento.Monto,
                    QR = movimiento.QR,
                    CBU = movimiento.CBU
                }
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var movimiento = await _context.MovimientosBilletera
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movimiento == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el movimiento de billetera."
                });
            }

            _context.MovimientosBilletera.Remove(movimiento);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Movimiento de billetera eliminado correctamente."
            });
        }
    }
}