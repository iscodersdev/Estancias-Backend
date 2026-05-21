using DAL.Data;
using DAL.DTOs;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.Endpoints
{
    [Route("endpoint/empresas")]
    [ApiController]
    public class EmpresasEndpointController : ControllerBase
    {
        private readonly EstanciasContext _context;

        public EmpresasEndpointController(EstanciasContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var empresas = await _context.Empresas
                .Select(e => new EmpresaDTO
                {
                    Id = e.Id,
                    CUIT = e.CUIT.ToString(),
                    RazonSocial = e.RazonSocial,
                    Grupo = e.Grupo != null ? e.Grupo.Nombre : null
                })
                .ToListAsync();

            return Ok(new
            {
                ok = true,
                data = empresas
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var empresa = await _context.Empresas
                .Where(e => e.Id == id)
                .Select(e => new EmpresaDetalleDTO
                {
                    Id = e.Id,
                    CUIT = e.CUIT.ToString(),
                    GrupoId = e.Grupo != null ? (int?)e.Grupo.Id : null,
                    Grupo = e.Grupo != null ? e.Grupo.Nombre : null,
                    RazonSocial = e.RazonSocial,
                    Abreviatura = e.Abreviatura,
                    Domicilio = e.Domicilio,
                    Telefono = e.Telefono,
                    Mail = e.Mail,
                    ColorFontCarnet = e.ColorFontCarnet,
                    ColorCarnet = e.ColorCarnet,
                    Twitter = e.Twitter,
                    Facebook = e.Facebook,
                    Instagram = e.Instagram,
                    WhatsApp = e.WhatsApp,
                    ColorFondo = e.ColorFondo,
                    ColorBotones = e.ColorBotones,
                    ColorLogin = e.ColorLogin
                })
                .FirstOrDefaultAsync();

            if (empresa == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró la empresa."
                });
            }

            return Ok(new
            {
                ok = true,
                data = empresa
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EmpresaCreateRequest request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Los datos de la empresa son obligatorios."
                });
            }

            if (request.CUIT <= 0)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "El CUIT es obligatorio."
                });
            }

            if (string.IsNullOrWhiteSpace(request.RazonSocial))
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "La razón social es obligatoria."
                });
            }

            if (request.GrupoId == 0)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Debe seleccionar un grupo."
                });
            }

            var grupo = await _context.Grupos
                .FirstOrDefaultAsync(g => g.Id == request.GrupoId);

            if (grupo == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el grupo seleccionado."
                });
            }

            var empresa = new Empresas
            {
                CUIT = request.CUIT,
                RazonSocial = request.RazonSocial,
                Abreviatura = request.Abreviatura,
                Domicilio = request.Domicilio,
                Telefono = request.Telefono,
                Mail = request.Mail,
                Grupo = grupo,
                ColorFontCarnet = request.ColorFontCarnet,
                ColorCarnet = request.ColorCarnet,
                Instagram = request.Instagram,
                Twitter = request.Twitter,
                Facebook = request.Facebook,
                WhatsApp = request.WhatsApp,
                ColorFondo = request.ColorFondo,
                ColorBotones = request.ColorBotones,
                ColorLogin = request.ColorLogin
            };

            await _context.Empresas.AddAsync(empresa);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Empresa creada correctamente.",
                data = new EmpresaDTO
                {
                    Id = empresa.Id,
                    CUIT = empresa.CUIT.ToString(),
                    RazonSocial = empresa.RazonSocial,
                    Grupo = empresa.Grupo != null ? empresa.Grupo.Nombre : null
                }
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EmpresaUpdateRequest request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Los datos de la empresa son obligatorios."
                });
            }

            if (request.CUIT <= 0)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "El CUIT es obligatorio."
                });
            }

            if (string.IsNullOrWhiteSpace(request.RazonSocial))
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "La razón social es obligatoria."
                });
            }

            if (request.GrupoId == 0)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Debe seleccionar un grupo."
                });
            }

            var empresa = await _context.Empresas
                .FirstOrDefaultAsync(e => e.Id == id);

            if (empresa == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró la empresa."
                });
            }

            var grupo = await _context.Grupos
                .FirstOrDefaultAsync(g => g.Id == request.GrupoId);

            if (grupo == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el grupo seleccionado."
                });
            }

            empresa.CUIT = request.CUIT;
            empresa.RazonSocial = request.RazonSocial;
            empresa.Abreviatura = request.Abreviatura;
            empresa.Domicilio = request.Domicilio;
            empresa.Telefono = request.Telefono;
            empresa.Mail = request.Mail;
            empresa.Grupo = grupo;
            empresa.ColorFontCarnet = request.ColorFontCarnet;
            empresa.ColorCarnet = request.ColorCarnet;
            empresa.ColorFondo = request.ColorFondo;
            empresa.ColorBotones = request.ColorBotones;
            empresa.Instagram = request.Instagram;
            empresa.Twitter = request.Twitter;
            empresa.Facebook = request.Facebook;
            empresa.WhatsApp = request.WhatsApp;
            empresa.ColorLogin = request.ColorLogin;

            _context.Empresas.Update(empresa);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Empresa actualizada correctamente.",
                data = new EmpresaDTO
                {
                    Id = empresa.Id,
                    CUIT = empresa.CUIT.ToString(),
                    RazonSocial = empresa.RazonSocial,
                    Grupo = empresa.Grupo != null ? empresa.Grupo.Nombre : null
                }
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var empresa = await _context.Empresas
                .FirstOrDefaultAsync(e => e.Id == id);

            if (empresa == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró la empresa."
                });
            }

            _context.Empresas.Remove(empresa);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Empresa eliminada correctamente."
            });
        }
    }
}