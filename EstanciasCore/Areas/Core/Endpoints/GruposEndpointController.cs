using DAL.Data;
using DAL.DTOs;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.Endpoints
{
    [Route("endpoint/grupos")]
    [ApiController]
    public class GruposEndpointController : ControllerBase
    {
        private readonly EstanciasContext _context;

        public GruposEndpointController(EstanciasContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var grupos = await _context.Grupos
                .Select(g => new GruposDTO
                {
                    Id = g.Id,
                    Nombre = g.Nombre
                })
                .ToListAsync();

            return Ok(new
            {
                ok = true,
                data = grupos
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var grupo = await _context.Grupos
                .Where(g => g.Id == id)
                .Select(g => new GruposDTO
                {
                    Id = g.Id,
                    Nombre = g.Nombre
                })
                .FirstOrDefaultAsync();

            if (grupo == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el grupo."
                });
            }

            return Ok(new
            {
                ok = true,
                data = grupo
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GrupoCreateRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Nombre))
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "El nombre del grupo es obligatorio."
                });
            }

            var grupo = new Grupos
            {
                Nombre = request.Nombre
            };

            await _context.Grupos.AddAsync(grupo);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Grupo creado correctamente.",
                data = new GruposDTO
                {
                    Id = grupo.Id,
                    Nombre = grupo.Nombre
                }
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] GrupoUpdateRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Nombre))
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "El nombre del grupo es obligatorio."
                });
            }

            var grupo = await _context.Grupos
                .FirstOrDefaultAsync(g => g.Id == id);

            if (grupo == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el grupo."
                });
            }

            grupo.Nombre = request.Nombre;

            _context.Grupos.Update(grupo);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Grupo actualizado correctamente.",
                data = new GruposDTO
                {
                    Id = grupo.Id,
                    Nombre = grupo.Nombre
                }
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var grupo = await _context.Grupos
                .FirstOrDefaultAsync(g => g.Id == id);

            if (grupo == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el grupo."
                });
            }

            _context.Grupos.Remove(grupo);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Grupo eliminado correctamente."
            });
        }
    }
}