using DAL.Data;
using DAL.DTOs;
using DAL.Models.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.Endpoints
{
    [Route("endpoint/rubros")]
    [ApiController]
    public class RubrosEndpointController : ControllerBase
    {
        private readonly EstanciasContext _context;

        public RubrosEndpointController(EstanciasContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var rubros = await _context.Rubros
                .Where(r => r.Activo)
                .Select(r => new RubroDTO
                {
                    Id = r.Id,
                    Nombre = r.Nombre,
                    Descripcion = r.Descripcion,
                    Activo = r.Activo
                })
                .ToListAsync();

            return Ok(new
            {
                ok = true,
                data = rubros
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var rubro = await _context.Rubros
                .Where(r => r.Id == id)
                .Select(r => new RubroDTO
                {
                    Id = r.Id,
                    Nombre = r.Nombre,
                    Descripcion = r.Descripcion,
                    Activo = r.Activo
                })
                .FirstOrDefaultAsync();

            if (rubro == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el rubro."
                });
            }

            return Ok(new
            {
                ok = true,
                data = rubro
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RubroCreateRequest request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Los datos del rubro son obligatorios."
                });
            }

            if (string.IsNullOrWhiteSpace(request.Nombre))
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "El nombre del rubro es obligatorio."
                });
            }

            var rubro = new Rubro
            {
                Nombre = request.Nombre,
                Descripcion = request.Descripcion,
                Activo = true
            };

            await _context.Rubros.AddAsync(rubro);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Rubro creado correctamente.",
                data = new RubroDTO
                {
                    Id = rubro.Id,
                    Nombre = rubro.Nombre,
                    Descripcion = rubro.Descripcion,
                    Activo = rubro.Activo
                }
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RubroUpdateRequest request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Los datos del rubro son obligatorios."
                });
            }

            if (string.IsNullOrWhiteSpace(request.Nombre))
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "El nombre del rubro es obligatorio."
                });
            }

            var rubro = await _context.Rubros
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rubro == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el rubro."
                });
            }

            rubro.Nombre = request.Nombre;
            rubro.Descripcion = request.Descripcion;
            rubro.Activo = request.Activo;

            _context.Rubros.Update(rubro);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Rubro actualizado correctamente.",
                data = new RubroDTO
                {
                    Id = rubro.Id,
                    Nombre = rubro.Nombre,
                    Descripcion = rubro.Descripcion,
                    Activo = rubro.Activo
                }
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var rubro = await _context.Rubros
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rubro == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el rubro."
                });
            }

            rubro.Activo = false;

            _context.Rubros.Update(rubro);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Rubro eliminado correctamente.",
                data = new RubroDTO
                {
                    Id = rubro.Id,
                    Nombre = rubro.Nombre,
                    Descripcion = rubro.Descripcion,
                    Activo = rubro.Activo
                }
            });
        }
    }
}