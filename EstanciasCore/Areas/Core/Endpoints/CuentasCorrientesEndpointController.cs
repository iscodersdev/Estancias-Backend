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
    [Route("endpoint/cuentas-corrientes")]
    [ApiController]
    public class CuentasCorrientesEndpointController : ControllerBase
    {
        private readonly EstanciasContext _context;

        public CuentasCorrientesEndpointController(EstanciasContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var cuentas = await _context.CuentaCorriente
                .Include(c => c.Cliente)
                    .ThenInclude(c => c.Persona)
                .ToListAsync();

            var data = cuentas.Select(c => new CuentasCorrientesDTO
            {
                Id = c.Id,
                ClienteNombre = c.Cliente != null && c.Cliente.Persona != null
                    ? c.Cliente.Persona.Nombres + "-" + c.Cliente.Persona.Apellido
                    : null,
                Fecha = c.Fecha.ToShortDateString(),
                Vencimiento = c.Vencimiento.HasValue ? c.Vencimiento.Value.ToShortDateString() : null,
                Saldo = c.Saldo
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
            var cuenta = await _context.CuentaCorriente
                .Include(c => c.Cliente)
                    .ThenInclude(c => c.Persona)
                .Include(c => c.Concepto)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cuenta == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró la cuenta corriente."
                });
            }

            var data = new CuentaCorrienteDetalleDTO
            {
                Id = cuenta.Id,
                ClienteId = cuenta.Cliente != null ? cuenta.Cliente.Id : 0,
                ClienteNombre = cuenta.Cliente != null && cuenta.Cliente.Persona != null
                    ? cuenta.Cliente.Persona.Nombres + "-" + cuenta.Cliente.Persona.Apellido
                    : null,
                ConceptoId = cuenta.Concepto != null ? cuenta.Concepto.Id : 0,
                ConceptoNombre = cuenta.Concepto != null ? cuenta.Concepto.Nombre : null,
                Fecha = cuenta.Fecha,
                Vencimiento = cuenta.Vencimiento,
                Observaciones = cuenta.Observaciones,
                Importe = cuenta.Importe,
                Saldo = cuenta.Saldo
            };

            return Ok(new
            {
                ok = true,
                data = data
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CuentaCorrienteCreateRequest request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Los datos de la cuenta corriente son obligatorios."
                });
            }

            if (request.ClienteId == 0)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Debe seleccionar un cliente."
                });
            }

            if (request.ConceptoId == 0)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Debe seleccionar un concepto."
                });
            }

            var cliente = await _context.Clientes
                .Include(c => c.Persona)
                .FirstOrDefaultAsync(c => c.Id == request.ClienteId);

            if (cliente == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el cliente seleccionado."
                });
            }

            var concepto = await _context.Conceptos
                .FirstOrDefaultAsync(c => c.Id == request.ConceptoId);

            if (concepto == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el concepto seleccionado."
                });
            }

            var cuenta = new CuentaCorriente
            {
                Fecha = request.Fecha,
                Vencimiento = request.Vencimiento,
                Observaciones = request.Observaciones,
                Importe = request.Importe,
                Saldo = request.Saldo,
                Cliente = cliente,
                Concepto = concepto
            };

            await _context.CuentaCorriente.AddAsync(cuenta);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Cuenta corriente creada correctamente.",
                data = new CuentaCorrienteDetalleDTO
                {
                    Id = cuenta.Id,
                    ClienteId = cliente.Id,
                    ClienteNombre = cliente.Persona != null
                        ? cliente.Persona.Nombres + "-" + cliente.Persona.Apellido
                        : null,
                    ConceptoId = concepto.Id,
                    ConceptoNombre = concepto.Nombre,
                    Fecha = cuenta.Fecha,
                    Vencimiento = cuenta.Vencimiento,
                    Observaciones = cuenta.Observaciones,
                    Importe = cuenta.Importe,
                    Saldo = cuenta.Saldo
                }
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CuentaCorrienteUpdateRequest request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Los datos de la cuenta corriente son obligatorios."
                });
            }

            if (request.ClienteId == 0)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Debe seleccionar un cliente."
                });
            }

            if (request.ConceptoId == 0)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Debe seleccionar un concepto."
                });
            }

            var cuenta = await _context.CuentaCorriente
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cuenta == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró la cuenta corriente."
                });
            }

            var cliente = await _context.Clientes
                .Include(c => c.Persona)
                .FirstOrDefaultAsync(c => c.Id == request.ClienteId);

            if (cliente == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el cliente seleccionado."
                });
            }

            var concepto = await _context.Conceptos
                .FirstOrDefaultAsync(c => c.Id == request.ConceptoId);

            if (concepto == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el concepto seleccionado."
                });
            }

            cuenta.Fecha = request.Fecha;
            cuenta.Vencimiento = request.Vencimiento;
            cuenta.Observaciones = request.Observaciones;
            cuenta.Importe = request.Importe;
            cuenta.Saldo = request.Saldo;
            cuenta.Cliente = cliente;
            cuenta.Concepto = concepto;

            _context.CuentaCorriente.Update(cuenta);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Cuenta corriente actualizada correctamente.",
                data = new CuentaCorrienteDetalleDTO
                {
                    Id = cuenta.Id,
                    ClienteId = cliente.Id,
                    ClienteNombre = cliente.Persona != null
                        ? cliente.Persona.Nombres + "-" + cliente.Persona.Apellido
                        : null,
                    ConceptoId = concepto.Id,
                    ConceptoNombre = concepto.Nombre,
                    Fecha = cuenta.Fecha,
                    Vencimiento = cuenta.Vencimiento,
                    Observaciones = cuenta.Observaciones,
                    Importe = cuenta.Importe,
                    Saldo = cuenta.Saldo
                }
            });
        }
    }

    public static class CuentaCorrienteExtensions
    {
        public static bool ClientienteTienePersona(this CuentaCorriente cuenta)
        {
            return cuenta.Cliente != null && cuenta.Cliente.Persona != null;
        }
    }
}