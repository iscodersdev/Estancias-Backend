using Commons.Identity.Services;
using DAL.Data;
using DAL.DTOs;
using DAL.Models;
using EstanciasCore.API.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.Endpoints
{
    [TypeFilter(typeof(EndpointUatAuthorizeAttribute))]
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

        // GET: endpoint/clientes
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var clientes = await _context.Clientes
                .Where(x => x.FechaBaja == null)
                .Include(x => x.TipoCliente)
                .Include(x => x.Persona)
                .Include(x => x.Empresa)
                .ToListAsync();

            var data = clientes.Select(p => new ClienteDTO
            {
                Id = p.Id,
                Tipo = p.TipoCliente != null ? p.TipoCliente.Nombre : "---",
                NombreCompleto = p.Persona != null ? p.Persona.GetNombreCompleto() : "---",
                CUIL = p.Persona != null ? p.Persona.Cuil?.ToString() : "---",
                RazonSocial = p.RazonSocial != null ? p.RazonSocial : "---",
                Empresa = p.Empresa != null ? p.Empresa.RazonSocial : "---",
                FechaIngreso = p.FechaIngreso.ToShortDateString(),
                Estado = p.ClienteValidado
            }).ToList();

            return Ok(new
            {
                ok = true,
                data = data
            });
        }

        // POST: endpoint/clientes/exportar-excel
        [HttpPost("exportar-excel")]
        public async Task<IActionResult> ExportarExcel([FromBody] List<int> ids)
        {
            try
            {
                var query = _context.Clientes
                    .AsNoTracking()
                    .Where(x => x.FechaBaja == null)
                    .AsQueryable();

                if (ids != null && ids.Any())
                {
                    query = query.Where(x => ids.Contains(x.Id));
                }

                var clientesRaw = await query
                    .OrderBy(x => x.Persona != null ? x.Persona.Apellido : "")
                    .ThenBy(x => x.Persona != null ? x.Persona.Nombres : "")
                    .Select(x => new
                    {
                        x.Id,

                        TipoCliente = x.TipoCliente != null
                            ? x.TipoCliente.Nombre
                            : "---",

                        Apellido = x.Persona != null
                            ? x.Persona.Apellido
                            : "",

                        Nombres = x.Persona != null
                            ? x.Persona.Nombres
                            : "",

                        CUIL = x.Persona != null
                            ? x.Persona.Cuil
                            : null,

                        RazonSocial = x.RazonSocial,

                        Empresa = x.Empresa != null
                            ? x.Empresa.RazonSocial
                            : "---",

                        x.FechaIngreso,
                        x.ClienteValidado
                    })
                    .ToListAsync();

                if (!clientesRaw.Any())
                {
                    return NotFound(new
                    {
                        ok = false,
                        message = "No se encontraron clientes para exportar."
                    });
                }

                var clientesExcel = clientesRaw.Select(x => new ClienteExcelExportDTO
                {
                    TipoCliente = x.TipoCliente,
                    NombreCompleto = ((x.Apellido ?? "") + " " + (x.Nombres ?? "")).Trim(),
                    CUIL = x.CUIL != null ? x.CUIL.ToString() : "---",
                    RazonSocial = !string.IsNullOrWhiteSpace(x.RazonSocial) ? x.RazonSocial : "---",
                    Empresa = x.Empresa,
                    FechaIngreso = x.FechaIngreso != DateTime.MinValue ? x.FechaIngreso.ToString("dd/MM/yyyy") : "",
                    Estado = x.ClienteValidado ? "Validado" : "Pendiente"
                }).ToList();

                var excelBytes = GenerateClientesXlsxBytes(clientesExcel);
                var excelName = $"Clientes_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

                return File(
                    excelBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    excelName
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    ok = false,
                    message = "Hubo un error al exportar el Excel de clientes.",
                    error = ex.Message,
                    inner = ex.InnerException != null ? ex.InnerException.Message : null
                });
            }
        }

        private byte[] GenerateClientesXlsxBytes(List<ClienteExcelExportDTO> datos)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Clientes");

                worksheet.Cells.LoadFromCollection(datos, true);

                worksheet.Cells["A1"].Value = "Tipo Cliente";
                worksheet.Cells["B1"].Value = "Nombre Completo";
                worksheet.Cells["C1"].Value = "CUIL";
                worksheet.Cells["D1"].Value = "Razón Social";
                worksheet.Cells["E1"].Value = "Empresa";
                worksheet.Cells["F1"].Value = "Fecha Ingreso";
                worksheet.Cells["G1"].Value = "Estado";

                worksheet.Row(1).Style.Font.Bold = true;

                worksheet.Column(1).Width = 20;
                worksheet.Column(2).Width = 35;
                worksheet.Column(3).Width = 18;
                worksheet.Column(4).Width = 35;
                worksheet.Column(5).Width = 35;
                worksheet.Column(6).Width = 18;
                worksheet.Column(7).Width = 15;

                worksheet.Column(3).Style.Numberformat.Format = "@";

                return package.GetAsByteArray();
            }
        }

        // GET: endpoint/clientes/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var cliente = await _context.Clientes
                .Include(x => x.TipoCliente)
                .Include(x => x.Persona)
                    .ThenInclude(x => x.TipoDocumento)
                .Include(x => x.Persona)
                    .ThenInclude(x => x.Pais)
                .Include(x => x.Usuario)
                .Include(x => x.Empresa)
                .Include(x => x.Provincia)
                .Include(x => x.Localidad)
                .Include(x => x.DependeDe)
                    .ThenInclude(x => x.Persona)
                .Include(x => x.Codeudor)
                    .ThenInclude(x => x.Persona)
                .Include(x => x.ReferenciaA)
                .Include(x => x.ReferenciaB)
                .FirstOrDefaultAsync(x => x.Id == id);

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

        // POST: endpoint/clientes
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ClienteCreateRequest request)
        {
            try
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

                if (request.NroDocumento == null)
                {
                    return BadRequest(new
                    {
                        ok = false,
                        message = "Debe ingresar el Numero de Documento del Cliente."
                    });
                }

                var usuarioCliente = await _context.Usuarios
                    .Where(x => x.Clientes.Persona.NroDocumento == request.NroDocumento.ToString())
                    .FirstOrDefaultAsync();

                if (usuarioCliente != null)
                {
                    return BadRequest(new
                    {
                        ok = false,
                        message = "Ya existe un Cliente con el Numero de Documento Ingresado"
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

                if (referenciaA != null)
                    await _context.Referencias.AddAsync(referenciaA);

                if (referenciaB != null)
                    await _context.Referencias.AddAsync(referenciaB);

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
                    Mail = request.Mail
                };

                var result = await _userService.CreateAsync(usuario, request.NroDocumento.ToString());

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

                    FechaIngreso = request.FechaIngreso,
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

                await _context.Personas.AddAsync(persona);
                await _context.Clientes.AddAsync(cliente);

                var user = await _context.Usuarios.FindAsync(cliente.UsuarioId);
                if (user != null)
                {
                    user.Personas = persona;
                    _context.Usuarios.Update(user);
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    ok = true,
                    message = "Se cargo correctamente el Cliente.",
                    data = MapClienteDetalle(cliente)
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    ok = false,
                    message = "Hubo un error al cargar el Cliente. Intentelo nuevamente mas tarde."
                });
            }
        }

        // PUT: endpoint/clientes/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ClienteUpdateRequest request)
        {
            try
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

                var clienteEditar = await _context.Clientes
                    .Include(x => x.Persona)
                    .Include(x => x.Usuario)
                    .Include(x => x.ReferenciaA)
                    .Include(x => x.ReferenciaB)
                    .Include(x => x.DependeDe)
                    .Include(x => x.Codeudor)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (clienteEditar == null)
                {
                    return NotFound(new
                    {
                        ok = false,
                        message = "No se encontró el cliente."
                    });
                }

                if (clienteEditar.ReferenciaA != null)
                {
                    clienteEditar.ReferenciaA.NombreCompleto = request.ReferenciaA != null ? request.ReferenciaA.NombreCompleto : null;
                    clienteEditar.ReferenciaA.Vinculo = request.ReferenciaA != null ? request.ReferenciaA.Vinculo : null;
                    clienteEditar.ReferenciaA.Telefono = request.ReferenciaA != null ? request.ReferenciaA.Telefono : null;
                }
                else
                {
                    clienteEditar.ReferenciaA = MapReferencia(request.ReferenciaA);
                    if (clienteEditar.ReferenciaA != null)
                        await _context.Referencias.AddAsync(clienteEditar.ReferenciaA);
                }

                if (clienteEditar.ReferenciaB != null)
                {
                    clienteEditar.ReferenciaB.NombreCompleto = request.ReferenciaB != null ? request.ReferenciaB.NombreCompleto : null;
                    clienteEditar.ReferenciaB.Vinculo = request.ReferenciaB != null ? request.ReferenciaB.Vinculo : null;
                    clienteEditar.ReferenciaB.Telefono = request.ReferenciaB != null ? request.ReferenciaB.Telefono : null;
                }
                else
                {
                    clienteEditar.ReferenciaB = MapReferencia(request.ReferenciaB);
                    if (clienteEditar.ReferenciaB != null)
                        await _context.Referencias.AddAsync(clienteEditar.ReferenciaB);
                }

                if (request.ProvinciaId.HasValue && request.ProvinciaId.Value != 0)
                    clienteEditar.Provincia = await _context.Provincia.FindAsync(request.ProvinciaId.Value);
                else
                    clienteEditar.Provincia = null;

                if (request.LocalidadId.HasValue && request.LocalidadId.Value != 0)
                    clienteEditar.Localidad = await _context.Localidad.FindAsync(request.LocalidadId.Value);
                else
                    clienteEditar.Localidad = null;

                clienteEditar.RazonSocial = request.RazonSocial;
                clienteEditar.NumeroCliente = request.NumeroCliente;
                clienteEditar.Domicilio = request.Domicilio;
                clienteEditar.CodigoPostal = request.CodigoPostal;
                clienteEditar.CBU = request.CBU;
                clienteEditar.Telefono = request.Telefono;
                clienteEditar.Celular = request.Celular;
                clienteEditar.FechaIngresoLaboral = request.FechaIngresoLaboral;
                clienteEditar.NumeroLegajoLaboral = request.NumeroLegajoLaboral;
                clienteEditar.CategoriaLaboral = request.CategoriaLaboral;
                clienteEditar.DestinoLaboral = request.DestinoLaboral;
                clienteEditar.NumeroAsociado = request.NumeroAsociado;
                clienteEditar.PersonaPoliticamenteExpuesta = request.PersonaPoliticamenteExpuesta;
                clienteEditar.EsMilitar = request.EsMilitar;
                clienteEditar.FechaIngreso = request.FechaIngreso;
                clienteEditar.ClienteValidado = true;
                clienteEditar.RecibirPublicidad = request.RecibirPublicidad;

                if (request.DependeDeId.HasValue && request.DependeDeId.Value != 0)
                    clienteEditar.DependeDe = await _context.Clientes.FindAsync(request.DependeDeId.Value);
                else
                    clienteEditar.DependeDe = null;

                if (request.CodeudorId.HasValue && request.CodeudorId.Value != 0)
                    clienteEditar.Codeudor = await _context.Clientes.FindAsync(request.CodeudorId.Value);
                else
                    clienteEditar.Codeudor = null;

                clienteEditar.TipoCliente = await _context.TiposClientes.FindAsync(request.TipoClienteId);
                clienteEditar.Empresa = await _context.Empresas.FindAsync(request.EmpresaId);

                clienteEditar.Persona.TipoDocumento = await _context.TipoDocumento.FindAsync(request.TipoDocumentoId);
                clienteEditar.Persona.Pais = await _context.Paises.FindAsync(request.PaisId);
                clienteEditar.Persona.NroDocumento = request.NroDocumento;
                clienteEditar.Persona.Cuil = request.CUIL;
                clienteEditar.Persona.Apellido = request.Apellido;
                clienteEditar.Persona.Nombres = request.Nombres;
                clienteEditar.Persona.FechaNacimiento = request.FechaNacimiento;
                clienteEditar.Persona.CantidadHijos = request.CantidadHijos;

                if (clienteEditar.Usuario != null)
                {
                    clienteEditar.Usuario.Mail = request.Mail;
                    clienteEditar.Usuario.Email = request.Mail;
                    clienteEditar.Usuario.UserName = request.Mail;
                    clienteEditar.Usuario.Personas = clienteEditar.Persona;
                    _context.Usuarios.Update(clienteEditar.Usuario);
                }

                _context.Clientes.Update(clienteEditar);

                var user = await _context.Usuarios.FindAsync(clienteEditar.UsuarioId);
                if (user != null)
                {
                    user.Personas = clienteEditar.Persona;
                    _context.Usuarios.Update(user);
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    ok = true,
                    message = "Se modifico corretamente el Cliente.",
                    data = MapClienteDetalle(clienteEditar)
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    ok = false,
                    message = "Hubo un error al modificar el Cliente. Intentelo nuevamente mas tarde."
                });
            }
        }

        // POST: endpoint/clientes/{id}/foto
        [HttpPost("{id}/foto")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CargarFotoCliente(int id, [FromForm] IFormFile FotoCliente)
        {
            try
            {
                var clienteEdit = await _context.Clientes
                    .Include(x => x.Persona)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (clienteEdit == null)
                {
                    return NotFound(new
                    {
                        ok = false,
                        message = "No se encontró el cliente."
                    });
                }

                if (clienteEdit.Persona == null)
                {
                    return BadRequest(new
                    {
                        ok = false,
                        message = "El cliente no tiene una persona asociada."
                    });
                }

                if (FotoCliente != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await FotoCliente.CopyToAsync(memoryStream);
                        clienteEdit.Persona.Foto = memoryStream.ToArray();
                    }
                }

                _context.Clientes.Update(clienteEdit);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    ok = true,
                    message = "Se cargo correctamente la Foto del Cliente."
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    ok = false,
                    message = "Hubo un error al cargar la Foto del Cliente. Intentelo nuevamente mas tarde."
                });
            }
        }

        // GET: endpoint/clientes/{id}/foto
        [HttpGet("{id}/foto")]
        public async Task<IActionResult> ObtenerFotoCliente(int id)
        {
            var cliente = await _context.Clientes
                .Include(x => x.Persona)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (cliente == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el cliente."
                });
            }

            if (cliente.Persona == null || cliente.Persona.Foto == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "El cliente no tiene foto cargada."
                });
            }

            return Ok(new
            {
                ok = true,
                data = new
                {
                    clienteId = cliente.Id,
                    fotoBase64 = Convert.ToBase64String(cliente.Persona.Foto)
                }
            });
        }

        // DELETE: endpoint/clientes/{id}
        // Respeta el controller original: elimina físicamente, no hace FechaBaja.
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var cliente = await _context.Clientes
                    .Include(x => x.Usuario)
                    .Include(x => x.ReferenciaA)
                    .Include(x => x.ReferenciaB)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (cliente == null)
                {
                    return NotFound(new
                    {
                        ok = false,
                        message = "No se encontró el cliente."
                    });
                }

                if (cliente.Usuario != null)
                {
                    cliente.Usuario.Clientes = null;
                    cliente.Usuario = null;
                }

                cliente.ReferenciaA = null;
                cliente.ReferenciaB = null;

                _context.Clientes.Remove(cliente);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    ok = true,
                    message = "Se dio de Baja correctamente al Cliente."
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    ok = false,
                    message = "Hubo un error al dar de baja al Cliente."
                });
            }
        }

        // GET: endpoint/clientes/combo?term=juan&id=5
        [HttpGet("combo")]
        public async Task<IActionResult> ClienteCombo([FromQuery] string term, [FromQuery] int? id)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return Ok(new
                {
                    ok = true,
                    data = new object[] { }
                });
            }

            term = term.ToUpper();

            var items = _context.Clientes
                .Where(x => x.FechaBaja == null)
                .Where(x =>
                    (x.Persona.Nombres.ToUpper() + " " + x.Persona.Apellido.ToUpper())
                    .Contains(term.ToUpper()));

            if (id != null)
                items = items.Where(x => x.Id != id.Value);

            var list = await items
                .Select(x => new
                {
                    text = x.Persona.Nombres.ToUpper() + " " + x.Persona.Apellido.ToUpper(),
                    id = x.Id
                })
                .Take(30)
                .ToListAsync();

            return Ok(new
            {
                ok = true,
                data = list
            });
        }

        // GET: endpoint/clientes/usuarios-combo?term=juan&id=usuarioId
        [HttpGet("usuarios-combo")]
        public async Task<IActionResult> UsuarioCombo([FromQuery] string term, [FromQuery] string id)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return Ok(new
                {
                    ok = true,
                    data = new object[] { }
                });
            }

            term = term.ToUpper();

            var items = _context.Usuarios
                .Where(x =>
                    (x.Clientes.Persona.Nombres.ToUpper() + " " + x.Clientes.Persona.Apellido.ToUpper())
                    .Contains(term.ToUpper()));

            if (id != null)
                items = items.Where(x => x.Id != id);

            var list = await items
                .Select(x => new
                {
                    text = x.Clientes.Persona.Nombres.ToUpper() + " " + x.Clientes.Persona.Apellido.ToUpper(),
                    id = x.Id
                })
                .Take(30)
                .ToListAsync();

            return Ok(new
            {
                ok = true,
                data = list
            });
        }

        // POST: endpoint/clientes/validar
        [HttpPost("validar")]
        public async Task<IActionResult> ValdiarCliente([FromBody] ValidarClienteRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.ValdiarClienteId))
                {
                    return BadRequest(new
                    {
                        ok = false,
                        message = "Debe enviar el Id del cliente a validar."
                    });
                }

                var cliente = await _context.Clientes.FindAsync(Convert.ToInt32(request.ValdiarClienteId));

                if (cliente == null)
                {
                    return NotFound(new
                    {
                        ok = false,
                        message = "No se encontró el cliente."
                    });
                }

                cliente.ClienteValidado = true;
                _context.Update(cliente);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    ok = true,
                    message = "Se Valido el Cliente Correctamente."
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    ok = false,
                    message = "Hubo un error al Validar el Cliente. Intentelo nuevamente mas tarde."
                });
            }
        }

        // PATCH: endpoint/clientes/{id}/validar
        // Versión API práctica del mismo validar.
        [HttpPatch("{id}/validar")]
        public async Task<IActionResult> ValidarClientePorId(int id)
        {
            try
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
                _context.Update(cliente);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    ok = true,
                    message = "Se Valido el Cliente Correctamente."
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    ok = false,
                    message = "Hubo un error al Validar el Cliente. Intentelo nuevamente mas tarde."
                });
            }
        }

        // POST: endpoint/clientes/select-localidades?id=1
        // Respeta el controller original: devuelve string HTML con options.
        [HttpPost("select-localidades")]
        public async Task<IActionResult> SelectLocalidades([FromQuery] int id)
        {
            string array = "";

            var localidad = await _context.Localidad
                .Where(x => x.IdProvincia == id)
                .ToListAsync();

            foreach (var loc in localidad)
            {
                array += "<option value='" + loc.Id + "'>" + loc.Descripcion + "</option>";
            }

            return Ok(array);
        }

        // GET: endpoint/clientes/localidades?idProvincia=1
        // Versión JSON útil para API.
        [HttpGet("localidades")]
        public async Task<IActionResult> GetLocalidadesPorProvincia([FromQuery] int idProvincia)
        {
            var localidades = await _context.Localidad
                .Where(x => x.IdProvincia == idProvincia)
                .Select(x => new LocalidadSelectDTO
                {
                    Id = x.Id,
                    Descripcion = x.Descripcion
                })
                .ToListAsync();

            return Ok(new
            {
                ok = true,
                data = localidades
            });
        }

        // GET: endpoint/clientes/form-data
        // Equivalente API del ClienteViewBag.
        [HttpGet("form-data")]
        public async Task<IActionResult> GetFormData([FromQuery] int? clienteId = null)
        {
            var tiposClientes = await _context.TiposClientes
                .Select(x => new
                {
                    text = x.Nombre,
                    value = x.Id.ToString()
                })
                .ToListAsync();

            var tiposDocumento = await _context.TipoDocumento
                .Select(x => new
                {
                    text = x.Descripcion,
                    value = x.Id.ToString()
                })
                .ToListAsync();

            var paises = await _context.Paises
                .Select(x => new
                {
                    text = x.Nombre,
                    value = x.Id.ToString()
                })
                .ToListAsync();

            var empresas = await _context.Empresas
                .Select(x => new
                {
                    text = x.RazonSocial,
                    value = x.Id.ToString()
                })
                .ToListAsync();

            var provincias = await _context.Provincia.ToListAsync();

            var provinciasData = provincias
                .Select(x => new
                {
                    text = x.Descripcion,
                    value = x.Id.ToString()
                })
                .ToList();

            IQueryable<Localidad> localidadesQuery;

            if (provincias != null && provincias.Count > 0)
                localidadesQuery = _context.Localidad.Where(x => x.IdProvincia == provincias.First().Id);
            else
                localidadesQuery = _context.Localidad;

            string codeudorId = "";
            string codeudorDescripcion = "";
            string dependeDeId = "";
            string dependeDeDescripcion = "";

            if (clienteId.HasValue)
            {
                var cliente = await _context.Clientes
                    .Include(x => x.Provincia)
                    .Include(x => x.Codeudor)
                        .ThenInclude(x => x.Persona)
                    .Include(x => x.DependeDe)
                        .ThenInclude(x => x.Persona)
                    .FirstOrDefaultAsync(x => x.Id == clienteId.Value);

                if (cliente != null)
                {
                    if (cliente.Provincia != null)
                        localidadesQuery = _context.Localidad.Where(x => x.IdProvincia == cliente.Provincia.Id);

                    codeudorId = cliente.Codeudor != null ? cliente.Codeudor.Id.ToString() : "";
                    codeudorDescripcion = cliente.Codeudor != null
                        ? cliente.Codeudor.Persona?.Nombres?.ToUpper() + " " + cliente.Codeudor.Persona?.Apellido?.ToUpper()
                        : "";

                    dependeDeId = cliente.DependeDe != null ? cliente.DependeDe.Id.ToString() : "";
                    dependeDeDescripcion = cliente.DependeDe != null
                        ? cliente.DependeDe.Persona.Nombres?.ToUpper() + " " + cliente.DependeDe.Persona.Apellido?.ToUpper()
                        : "";
                }
            }

            var localidades = await localidadesQuery
                .Select(x => new
                {
                    text = x.Descripcion,
                    value = x.Id.ToString()
                })
                .ToListAsync();

            return Ok(new
            {
                ok = true,
                data = new
                {
                    tiposClientes,
                    tiposDocumento,
                    paises,
                    empresas,
                    provincias = provinciasData,
                    localidades,
                    codeudorId,
                    codeudorDescripcion,
                    dependeDeId,
                    dependeDeDescripcion
                }
            });
        }

        private async Task<IActionResult> ValidateClienteRequest(ClienteCreateRequest request, int? clienteId)
        {
            if (string.IsNullOrWhiteSpace(request.NroDocumento))
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Debe ingresar el Numero de Documento del Cliente."
                });
            }

            if (request.TipoClienteId == 0)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Debe seleccionar un Tipo de Cliente"
                });
            }

            if (request.TipoDocumentoId == 0)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Debe seleccionar un Tipo de Documento"
                });
            }

            if (request.PaisId == 0)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Debe seleccionar un Pais"
                });
            }

            if (request.EmpresaId == 0)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Debe seleccionar una Empresa"
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

    public class ValidarClienteRequest
    {
        public string ValdiarClienteId { get; set; }
    }
}

public class ClienteExcelExportDTO
{
    public string TipoCliente { get; set; }
    public string NombreCompleto { get; set; }
    public string CUIL { get; set; }
    public string RazonSocial { get; set; }
    public string Empresa { get; set; }
    public string FechaIngreso { get; set; }
    public string Estado { get; set; }
}