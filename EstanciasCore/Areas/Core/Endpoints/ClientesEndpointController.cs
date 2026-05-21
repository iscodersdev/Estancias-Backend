using Commons.Identity.Services;
using DAL.Data;
using DAL.DTOs;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.Endpoints
{
    [Route("endpoint/clientes")]
    [ApiController]
    public class ClientesEndpointController : ControllerBase
    {
        private readonly EstanciasContext _context;
        private readonly UserService<Usuario> _userService;

        public ClientesEndpointController(EstanciasContext context, UserService<Usuario> userService)
        {
            _context = context;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var clientes = await _context.Clientes
                .Where(c => c.FechaBaja == null)
                .Include(c => c.TipoCliente)
                .Include(c => c.Persona)
                .Include(c => c.Empresa)
                .ToListAsync();

            var data = clientes.Select(c => new ClienteDTO
            {
                Id = c.Id,
                Tipo = c.TipoCliente != null ? c.TipoCliente.Nombre : "---",
                NombreCompleto = c.Persona != null ? c.Persona.GetNombreCompleto() : "---",
                CUIL = c.Persona != null ? c.Persona.Cuil?.ToString() : "---",
                RazonSocial = c.RazonSocial != null ? c.RazonSocial : "---",
                Empresa = c.Empresa != null ? c.Empresa.RazonSocial : "---",
                FechaIngreso = c.FechaIngreso.ToShortDateString(),
                Estado = c.ClienteValidado
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
            var cliente = await _context.Clientes
                .Include(c => c.TipoCliente)
                .Include(c => c.Persona)
                    .ThenInclude(p => p.TipoDocumento)
                .Include(c => c.Persona)
                    .ThenInclude(p => p.Pais)
                .Include(c => c.Usuario)
                .Include(c => c.Empresa)
                .Include(c => c.Provincia)
                .Include(c => c.Localidad)
                .Include(c => c.DependeDe)
                    .ThenInclude(d => d.Persona)
                .Include(c => c.Codeudor)
                    .ThenInclude(co => co.Persona)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cliente == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el cliente."
                });
            }

            return Ok(new
            {
                ok = true,
                data = MapClienteDetalle(cliente)
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ClienteCreateRequest request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Los datos del cliente son obligatorios."
                });
            }

            var validation = await ValidateClienteRequest(request, null);

            if (validation != null)
                return validation;

            var existeDocumento = await _context.Clientes
                .AnyAsync(c => c.Persona != null && c.Persona.NroDocumento == request.NroDocumento);

            if (existeDocumento)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Ya existe un cliente con el número de documento ingresado."
                });
            }

            var tipoCliente = await _context.TiposClientes.FindAsync(request.TipoClienteId);
            var tipoDocumento = await _context.TipoDocumento.FindAsync(request.TipoDocumentoId);
            var pais = await _context.Paises.FindAsync(request.PaisId);
            var empresa = await _context.Empresas.FindAsync(request.EmpresaId);

            var provincia = request.ProvinciaId.HasValue && request.ProvinciaId.Value != 0
                ? await _context.Provincia.FindAsync(request.ProvinciaId.Value)
                : null;

            var localidad = request.LocalidadId.HasValue && request.LocalidadId.Value != 0
                ? await _context.Localidad.FindAsync(request.LocalidadId.Value)
                : null;

            var dependeDe = request.DependeDeId.HasValue && request.DependeDeId.Value != 0
                ? await _context.Clientes.FindAsync(request.DependeDeId.Value)
                : null;

            var codeudor = request.CodeudorId.HasValue && request.CodeudorId.Value != 0
                ? await _context.Clientes.FindAsync(request.CodeudorId.Value)
                : null;

            var referenciaA = MapReferencia(request.ReferenciaA);
            var referenciaB = MapReferencia(request.ReferenciaB);

            var persona = new Persona
            {
                TipoDocumento = tipoDocumento,
                Pais = pais,
                NroDocumento = request.NroDocumento,
                Cuil = request.CUIL,
                Apellido = request.Apellido,
                Nombres = request.Nombres,
                FechaNacimiento = request.FechaNacimiento,
                CantidadHijos = request.CantidadHijos
            };

            var usuario = new Usuario
            {
                UserName = request.Mail,
                Email = request.Mail,
                Mail = request.Mail,
                Personas = persona
            };

            var result = await _userService.CreateAsync(usuario, request.NroDocumento);

            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "No se pudo crear el usuario del cliente.",
                    errors = result.Errors.Select(e => e.Description).ToList()
                });
            }

            var cliente = new Clientes
            {
                TipoCliente = tipoCliente,
                Persona = persona,
                Usuario = usuario,
                UsuarioId = usuario.Id,
                ReferenciaA = referenciaA,
                ReferenciaB = referenciaB,
                Empresa = empresa,
                Provincia = provincia,
                Localidad = localidad,
                DependeDe = dependeDe,
                Codeudor = codeudor,

                RazonSocial = request.RazonSocial,
                NumeroCliente = request.NumeroCliente,
                Domicilio = request.Domicilio,
                CodigoPostal = request.CodigoPostal,
                CBU = request.CBU,
                Telefono = request.Telefono,
                Celular = request.Celular,

                FechaIngreso = request.FechaIngreso == default(DateTime) ? DateTime.Now : request.FechaIngreso,
                FechaIngresoLaboral = request.FechaIngresoLaboral,
                CategoriaLaboral = request.CategoriaLaboral,
                DestinoLaboral = request.DestinoLaboral,
                NumeroLegajoLaboral = request.NumeroLegajoLaboral,
                NumeroAsociado = request.NumeroAsociado,

                PersonaPoliticamenteExpuesta = request.PersonaPoliticamenteExpuesta,
                EsMilitar = request.EsMilitar,
                RecibirPublicidad = request.RecibirPublicidad,

                ClienteValidado = true
            };

            await _context.Clientes.AddAsync(cliente);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Cliente creado correctamente.",
                data = MapClienteDetalle(cliente)
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ClienteUpdateRequest request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Los datos del cliente son obligatorios."
                });
            }

            var validation = await ValidateClienteRequest(request, id);

            if (validation != null)
                return validation;

            var cliente = await _context.Clientes
                .Include(c => c.Persona)
                .Include(c => c.Usuario)
                .Include(c => c.ReferenciaA)
                .Include(c => c.ReferenciaB)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cliente == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el cliente."
                });
            }

            var existeDocumento = await _context.Clientes
                .AnyAsync(c => c.Id != id
                    && c.Persona != null
                    && c.Persona.NroDocumento == request.NroDocumento);

            if (existeDocumento)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Ya existe otro cliente con el número de documento ingresado."
                });
            }

            var tipoCliente = await _context.TiposClientes.FindAsync(request.TipoClienteId);
            var tipoDocumento = await _context.TipoDocumento.FindAsync(request.TipoDocumentoId);
            var pais = await _context.Paises.FindAsync(request.PaisId);
            var empresa = await _context.Empresas.FindAsync(request.EmpresaId);

            var provincia = request.ProvinciaId.HasValue && request.ProvinciaId.Value != 0
                ? await _context.Provincia.FindAsync(request.ProvinciaId.Value)
                : null;

            var localidad = request.LocalidadId.HasValue && request.LocalidadId.Value != 0
                ? await _context.Localidad.FindAsync(request.LocalidadId.Value)
                : null;

            var dependeDe = request.DependeDeId.HasValue && request.DependeDeId.Value != 0
                ? await _context.Clientes.FindAsync(request.DependeDeId.Value)
                : null;

            var codeudor = request.CodeudorId.HasValue && request.CodeudorId.Value != 0
                ? await _context.Clientes.FindAsync(request.CodeudorId.Value)
                : null;

            if (cliente.ReferenciaA != null)
            {
                cliente.ReferenciaA.NombreCompleto = request.ReferenciaA != null ? request.ReferenciaA.NombreCompleto : null;
                cliente.ReferenciaA.Vinculo = request.ReferenciaA != null ? request.ReferenciaA.Vinculo : null;
                cliente.ReferenciaA.Telefono = request.ReferenciaA != null ? request.ReferenciaA.Telefono : null;
            }
            else
            {
                cliente.ReferenciaA = MapReferencia(request.ReferenciaA);
                if (cliente.ReferenciaA != null)
                    await _context.Referencias.AddAsync(cliente.ReferenciaA);
            }

            if (cliente.ReferenciaB != null)
            {
                cliente.ReferenciaB.NombreCompleto = request.ReferenciaB != null ? request.ReferenciaB.NombreCompleto : null;
                cliente.ReferenciaB.Vinculo = request.ReferenciaB != null ? request.ReferenciaB.Vinculo : null;
                cliente.ReferenciaB.Telefono = request.ReferenciaB != null ? request.ReferenciaB.Telefono : null;
            }
            else
            {
                cliente.ReferenciaB = MapReferencia(request.ReferenciaB);
                if (cliente.ReferenciaB != null)
                    await _context.Referencias.AddAsync(cliente.ReferenciaB);
            }

            cliente.TipoCliente = tipoCliente;
            cliente.Empresa = empresa;
            cliente.Provincia = provincia;
            cliente.Localidad = localidad;
            cliente.DependeDe = dependeDe;
            cliente.Codeudor = codeudor;

            cliente.RazonSocial = request.RazonSocial;
            cliente.NumeroCliente = request.NumeroCliente;
            cliente.Domicilio = request.Domicilio;
            cliente.CodigoPostal = request.CodigoPostal;
            cliente.CBU = request.CBU;
            cliente.Telefono = request.Telefono;
            cliente.Celular = request.Celular;

            cliente.FechaIngreso = request.FechaIngreso == default(DateTime) ? cliente.FechaIngreso : request.FechaIngreso;
            cliente.FechaIngresoLaboral = request.FechaIngresoLaboral;
            cliente.CategoriaLaboral = request.CategoriaLaboral;
            cliente.DestinoLaboral = request.DestinoLaboral;
            cliente.NumeroLegajoLaboral = request.NumeroLegajoLaboral;
            cliente.NumeroAsociado = request.NumeroAsociado;

            cliente.PersonaPoliticamenteExpuesta = request.PersonaPoliticamenteExpuesta;
            cliente.EsMilitar = request.EsMilitar;
            cliente.RecibirPublicidad = request.RecibirPublicidad;
            cliente.ClienteValidado = true;

            cliente.Persona.TipoDocumento = tipoDocumento;
            cliente.Persona.Pais = pais;
            cliente.Persona.NroDocumento = request.NroDocumento;
            cliente.Persona.Cuil = request.CUIL;
            cliente.Persona.Apellido = request.Apellido;
            cliente.Persona.Nombres = request.Nombres;
            cliente.Persona.FechaNacimiento = request.FechaNacimiento;
            cliente.Persona.CantidadHijos = request.CantidadHijos;

            if (cliente.Usuario != null)
            {
                cliente.Usuario.Mail = request.Mail;
                cliente.Usuario.Email = request.Mail;
                cliente.Usuario.UserName = request.Mail;
                cliente.Usuario.Personas = cliente.Persona;
                _context.Usuarios.Update(cliente.Usuario);
            }

            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Cliente actualizado correctamente.",
                data = MapClienteDetalle(cliente)
            });
        }

        [HttpGet("combo")]
        public async Task<IActionResult> ClienteCombo([FromQuery] string term, [FromQuery] int? id)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return Ok(new
                {
                    ok = true,
                    data = new ClienteComboDTO[] { }
                });
            }

            term = term.ToUpper();

            var query = _context.Clientes
                .Where(c => c.FechaBaja == null)
                .Where(c =>
                    (c.Persona.Nombres.ToUpper() + " " + c.Persona.Apellido.ToUpper())
                    .Contains(term));

            if (id != null)
                query = query.Where(c => c.Id != id.Value);

            var data = await query
                .Select(c => new ClienteComboDTO
                {
                    text = c.Persona.Nombres.ToUpper() + " " + c.Persona.Apellido.ToUpper(),
                    id = c.Id
                })
                .Take(30)
                .ToListAsync();

            return Ok(new
            {
                ok = true,
                data = data
            });
        }

        [HttpGet("usuarios-combo")]
        public async Task<IActionResult> UsuarioCombo([FromQuery] string term, [FromQuery] string id)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return Ok(new
                {
                    ok = true,
                    data = new ClienteComboDTO[] { }
                });
            }

            term = term.ToUpper();

            var query = _context.Usuarios
                .Where(u =>
                    (u.Clientes.Persona.Nombres.ToUpper() + " " + u.Clientes.Persona.Apellido.ToUpper())
                    .Contains(term));

            if (!string.IsNullOrWhiteSpace(id))
                query = query.Where(u => u.Id != id);

            var data = await query
                .Select(u => new ClienteComboDTO
                {
                    text = u.Clientes.Persona.Nombres.ToUpper() + " " + u.Clientes.Persona.Apellido.ToUpper(),
                    id = u.Clientes.Id
                })
                .Take(30)
                .ToListAsync();

            return Ok(new
            {
                ok = true,
                data = data
            });
        }

        [HttpGet("localidades")]
        public async Task<IActionResult> GetLocalidadesPorProvincia([FromQuery] int idProvincia)
        {
            var localidades = await _context.Localidad
                .Where(l => l.IdProvincia == idProvincia)
                .Select(l => new LocalidadSelectDTO
                {
                    Id = l.Id,
                    Descripcion = l.Descripcion
                })
                .ToListAsync();

            return Ok(new
            {
                ok = true,
                data = localidades
            });
        }

        [HttpPatch("{id}/validar")]
        public async Task<IActionResult> ValidarCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el cliente."
                });
            }

            cliente.ClienteValidado = true;

            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Cliente validado correctamente."
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cliente == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el cliente."
                });
            }

            cliente.FechaBaja = DateTime.Now;

            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                message = "Cliente dado de baja correctamente."
            });
        }

        private async Task<IActionResult> ValidateClienteRequest(ClienteCreateRequest request, int? clienteId)
        {
            if (string.IsNullOrWhiteSpace(request.NroDocumento))
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Debe ingresar el número de documento del cliente."
                });
            }

            if (request.TipoClienteId == 0)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Debe seleccionar un tipo de cliente."
                });
            }

            if (request.TipoDocumentoId == 0)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Debe seleccionar un tipo de documento."
                });
            }

            if (request.PaisId == 0)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Debe seleccionar un país."
                });
            }

            if (request.EmpresaId == 0)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Debe seleccionar una empresa."
                });
            }

            if (string.IsNullOrWhiteSpace(request.Mail))
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Debe ingresar el mail del cliente."
                });
            }

            if (await _context.TiposClientes.FindAsync(request.TipoClienteId) == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el tipo de cliente seleccionado."
                });
            }

            if (await _context.TipoDocumento.FindAsync(request.TipoDocumentoId) == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el tipo de documento seleccionado."
                });
            }

            if (await _context.Paises.FindAsync(request.PaisId) == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el país seleccionado."
                });
            }

            if (await _context.Empresas.FindAsync(request.EmpresaId) == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró la empresa seleccionada."
                });
            }

            return null;
        }

        private Referencia MapReferencia(ReferenciaRequest request)
        {
            if (request == null)
                return null;

            if (string.IsNullOrWhiteSpace(request.NombreCompleto)
                && string.IsNullOrWhiteSpace(request.Vinculo)
                && string.IsNullOrWhiteSpace(request.Telefono))
                return null;

            return new Referencia
            {
                NombreCompleto = request.NombreCompleto,
                Vinculo = request.Vinculo,
                Telefono = request.Telefono
            };
        }

        private ClienteDetalleDTO MapClienteDetalle(Clientes cliente)
        {
            return new ClienteDetalleDTO
            {
                Id = cliente.Id,

                TipoClienteId = cliente.TipoCliente != null ? (int?)cliente.TipoCliente.Id : null,
                TipoClienteNombre = cliente.TipoCliente != null ? cliente.TipoCliente.Nombre : null,

                PersonaId = cliente.Persona != null ? (int?)cliente.Persona.Id : null,
                NombreCompleto = cliente.Persona != null ? cliente.Persona.GetNombreCompleto() : null,
                Apellido = cliente.Persona != null ? cliente.Persona.Apellido : null,
                Nombres = cliente.Persona != null ? cliente.Persona.Nombres : null,
                NroDocumento = cliente.Persona != null ? cliente.Persona.NroDocumento?.ToString() : null,
                CUIL = cliente.Persona != null ? cliente.Persona.Cuil?.ToString() : null,

                UsuarioId = cliente.UsuarioId,
                UsuarioEmail = cliente.Usuario != null ? cliente.Usuario.Email : null,

                EmpresaId = cliente.Empresa != null ? (int?)cliente.Empresa.Id : null,
                Empresa = cliente.Empresa != null ? cliente.Empresa.RazonSocial : null,

                RazonSocial = cliente.RazonSocial,
                NumeroCliente = cliente.NumeroCliente,
                Domicilio = cliente.Domicilio,

                ProvinciaId = cliente.Provincia != null ? (int?)cliente.Provincia.Id : null,
                Provincia = cliente.Provincia != null ? cliente.Provincia.Descripcion : null,

                LocalidadId = cliente.Localidad != null ? (int?)cliente.Localidad.Id : null,
                Localidad = cliente.Localidad != null ? cliente.Localidad.Descripcion : null,

                CodigoPostal = cliente.CodigoPostal,
                CBU = cliente.CBU,
                Telefono = cliente.Telefono,
                Celular = cliente.Celular,

                FechaIngresoLaboral = cliente.FechaIngresoLaboral,
                NumeroLegajoLaboral = cliente.NumeroLegajoLaboral,
                CategoriaLaboral = cliente.CategoriaLaboral,
                DestinoLaboral = cliente.DestinoLaboral,
                NumeroAsociado = cliente.NumeroAsociado,

                DependeDeId = cliente.DependeDe != null ? (int?)cliente.DependeDe.Id : null,
                DependeDeNombre = cliente.DependeDe != null && cliente.DependeDe.Persona != null
                    ? cliente.DependeDe.Persona.Nombres + " " + cliente.DependeDe.Persona.Apellido
                    : null,

                CodeudorId = cliente.Codeudor != null ? (int?)cliente.Codeudor.Id : null,
                CodeudorNombre = cliente.Codeudor != null && cliente.Codeudor.Persona != null
                    ? cliente.Codeudor.Persona.Nombres + " " + cliente.Codeudor.Persona.Apellido
                    : null,

                PersonaPoliticamenteExpuesta = cliente.PersonaPoliticamenteExpuesta,
                EsMilitar = cliente.EsMilitar,
                FechaIngreso = cliente.FechaIngreso,
                FechaBaja = cliente.FechaBaja,
                ClienteValidado = cliente.ClienteValidado,
                RecibirPublicidad = cliente.RecibirPublicidad,
                NroDocReferido = cliente.NroDocReferido,
                RegistroMobile = cliente.RegistroMobile,

                TieneFotoDNIAnverso = cliente.FotoDNIAnverso != null,
                TieneFotoDNIReverso = cliente.FotoDNIReverso != null,
                TieneFotoSosteniendoDNI = cliente.FotoSosteniendoDNI != null,
                TieneLegajoElectronico = cliente.LegajoElectronico != null,
                TieneFirmaOlografica = cliente.FirmaOlografica != null,
                TieneFirmaOlograficaConfirmacion = cliente.FirmaOlograficaConfirmacion != null
            };
        }
    }
}