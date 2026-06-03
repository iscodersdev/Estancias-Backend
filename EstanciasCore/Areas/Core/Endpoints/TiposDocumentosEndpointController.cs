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
    [Route("endpoint/tipos-documentos")]
    [ApiController]
    public class TiposDocumentosEndpointController : ControllerBase
    {
        private readonly EstanciasContext _context;

        public TiposDocumentosEndpointController(EstanciasContext context)
        {
            _context = context;

        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tiposDocumento = await _context.TipoDocumento
                .Select(t => new
                {
                    t.Id,
                    t.Descripcion
                })
                .ToListAsync();

            return Ok(new
            {
                ok = true,
                data = tiposDocumento
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var tipoDocumento = await _context.TipoDocumento
                .Where(t => t.Id == id)
                .Select(t => new
                {
                    t.Id,
                    t.Descripcion
                })
                .FirstOrDefaultAsync();

            if (tipoDocumento == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el tipo de documento."
                });
            }

            return Ok(new
            {
                ok = true,
                data = tipoDocumento
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TipoDocumentoCreateRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Descripcion))
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "La descripción es obligatoria."
                });
            }

            var tipoDocumento = new TipoDocumento
            {
                Descripcion = request.Descripcion
            };

            await _context.TipoDocumento.AddAsync(tipoDocumento);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Tipo de documento creado correctamente.",
                data = new
                {
                    tipoDocumento.Id,
                    tipoDocumento.Descripcion
                }
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TipoDocumentoUpdateRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Descripcion))
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "La descripción es obligatoria."
                });
            }

            var tipoDocumento = await _context.TipoDocumento
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tipoDocumento == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el tipo de documento."
                });
            }

            tipoDocumento.Descripcion = request.Descripcion;

            _context.TipoDocumento.Update(tipoDocumento);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Tipo de documento actualizado correctamente.",
                data = new
                {
                    tipoDocumento.Id,
                    tipoDocumento.Descripcion
                }
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var tipoDocumento = await _context.TipoDocumento
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tipoDocumento == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el tipo de documento."
                });
            }

            _context.TipoDocumento.Remove(tipoDocumento);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Tipo de documento eliminado correctamente."
            });
        }
    }
}