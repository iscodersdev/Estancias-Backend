using Commons.Models;
using DAL.Data;
using DAL.DTOs;
using DAL.Models;
using EstanciasCore.API.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.Endpoints
{
    [TypeFilter(typeof(EndpointUatAuthorizeAttribute))]
    [ApiController]
    [Route("endpoint/provincias")]
    public class ProvinciasEndpointController : ControllerBase
    {
        private readonly EstanciasContext _context;
        public ProvinciasEndpointController(EstanciasContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> getAll()
        {
            var provincias = await _context.Provincia.Select(p => new ProvinciaDTO
            {
                Id = p.Id,
                Latitud = p.Latitud,
                Longitud = p.Longitud,
                Descripcion = p.Descripcion,
                DescripcionCompleta = p.DescripcionCompleta,
            })
            .ToListAsync();

            return Ok(new
            {
                ok = true,
                data = provincias
            });
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var provincia = await _context.Provincia
                .Where(p => p.Id == id)
                .Select(p => new ProvinciaDTO
                {
                    Id = p.Id,
                    Latitud = p.Latitud,
                    Longitud = p.Longitud,
                    Descripcion = p.Descripcion,
                    DescripcionCompleta = p.DescripcionCompleta
                })
                .FirstOrDefaultAsync();

            if (provincia == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró la provincia."
                });
            }

            return Ok(new
            {
                ok = true,
                data = provincia
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProvinciaCreateRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Descripcion))
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "La descripción de la provincia es obligatoria."
                });
            }

            var provincia = new Provincia
            {
                Latitud = request.Latitud,
                Longitud = request.Longitud,
                Descripcion = request.Descripcion,
                DescripcionCompleta = request.DescripcionCompleta
            };

            await _context.Provincia.AddAsync(provincia);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Provincia creada correctamente.",
                data = new ProvinciaDTO
                {
                    Id = provincia.Id,
                    Latitud = provincia.Latitud,
                    Longitud = provincia.Longitud,
                    Descripcion = provincia.Descripcion,
                    DescripcionCompleta = provincia.DescripcionCompleta
                }
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProvinciaUpdateRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Descripcion))
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "La descripción de la provincia es obligatoria."
                });
            }

            var provincia = await _context.Provincia
                .FirstOrDefaultAsync(p => p.Id == id);

            if (provincia == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró la provincia."
                });
            }

            provincia.Latitud = request.Latitud;
            provincia.Longitud = request.Longitud;
            provincia.Descripcion = request.Descripcion;
            provincia.DescripcionCompleta = request.DescripcionCompleta;

            _context.Provincia.Update(provincia);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Provincia actualizada correctamente.",
                data = new ProvinciaDTO
                {
                    Id = provincia.Id,
                    Latitud = provincia.Latitud,
                    Longitud = provincia.Longitud,
                    Descripcion = provincia.Descripcion,
                    DescripcionCompleta = provincia.DescripcionCompleta
                }
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var provincia = await _context.Provincia
                .FirstOrDefaultAsync(p => p.Id == id);

            if (provincia == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró la provincia."
                });
            }

            _context.Provincia.Remove(provincia);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Provincia eliminada correctamente."
            });
        }
    }
}