using DAL.Data;
using DAL.DTOs;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.Endpoints
{
    [Route("endpoint/tipos-clientes")]
    [ApiController]
    public class TiposClientesEndpointController : ControllerBase
    {
        private readonly EstanciasContext _context;

        public TiposClientesEndpointController(EstanciasContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tiposClientes = await _context.TiposClientes
                .Select(t => new TiposClientesDTO
                {
                    Id = t.Id,
                    Nombre = t.Nombre,
                    CantidadActividadesSemanales = t.CantidadActividadesSemanales
                })
                .ToListAsync();

            return Ok(new
            {
                ok = true,
                data = tiposClientes
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var tipoCliente = await _context.TiposClientes
                .Where(t => t.Id == id)
                .Select(t => new TiposClientesDTO
                {
                    Id = t.Id,
                    Nombre = t.Nombre,
                    CantidadActividadesSemanales = t.CantidadActividadesSemanales
                })
                .FirstOrDefaultAsync();

            if (tipoCliente == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el tipo de cliente."
                });
            }

            return Ok(new
            {
                ok = true,
                data = tipoCliente
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TipoClienteCreateRequest request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Los datos del tipo de cliente son obligatorios."
                });
            }

            if (string.IsNullOrWhiteSpace(request.Nombre))
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "El nombre del tipo de cliente es obligatorio."
                });
            }

            if (request.CantidadActividadesSemanales < 0)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "La cantidad de actividades semanales no puede ser negativa."
                });
            }

            var tipoCliente = new TiposClientes
            {
                Nombre = request.Nombre,
                CantidadActividadesSemanales = request.CantidadActividadesSemanales
            };

            await _context.TiposClientes.AddAsync(tipoCliente);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Tipo de cliente creado correctamente.",
                data = new TiposClientesDTO
                {
                    Id = tipoCliente.Id,
                    Nombre = tipoCliente.Nombre,
                    CantidadActividadesSemanales = tipoCliente.CantidadActividadesSemanales
                }
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TipoClienteUpdateRequest request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Los datos del tipo de cliente son obligatorios."
                });
            }

            if (string.IsNullOrWhiteSpace(request.Nombre))
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "El nombre del tipo de cliente es obligatorio."
                });
            }

            if (request.CantidadActividadesSemanales < 0)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "La cantidad de actividades semanales no puede ser negativa."
                });
            }

            var tipoCliente = await _context.TiposClientes
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tipoCliente == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el tipo de cliente."
                });
            }

            tipoCliente.Nombre = request.Nombre;
            tipoCliente.CantidadActividadesSemanales = request.CantidadActividadesSemanales;

            _context.TiposClientes.Update(tipoCliente);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Tipo de cliente actualizado correctamente.",
                data = new TiposClientesDTO
                {
                    Id = tipoCliente.Id,
                    Nombre = tipoCliente.Nombre,
                    CantidadActividadesSemanales = tipoCliente.CantidadActividadesSemanales
                }
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var tipoCliente = await _context.TiposClientes
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tipoCliente == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el tipo de cliente."
                });
            }

            _context.TiposClientes.Remove(tipoCliente);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Tipo de cliente eliminado correctamente."
            });
        }
    }
}