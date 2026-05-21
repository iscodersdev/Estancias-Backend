using DAL.Data;
using DAL.DTOs;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.Endpoints
{
    [Route("endpoint/categorias")]
    [ApiController]
    public class CategoriasEndpointController : ControllerBase
    {
        private readonly EstanciasContext _context;

        public CategoriasEndpointController(EstanciasContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categorias = await _context.Categorias
                .Select(c => new CategoriasDTO
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Activo = c.Activo
                })
                .ToListAsync();

            return Ok(new
            {
                ok = true,
                data = categorias
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var categoria = await _context.Categorias
                .Where(c => c.Id == id)
                .Select(c => new CategoriasDTO
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Activo = c.Activo
                })
                .FirstOrDefaultAsync();

            if (categoria == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró la categoría."
                });
            }

            return Ok(new
            {
                ok = true,
                data = categoria
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoriaCreateRequest request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Los datos de la categoría son obligatorios."
                });
            }

            if (string.IsNullOrWhiteSpace(request.Nombre))
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "El nombre de la categoría es obligatorio."
                });
            }

            var categoria = new Categorias
            {
                Nombre = request.Nombre,
                Activo = request.Activo
            };

            await _context.Categorias.AddAsync(categoria);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Categoría creada correctamente.",
                data = new CategoriasDTO
                {
                    Id = categoria.Id,
                    Nombre = categoria.Nombre,
                    Activo = categoria.Activo
                }
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoriaUpdateRequest request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Los datos de la categoría son obligatorios."
                });
            }

            if (string.IsNullOrWhiteSpace(request.Nombre))
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "El nombre de la categoría es obligatorio."
                });
            }

            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categoria == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró la categoría."
                });
            }

            categoria.Nombre = request.Nombre;
            categoria.Activo = request.Activo;

            _context.Categorias.Update(categoria);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Categoría actualizada correctamente.",
                data = new CategoriasDTO
                {
                    Id = categoria.Id,
                    Nombre = categoria.Nombre,
                    Activo = categoria.Activo
                }
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categoria == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró la categoría."
                });
            }

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Categoría eliminada correctamente."
            });
        }
    }
}