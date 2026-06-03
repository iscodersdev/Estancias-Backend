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
    [Route("endpoint/conceptos")]
    [ApiController]
    public class ConceptosEndpointController : ControllerBase
    {
        private readonly EstanciasContext _context;

        public ConceptosEndpointController(EstanciasContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var conceptos = await _context.Conceptos
                .Select(c => new ConceptosDTO
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Signo = c.Signo
                })
                .ToListAsync();

            return Ok(new
            {
                ok = true,
                data = conceptos
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var concepto = await _context.Conceptos
                .Where(c => c.Id == id)
                .Select(c => new ConceptosDTO
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Signo = c.Signo
                })
                .FirstOrDefaultAsync();

            if (concepto == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el concepto."
                });
            }

            return Ok(new
            {
                ok = true,
                data = concepto
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ConceptoCreateRequest request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Los datos del concepto son obligatorios."
                });
            }

            if (string.IsNullOrWhiteSpace(request.Nombre))
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "El nombre del concepto es obligatorio."
                });
            }

            var concepto = new Conceptos
            {
                Nombre = request.Nombre,
                Signo = request.Signo
            };

            await _context.Conceptos.AddAsync(concepto);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Concepto creado correctamente.",
                data = new ConceptosDTO
                {
                    Id = concepto.Id,
                    Nombre = concepto.Nombre,
                    Signo = concepto.Signo
                }
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ConceptoUpdateRequest request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Los datos del concepto son obligatorios."
                });
            }

            if (string.IsNullOrWhiteSpace(request.Nombre))
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "El nombre del concepto es obligatorio."
                });
            }

            var concepto = await _context.Conceptos
                .FirstOrDefaultAsync(c => c.Id == id);

            if (concepto == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el concepto."
                });
            }

            concepto.Nombre = request.Nombre;
            concepto.Signo = request.Signo;

            _context.Conceptos.Update(concepto);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Concepto actualizado correctamente.",
                data = new ConceptosDTO
                {
                    Id = concepto.Id,
                    Nombre = concepto.Nombre,
                    Signo = concepto.Signo
                }
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var concepto = await _context.Conceptos
                .FirstOrDefaultAsync(c => c.Id == id);

            if (concepto == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el concepto."
                });
            }

            _context.Conceptos.Remove(concepto);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Concepto eliminado correctamente."
            });
        }
    }
}