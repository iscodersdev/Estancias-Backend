using DAL.Data;
using DAL.DTOs;
using DAL.Models;
using EstanciasCore.API.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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
                .AsNoTracking()
                .ToListAsync();

            var sucursalesIds = sucursales.Select(s => s.Id).ToList();

            var relaciones = await _context.Set<SucursalesMarcas>()
                .AsNoTracking()
                .Include(sm => sm.Sucursales)
                .Include(sm => sm.Marca)
                .Where(sm => sucursalesIds.Contains(sm.Sucursales.Id))
                .ToListAsync();

            var data = sucursales.Select(s => new SucursalesDTO
            {
                Id = s.Id,
                name = s.name,
                address = s.address,
                phone = s.phone,
                latitude = s.latitude,
                longitude = s.longitude,
                group = s.group,

                Marcas = relaciones
                    .Where(r => r.Sucursales.Id == s.Id && r.Marca != null)
                    .Select(r => new MarcaSucursalDTO
                    {
                        Id = r.Marca.Id,
                        Nombre = r.Marca.Nombre
                    })
                    .ToList(),

                MarcasId = relaciones
                    .Where(r => r.Sucursales.Id == s.Id && r.Marca != null)
                    .Select(r => r.Marca.Id)
                    .ToList()
            }).ToList();

            return Ok(new
            {
                ok = true,
                data = data
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var sucursal = await _context.Sucursales
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sucursal == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró la sucursal."
                });
            }

            var data = await ArmarSucursalDTO(id);

            return Ok(new
            {
                ok = true,
                data = data
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

            var marcasIds = request.MarcasId != null
                ? request.MarcasId.Distinct().ToList()
                : new List<int>();

            var marcas = await _context.Marcas
                .Where(m => marcasIds.Contains(m.Id))
                .ToListAsync();

            if (marcas.Count != marcasIds.Count)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Una o más marcas seleccionadas no existen."
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

            foreach (var marca in marcas)
            {
                var sucursalMarca = new SucursalesMarcas
                {
                    Sucursales = sucursal,
                    Marca = marca
                };

                await _context.Set<SucursalesMarcas>().AddAsync(sucursalMarca);
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Sucursal creada correctamente.",
                data = await ArmarSucursalDTO(sucursal.Id)
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

            var marcasIds = request.MarcasId != null
                ? request.MarcasId.Distinct().ToList()
                : new List<int>();

            var marcas = await _context.Marcas
                .Where(m => marcasIds.Contains(m.Id))
                .ToListAsync();

            if (marcas.Count != marcasIds.Count)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Una o más marcas seleccionadas no existen."
                });
            }

            sucursal.name = request.name;
            sucursal.address = request.address;
            sucursal.phone = request.phone;
            sucursal.latitude = request.latitude;
            sucursal.longitude = request.longitude;
            sucursal.group = request.group;

            var marcasActuales = await _context.Set<SucursalesMarcas>()
                .Include(sm => sm.Sucursales)
                .Where(sm => sm.Sucursales.Id == id)
                .ToListAsync();

            _context.Set<SucursalesMarcas>().RemoveRange(marcasActuales);

            foreach (var marca in marcas)
            {
                var sucursalMarca = new SucursalesMarcas
                {
                    Sucursales = sucursal,
                    Marca = marca
                };

                await _context.Set<SucursalesMarcas>().AddAsync(sucursalMarca);
            }

            _context.Sucursales.Update(sucursal);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Sucursal actualizada correctamente.",
                data = await ArmarSucursalDTO(sucursal.Id)
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

            var marcasActuales = await _context.Set<SucursalesMarcas>()
                .Include(sm => sm.Sucursales)
                .Where(sm => sm.Sucursales.Id == id)
                .ToListAsync();

            _context.Set<SucursalesMarcas>().RemoveRange(marcasActuales);
            _context.Sucursales.Remove(sucursal);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Sucursal eliminada correctamente."
            });
        }

        private async Task<SucursalesDTO> ArmarSucursalDTO(int sucursalId)
        {
            var sucursal = await _context.Sucursales
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == sucursalId);

            if (sucursal == null)
            {
                return null;
            }

            var relaciones = await _context.Set<SucursalesMarcas>()
                .AsNoTracking()
                .Include(sm => sm.Sucursales)
                .Include(sm => sm.Marca)
                .Where(sm => sm.Sucursales.Id == sucursalId)
                .ToListAsync();

            return new SucursalesDTO
            {
                Id = sucursal.Id,
                name = sucursal.name,
                address = sucursal.address,
                phone = sucursal.phone,
                latitude = sucursal.latitude,
                longitude = sucursal.longitude,
                group = sucursal.group,

                Marcas = relaciones
                    .Where(r => r.Marca != null)
                    .Select(r => new MarcaSucursalDTO
                    {
                        Id = r.Marca.Id,
                        Nombre = r.Marca.Nombre
                    })
                    .ToList(),

                MarcasId = relaciones
                    .Where(r => r.Marca != null)
                    .Select(r => r.Marca.Id)
                    .ToList()
            };
        }
    }
}