using DAL.Data;
using DAL.DTOs;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.Endpoints
{
    [Route("endpoint/localidades")]
    [ApiController]
    public class LocalidadesEndpointController : ControllerBase
    {
        private readonly EstanciasContext _context;

        public LocalidadesEndpointController(EstanciasContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var localidades = await _context.Localidad
                .Select(l => new LocalidadDTO
                {
                    Id = l.Id,
                    LocalidadNombre = l.Descripcion,
                    ProvinciaNombre = l.ProvinciaNombre,
                    Latitud = l.Latitud,
                    Longitud = l.Longitud
                })
                .ToListAsync();

            return Ok(new
            {
                ok = true,
                data = localidades
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var localidad = await _context.Localidad
                .Where(l => l.Id == id)
                .Select(l => new LocalidadDTO
                {
                    Id = l.Id,
                    LocalidadNombre = l.Descripcion,
                    ProvinciaNombre = l.ProvinciaNombre,
                    Latitud = l.Latitud,
                    Longitud = l.Longitud
                })
                .FirstOrDefaultAsync();

            if (localidad == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró la localidad."
                });
            }

            return Ok(new
            {
                ok = true,
                data = localidad
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LocalidadCreateRequest request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Los datos de la localidad son obligatorios."
                });
            }

            if (string.IsNullOrWhiteSpace(request.Descripcion))
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "El nombre de la localidad es obligatorio."
                });
            }

            if (request.IdProvincia == 0)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Debe seleccionar una provincia."
                });
            }

            var provincia = await _context.Provincia
                .FirstOrDefaultAsync(p => p.Id == request.IdProvincia);

            if (provincia == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró la provincia seleccionada."
                });
            }

            var localidad = new Localidad
            {
                Descripcion = request.Descripcion,
                IdProvincia = provincia.Id,
                ProvinciaNombre = provincia.Descripcion,
                Latitud = request.Latitud,
                Longitud = request.Longitud
            };

            await _context.Localidad.AddAsync(localidad);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Localidad creada correctamente.",
                data = new LocalidadDTO
                {
                    Id = localidad.Id,
                    LocalidadNombre = localidad.Descripcion,
                    ProvinciaNombre = localidad.ProvinciaNombre,
                    Latitud = localidad.Latitud,
                    Longitud = localidad.Longitud
                }
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] LocalidadUpdateRequest request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Los datos de la localidad son obligatorios."
                });
            }

            if (string.IsNullOrWhiteSpace(request.Descripcion))
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "El nombre de la localidad es obligatorio."
                });
            }

            if (request.IdProvincia == 0)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Debe seleccionar una provincia."
                });
            }

            var localidad = await _context.Localidad
                .FirstOrDefaultAsync(l => l.Id == id);

            if (localidad == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró la localidad."
                });
            }

            var provincia = await _context.Provincia
                .FirstOrDefaultAsync(p => p.Id == request.IdProvincia);

            if (provincia == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró la provincia seleccionada."
                });
            }

            localidad.Descripcion = request.Descripcion;
            localidad.IdProvincia = provincia.Id;
            localidad.ProvinciaNombre = provincia.Descripcion;
            localidad.Latitud = request.Latitud;
            localidad.Longitud = request.Longitud;

            _context.Localidad.Update(localidad);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Localidad actualizada correctamente.",
                data = new LocalidadDTO
                {
                    Id = localidad.Id,
                    LocalidadNombre = localidad.Descripcion,
                    ProvinciaNombre = localidad.ProvinciaNombre,
                    Latitud = localidad.Latitud,
                    Longitud = localidad.Longitud
                }
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var localidad = await _context.Localidad
                .FirstOrDefaultAsync(l => l.Id == id);

            if (localidad == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró la localidad."
                });
            }

            _context.Localidad.Remove(localidad);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Localidad eliminada correctamente."
            });
        }
    }
}