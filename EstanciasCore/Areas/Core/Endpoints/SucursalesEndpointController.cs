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
                .OrderBy(s => s.Id)
                .ToListAsync();

            var sucursalesIds = sucursales.Select(s => s.Id).ToList();

            var relaciones = new List<SucursalesMarcas>();

            if (sucursalesIds.Any())
            {
                relaciones = await _context.Set<SucursalesMarcas>()
                    .AsNoTracking()
                    .Include(sm => sm.Sucursales)
                    .Include(sm => sm.Marca)
                    .Where(sm => sm.Sucursales != null && sucursalesIds.Contains(sm.Sucursales.Id))
                    .ToListAsync();
            }

            var data = sucursales.Select(s =>
            {
                var marcasDeSucursal = relaciones
                    .Where(r => r.Sucursales != null && r.Sucursales.Id == s.Id && r.Marca != null)
                    .ToList();

                var nombresMarcas = marcasDeSucursal
                    .Select(r => r.Marca.Nombre)
                    .ToList();

                var idsMarcas = marcasDeSucursal
                    .Select(r => r.Marca.Id)
                    .ToList();

                return new SucursalesEndpointDTO
                {
                    Id = s.Id,
                    name = s.name,
                    address = s.address,
                    phone = s.phone,
                    latitude = s.latitude,
                    longitude = s.longitude,
                    group = s.group,

                    Marca = string.Join(", ", nombresMarcas),
                    Marcas = nombresMarcas,
                    MarcasId = idsMarcas
                };
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

            var data = await ArmarSucursalEndpointDTO(id);

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

            var marcas = new List<Marcas>();

            if (marcasIds.Any())
            {
                marcas = await _context.Marcas
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
                data = await ArmarSucursalEndpointDTO(sucursal.Id)
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

            var marcas = new List<Marcas>();

            if (marcasIds.Any())
            {
                marcas = await _context.Marcas
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
            }

            sucursal.name = request.name;
            sucursal.address = request.address;
            sucursal.phone = request.phone;
            sucursal.latitude = request.latitude;
            sucursal.longitude = request.longitude;
            sucursal.group = request.group;

            var marcasActuales = await _context.Set<SucursalesMarcas>()
                .Include(sm => sm.Sucursales)
                .Where(sm => sm.Sucursales != null && sm.Sucursales.Id == id)
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
                data = await ArmarSucursalEndpointDTO(sucursal.Id)
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
                .Where(sm => sm.Sucursales != null && sm.Sucursales.Id == id)
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

        private async Task<SucursalesEndpointDTO> ArmarSucursalEndpointDTO(int sucursalId)
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
                .Where(sm => sm.Sucursales != null && sm.Sucursales.Id == sucursalId)
                .ToListAsync();

            var nombresMarcas = relaciones
                .Where(r => r.Marca != null)
                .Select(r => r.Marca.Nombre)
                .ToList();

            var idsMarcas = relaciones
                .Where(r => r.Marca != null)
                .Select(r => r.Marca.Id)
                .ToList();

            return new SucursalesEndpointDTO
            {
                Id = sucursal.Id,
                name = sucursal.name,
                address = sucursal.address,
                phone = sucursal.phone,
                latitude = sucursal.latitude,
                longitude = sucursal.longitude,
                group = sucursal.group,

                Marca = string.Join(", ", nombresMarcas),
                Marcas = nombresMarcas,
                MarcasId = idsMarcas
            };
        }
    }
}