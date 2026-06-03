using DAL.Data;
using DAL.DTOs;
using DAL.Models;
using EstanciasCore.API.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.Endpoints
{
    [TypeFilter(typeof(EndpointUatAuthorizeAttribute))]
    [Route("endpoint/leyendas-tipos-movimientos")]
    [ApiController]
    public class LeyendaTipoMovimientoEndpointController : ControllerBase
    {
        private readonly EstanciasContext _context;

        public LeyendaTipoMovimientoEndpointController(EstanciasContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var leyendas = await _context.LeyendaTipoMovimiento
                .Select(l => new LeyendaTipoMovimientoDTO
                {
                    Id = l.Id,
                    NombreMovimiento = l.NombreMovimiento,
                    TextoLeyenda = l.TextoLeyenda,
                    Activo = l.Activo
                })
                .ToListAsync();

            return Ok(new
            {
                ok = true,
                data = leyendas
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var leyenda = await _context.LeyendaTipoMovimiento
                .Where(l => l.Id == id)
                .Select(l => new LeyendaTipoMovimientoDTO
                {
                    Id = l.Id,
                    NombreMovimiento = l.NombreMovimiento,
                    TextoLeyenda = l.TextoLeyenda,
                    Activo = l.Activo
                })
                .FirstOrDefaultAsync();

            if (leyenda == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró la leyenda."
                });
            }

            return Ok(new
            {
                ok = true,
                data = leyenda
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LeyendaTipoMovimientoCreateRequest request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Los datos de la leyenda son obligatorios."
                });
            }

            if (string.IsNullOrWhiteSpace(request.NombreMovimiento))
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "El nombre del movimiento es obligatorio."
                });
            }

            if (string.IsNullOrWhiteSpace(request.TextoLeyenda))
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "El texto de la leyenda es obligatorio."
                });
            }

            var leyenda = new LeyendaTipoMovimiento
            {
                NombreMovimiento = request.NombreMovimiento,
                TextoLeyenda = request.TextoLeyenda,
                Activo = true
            };

            await _context.LeyendaTipoMovimiento.AddAsync(leyenda);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Leyenda creada correctamente.",
                data = new LeyendaTipoMovimientoDTO
                {
                    Id = leyenda.Id,
                    NombreMovimiento = leyenda.NombreMovimiento,
                    TextoLeyenda = leyenda.TextoLeyenda,
                    Activo = leyenda.Activo
                }
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] LeyendaTipoMovimientoUpdateRequest request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Los datos de la leyenda son obligatorios."
                });
            }

            if (string.IsNullOrWhiteSpace(request.NombreMovimiento))
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "El nombre del movimiento es obligatorio."
                });
            }

            if (string.IsNullOrWhiteSpace(request.TextoLeyenda))
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "El texto de la leyenda es obligatorio."
                });
            }

            var leyenda = await _context.LeyendaTipoMovimiento
                .FirstOrDefaultAsync(l => l.Id == id);

            if (leyenda == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró la leyenda."
                });
            }

            leyenda.NombreMovimiento = request.NombreMovimiento;
            leyenda.TextoLeyenda = request.TextoLeyenda;
            leyenda.Activo = request.Activo;

            _context.LeyendaTipoMovimiento.Update(leyenda);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Leyenda actualizada correctamente.",
                data = new LeyendaTipoMovimientoDTO
                {
                    Id = leyenda.Id,
                    NombreMovimiento = leyenda.NombreMovimiento,
                    TextoLeyenda = leyenda.TextoLeyenda,
                    Activo = leyenda.Activo
                }
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var leyenda = await _context.LeyendaTipoMovimiento
                .FirstOrDefaultAsync(l => l.Id == id);

            if (leyenda == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró la leyenda."
                });
            }

            _context.LeyendaTipoMovimiento.Remove(leyenda);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Leyenda eliminada correctamente."
            });
        }

        [HttpPatch("{id}/habilitar")]
        public async Task<IActionResult> Habilitar(int id)
        {
            var leyenda = await _context.LeyendaTipoMovimiento
                .FirstOrDefaultAsync(l => l.Id == id);

            if (leyenda == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró la leyenda."
                });
            }

            leyenda.Activo = true;

            _context.LeyendaTipoMovimiento.Update(leyenda);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Leyenda habilitada correctamente.",
                data = new LeyendaTipoMovimientoDTO
                {
                    Id = leyenda.Id,
                    NombreMovimiento = leyenda.NombreMovimiento,
                    TextoLeyenda = leyenda.TextoLeyenda,
                    Activo = leyenda.Activo
                }
            });
        }

        [HttpPatch("{id}/deshabilitar")]
        public async Task<IActionResult> Deshabilitar(int id)
        {
            var leyenda = await _context.LeyendaTipoMovimiento
                .FirstOrDefaultAsync(l => l.Id == id);

            if (leyenda == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró la leyenda."
                });
            }

            leyenda.Activo = false;

            _context.LeyendaTipoMovimiento.Update(leyenda);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Leyenda deshabilitada correctamente.",
                data = new LeyendaTipoMovimientoDTO
                {
                    Id = leyenda.Id,
                    NombreMovimiento = leyenda.NombreMovimiento,
                    TextoLeyenda = leyenda.TextoLeyenda,
                    Activo = leyenda.Activo
                }
            });
        }
    }
}