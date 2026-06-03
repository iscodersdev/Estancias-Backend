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
    [Route("endpoint/paises")]
    [ApiController]
    public class PaisesEndpointController : ControllerBase
    {
        private readonly EstanciasContext _context;

        public PaisesEndpointController(EstanciasContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var paises = await _context.Paises
                .Select(p => new PaisesDTO
                {
                    Id = p.Id,
                    Nombre = p.Nombre
                })
                .ToListAsync();

            return Ok(new
            {
                ok = true,
                data = paises
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var pais = await _context.Paises
                .Where(p => p.Id == id)
                .Select(p => new PaisesDTO
                {
                    Id = p.Id,
                    Nombre = p.Nombre
                })
                .FirstOrDefaultAsync();

            if (pais == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el país."
                });
            }

            return Ok(new
            {
                ok = true,
                data = pais
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PaisCreateRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Nombre))
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "El nombre del país es obligatorio."
                });
            }

            var pais = new Paises
            {
                Nombre = request.Nombre
            };

            await _context.Paises.AddAsync(pais);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "País creado correctamente.",
                data = new PaisesDTO
                {
                    Id = pais.Id,
                    Nombre = pais.Nombre
                }
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PaisUpdateRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Nombre))
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "El nombre del país es obligatorio."
                });
            }

            var pais = await _context.Paises
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pais == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el país."
                });
            }

            pais.Nombre = request.Nombre;

            _context.Paises.Update(pais);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "País actualizado correctamente.",
                data = new PaisesDTO
                {
                    Id = pais.Id,
                    Nombre = pais.Nombre
                }
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var pais = await _context.Paises
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pais == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el país."
                });
            }

            _context.Paises.Remove(pais);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "País eliminado correctamente."
            });
        }
    }
}