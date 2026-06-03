using DAL.Data;
using DAL.DTOs;
using DAL.Models.Core;
using EstanciasCore.API.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.Endpoints
{
    [TypeFilter(typeof(EndpointUatAuthorizeAttribute))]
    [Route("endpoint/billeteras")]
    [ApiController]
    public class BilleterasEndpointController : ControllerBase
    {
        private readonly EstanciasContext _context;

        public BilleterasEndpointController(EstanciasContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var billeteras = await _context.Billeteras
                .Include(b => b.Cliente)
                    .ThenInclude(c => c.Persona)
                .Include(b => b.Cliente)
                    .ThenInclude(c => c.Usuario)
                .ToListAsync();

            var data = billeteras.Select(b => new BilleteraDTO
            {
                Id = b.Id,
                ClienteId = b.Cliente != null ? b.Cliente.Id : 0,
                ClienteNombre = b.Cliente != null && b.Cliente.Persona != null
                    ? b.Cliente.Persona.Apellido + ", " + b.Cliente.Persona.Nombres
                    : null,
                ClienteEmail = b.Cliente != null && b.Cliente.Usuario != null
                    ? b.Cliente.Usuario.Email
                    : null,
                Saldo = b.Saldo,
                QRCobro = b.QRCobro,
                AliasCVU = b.AliasCVU,
                CVU = b.CVU
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
            var billetera = await _context.Billeteras
                .Include(b => b.Cliente)
                    .ThenInclude(c => c.Persona)
                .Include(b => b.Cliente)
                    .ThenInclude(c => c.Usuario)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (billetera == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró la billetera."
                });
            }

            var data = new BilleteraDTO
            {
                Id = billetera.Id,
                ClienteId = billetera.Cliente != null ? billetera.Cliente.Id : 0,
                ClienteNombre = billetera.Cliente != null && billetera.Cliente.Persona != null
                    ? billetera.Cliente.Persona.Apellido + ", " + billetera.Cliente.Persona.Nombres
                    : null,
                ClienteEmail = billetera.Cliente != null && billetera.Cliente.Usuario != null
                    ? billetera.Cliente.Usuario.Email
                    : null,
                Saldo = billetera.Saldo,
                QRCobro = billetera.QRCobro,
                AliasCVU = billetera.AliasCVU,
                CVU = billetera.CVU
            };

            return Ok(new
            {
                ok = true,
                data = data
            });
        }

        [HttpGet("clientes")]
        public async Task<IActionResult> BuscarClientes([FromQuery] string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return Ok(new
                {
                    ok = true,
                    data = new BilleteraClienteComboDTO[] { }
                });
            }

            var items = await _context.Clientes
                .Where(x =>
                    x.Persona.Nombres.Contains(q) ||
                    x.Persona.Apellido.Contains(q) ||
                    x.Persona.NroDocumento.Contains(q))
                .Select(x => new BilleteraClienteComboDTO
                {
                    Text = x.Persona.Apellido + ", " + x.Persona.Nombres,
                    Value = x.Id,
                    Subtext = x.Usuario.Mail,
                    Icon = "fa fa-user"
                })
                .Take(10)
                .ToListAsync();

            return Ok(new
            {
                ok = true,
                data = items
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BilleteraCreateRequest request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Los datos de la billetera son obligatorios."
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

            var cliente = await _context.Clientes
                .Include(c => c.Persona)
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.Id == request.ClienteId);

            if (cliente == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el cliente seleccionado."
                });
            }

            var billetera = new Billetera
            {
                Cliente = cliente,
                Saldo = request.Saldo,
                QRCobro = request.QRCobro,
                AliasCVU = request.AliasCVU,
                CVU = request.CVU
            };

            await _context.Billeteras.AddAsync(billetera);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Billetera creada correctamente.",
                data = new BilleteraDTO
                {
                    Id = billetera.Id,
                    ClienteId = cliente.Id,
                    ClienteNombre = cliente.Persona != null
                        ? cliente.Persona.Apellido + ", " + cliente.Persona.Nombres
                        : null,
                    ClienteEmail = cliente.Usuario != null ? cliente.Usuario.Email : null,
                    Saldo = billetera.Saldo,
                    QRCobro = billetera.QRCobro,
                    AliasCVU = billetera.AliasCVU,
                    CVU = billetera.CVU
                }
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] BilleteraUpdateRequest request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Los datos de la billetera son obligatorios."
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

            var billetera = await _context.Billeteras
                .FirstOrDefaultAsync(b => b.Id == id);

            if (billetera == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró la billetera."
                });
            }

            var cliente = await _context.Clientes
                .Include(c => c.Persona)
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.Id == request.ClienteId);

            if (cliente == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el cliente seleccionado."
                });
            }

            billetera.Cliente = cliente;
            billetera.Saldo = request.Saldo;
            billetera.QRCobro = request.QRCobro;
            billetera.AliasCVU = request.AliasCVU;
            billetera.CVU = request.CVU;

            _context.Billeteras.Update(billetera);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Billetera actualizada correctamente.",
                data = new BilleteraDTO
                {
                    Id = billetera.Id,
                    ClienteId = cliente.Id,
                    ClienteNombre = cliente.Persona != null
                        ? cliente.Persona.Apellido + ", " + cliente.Persona.Nombres
                        : null,
                    ClienteEmail = cliente.Usuario != null ? cliente.Usuario.Email : null,
                    Saldo = billetera.Saldo,
                    QRCobro = billetera.QRCobro,
                    AliasCVU = billetera.AliasCVU,
                    CVU = billetera.CVU
                }
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var billetera = await _context.Billeteras
                .FirstOrDefaultAsync(b => b.Id == id);

            if (billetera == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró la billetera."
                });
            }

            _context.Billeteras.Remove(billetera);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Billetera eliminada correctamente."
            });
        }
    }
}