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
    [Route("endpoint/sucursales")]
    [ApiController]
    public class SucursalesEndpointController : ControllerBase
    {
        private readonly EstanciasContext _context;

        public SucursalesEndpointController(EstanciasContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var sucursales = await _context.Sucursales
                .Select(s => new SucursalesDTO
                {
                    Id = s.Id,
                    name = s.name,
                    address = s.address,
                    phone = s.phone,
                    latitude = s.latitude,
                    longitude = s.longitude,
                    group = s.group
                })
                .ToListAsync();

            return Ok(new
            {
                ok = true,
                data = sucursales
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var sucursal = await _context.Sucursales
                .Where(s => s.Id == id)
                .Select(s => new SucursalesDTO
                {
                    Id = s.Id,
                    name = s.name,
                    address = s.address,
                    phone = s.phone,
                    latitude = s.latitude,
                    longitude = s.longitude,
                    group = s.group
                })
                .FirstOrDefaultAsync();

            if (sucursal == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró la sucursal."
                });
            }

            return Ok(new
            {
                ok = true,
                data = sucursal
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SucursalCreateRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.name))
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "El nombre de la sucursal es obligatorio."
                });
            }

            var sucursal = new Sucursales
            {
                name = request.name,
                address = request.address,
                phone = request.phone,
                latitude = request.latitude,
                longitude = request.longitude,
                group = request.group
            };

            await _context.Sucursales.AddAsync(sucursal);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Sucursal creada correctamente.",
                data = new SucursalesDTO
                {
                    Id = sucursal.Id,
                    name = sucursal.name,
                    address = sucursal.address,
                    phone = sucursal.phone,
                    latitude = sucursal.latitude,
                    longitude = sucursal.longitude,
                    group = sucursal.group
                }
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SucursalUpdateRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.name))
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "El nombre de la sucursal es obligatorio."
                });
            }

            var sucursal = await _context.Sucursales
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sucursal == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró la sucursal."
                });
            }

            sucursal.name = request.name;
            sucursal.address = request.address;
            sucursal.phone = request.phone;
            sucursal.latitude = request.latitude;
            sucursal.longitude = request.longitude;
            sucursal.group = request.group;

            _context.Sucursales.Update(sucursal);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Sucursal actualizada correctamente.",
                data = new SucursalesDTO
                {
                    Id = sucursal.Id,
                    name = sucursal.name,
                    address = sucursal.address,
                    phone = sucursal.phone,
                    latitude = sucursal.latitude,
                    longitude = sucursal.longitude,
                    group = sucursal.group
                }
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var sucursal = await _context.Sucursales
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sucursal == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró la sucursal."
                });
            }

            _context.Sucursales.Remove(sucursal);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Sucursal eliminada correctamente."
            });
        }
    }
}