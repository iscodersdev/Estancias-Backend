using DAL.Data;
using DAL.DTOs;
using DAL.Models.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.Endpoints
{
    [Route("endpoint/tipos-movimientos-billetera")]
    [ApiController]
    public class TipoMovimientoBilleteraEndpointController : ControllerBase
    {
        private readonly EstanciasContext _context;

        public TipoMovimientoBilleteraEndpointController(EstanciasContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tipos = await _context.TipoMovimientoBilletera
                .Select(t => new TipoMovimientoBilleteraDTO
                {
                    Id = t.Id,
                    Nombre = t.Nombre,
                    Credito = t.Credito,
                    Debito = t.Debito
                })
                .ToListAsync();

            return Ok(new
            {
                ok = true,
                data = tipos
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var tipo = await _context.TipoMovimientoBilletera
                .Where(t => t.Id == id)
                .Select(t => new TipoMovimientoBilleteraDTO
                {
                    Id = t.Id,
                    Nombre = t.Nombre,
                    Credito = t.Credito,
                    Debito = t.Debito
                })
                .FirstOrDefaultAsync();

            if (tipo == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el tipo de movimiento de billetera."
                });
            }

            return Ok(new
            {
                ok = true,
                data = tipo
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TipoMovimientoBilleteraCreateRequest request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Los datos del tipo de movimiento de billetera son obligatorios."
                });
            }

            if (string.IsNullOrWhiteSpace(request.Nombre))
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "El nombre del tipo de movimiento de billetera es obligatorio."
                });
            }

            if (request.Credito == request.Debito)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Debe indicar si el movimiento es crédito o débito, pero no ambos."
                });
            }

            var tipo = new TipoMovimientoBilletera
            {
                Nombre = request.Nombre,
                Credito = request.Credito,
                Debito = request.Debito
            };

            await _context.TipoMovimientoBilletera.AddAsync(tipo);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Tipo de movimiento de billetera creado correctamente.",
                data = new TipoMovimientoBilleteraDTO
                {
                    Id = tipo.Id,
                    Nombre = tipo.Nombre,
                    Credito = tipo.Credito,
                    Debito = tipo.Debito
                }
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TipoMovimientoBilleteraUpdateRequest request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Los datos del tipo de movimiento de billetera son obligatorios."
                });
            }

            if (string.IsNullOrWhiteSpace(request.Nombre))
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "El nombre del tipo de movimiento de billetera es obligatorio."
                });
            }

            if (request.Credito == request.Debito)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Debe indicar si el movimiento es crédito o débito, pero no ambos."
                });
            }

            var tipo = await _context.TipoMovimientoBilletera
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tipo == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el tipo de movimiento de billetera."
                });
            }

            tipo.Nombre = request.Nombre;
            tipo.Credito = request.Credito;
            tipo.Debito = request.Debito;

            _context.TipoMovimientoBilletera.Update(tipo);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Tipo de movimiento de billetera actualizado correctamente.",
                data = new TipoMovimientoBilleteraDTO
                {
                    Id = tipo.Id,
                    Nombre = tipo.Nombre,
                    Credito = tipo.Credito,
                    Debito = tipo.Debito
                }
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var tipo = await _context.TipoMovimientoBilletera
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tipo == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el tipo de movimiento de billetera."
                });
            }

            _context.TipoMovimientoBilletera.Remove(tipo);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Tipo de movimiento de billetera eliminado correctamente."
            });
        }
    }
}