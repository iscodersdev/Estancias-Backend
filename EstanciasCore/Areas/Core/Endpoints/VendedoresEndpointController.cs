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
    [ApiController]
    [Route("endpoint/vendedores")]
    public class VendedoresEndpointController : ControllerBase
    {
        private readonly EstanciasContext _context;
        private readonly UserService<Usuario> _userService;

        public VendedoresEndpointController(
            EstanciasContext context,
            UserService<Usuario> userService)
        {
            _context = context;
            _userService = userService;
        }

        // GET: endpoint/vendedores
        // Replica VendedoresDataTable() pero llevado a API.
        // Agrega búsqueda y paginación para reemplazar el DataTable del MVC.
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string buscar = "",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (page < 1)
                page = 1;

            if (pageSize < 1)
                pageSize = 10;

            var query = _context.Vendedores
                .Include(x => x.Persona)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                var texto = buscar.Trim().ToLower();

                query = query.Where(v =>
                    (
                        v.Persona != null &&
                        (
                            ((v.Persona.NroDocumento ?? "").ToLower().Contains(texto)) ||
                            (((v.Persona.Apellido ?? "") + " " + (v.Persona.Nombres ?? "")).ToLower().Contains(texto)) ||
                            (((v.Persona.Nombres ?? "") + " " + (v.Persona.Apellido ?? "")).ToLower().Contains(texto))
                        )
                    ) ||
                    ((v.Domicilio ?? "").ToLower().Contains(texto)) ||
                    ((v.Telefono ?? "").ToLower().Contains(texto)) ||
                    ((v.Mail ?? "").ToLower().Contains(texto))
                );
            }

            var totalRegistros = await query.CountAsync();

            var vendedores = await query
                .OrderBy(v => v.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(v => new ListVendedoresDTO
                {
                    Id = v.Id,
                    NroDocumento = v.Persona == null ? "---" : v.Persona.NroDocumento,
                    Nombre = v.Persona == null
                        ? "---"
                        : ((v.Persona.Apellido ?? "") + " " + (v.Persona.Nombres ?? "")).Trim(),
                    Domicilio = string.IsNullOrWhiteSpace(v.Domicilio) ? "---" : v.Domicilio,
                    Telefono = string.IsNullOrWhiteSpace(v.Telefono) ? "---" : v.Telefono,
                    Mail = string.IsNullOrWhiteSpace(v.Mail) ? "---" : v.Mail
                })
                .ToListAsync();

            return Ok(new VendedoresListadoDTO
            {
                TotalRegistros = totalRegistros,
                PaginaActual = page,
                RegistrosPorPagina = pageSize,
                Vendedores = vendedores
            });
        }

        // GET: endpoint/vendedores/form-data
        // Reemplaza VendedorViewBag().
        // Sirve para llenar los combos de TipoDocumento y Pais.
        [HttpGet("form-data")]
        public async Task<IActionResult> GetFormData()
        {
            var tiposDocumento = await _context.TipoDocumento
                .OrderBy(x => x.Descripcion)
                .Select(x => new VendedorSelectDTO
                {
                    Id = x.Id,
                    Nombre = x.Descripcion
                })
                .ToListAsync();

            var paises = await _context.Paises
                .OrderBy(x => x.Nombre)
                .Select(x => new VendedorSelectDTO
                {
                    Id = x.Id,
                    Nombre = x.Nombre
                })
                .ToListAsync();

            return Ok(new VendedorFormDataDTO
            {
                TiposDocumento = tiposDocumento,
                Paises = paises
            });
        }

        // GET: endpoint/vendedores/5
        // Replica el GET _Update(int Id) y también sirve para ver detalle.
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var vendedor = await _context.Vendedores
                .Include(x => x.Persona)
                    .ThenInclude(x => x.TipoDocumento)
                .Include(x => x.Persona)
                    .ThenInclude(x => x.Pais)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (vendedor == null)
            {
                return NotFound(new VendedorResponseDTO
                {
                    Status = 404,
                    Mensaje = "No se encontró el vendedor solicitado.",
                    Vendedor = null
                });
            }

            return Ok(new VendedorResponseDTO
            {
                Status = 200,
                Mensaje = "Vendedor obtenido correctamente.",
                Vendedor = MapToVendedorDTO(vendedor)
            });
        }

        // POST: endpoint/vendedores
        // Replica la lógica del POST _Create(Vendedores vendedor)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VendedorDTO dto)
        {
            if (dto == null)
            {
                return BadRequest(new VendedorResponseDTO
                {
                    Status = 400,
                    Mensaje = "Los datos enviados son inválidos.",
                    Vendedor = null
                });
            }

            if (string.IsNullOrWhiteSpace(dto.NroDocumento))
            {
                return BadRequest(new VendedorResponseDTO
                {
                    Status = 400,
                    Mensaje = "Debe ingresar el Numero de Documento del Vendedor.",
                    Vendedor = dto
                });
            }

            var existeVendedorConDocumento = await _context.Usuarios
                .AnyAsync(x =>
                    x.Vendedores != null &&
                    x.Vendedores.Persona != null &&
                    x.Vendedores.Persona.NroDocumento == dto.NroDocumento);

            if (existeVendedorConDocumento)
            {
                return BadRequest(new VendedorResponseDTO
                {
                    Status = 400,
                    Mensaje = "Ya existe un Vendedor con el Numero de Documento Ingresado.",
                    Vendedor = dto
                });
            }

            if (dto.TipoDocumentoId == null || dto.TipoDocumentoId <= 0)
            {
                return BadRequest(new VendedorResponseDTO
                {
                    Status = 400,
                    Mensaje = "Debe seleccionar un Tipo de Documento.",
                    Vendedor = dto
                });
            }

            if (dto.PaisId == null || dto.PaisId <= 0)
            {
                return BadRequest(new VendedorResponseDTO
                {
                    Status = 400,
                    Mensaje = "Debe seleccionar un Pais.",
                    Vendedor = dto
                });
            }

            try
            {
                var tipoDocumento = await _context.TipoDocumento.FindAsync(dto.TipoDocumentoId.Value);
                var pais = await _context.Paises.FindAsync(dto.PaisId.Value);

                if (tipoDocumento == null)
                {
                    return BadRequest(new VendedorResponseDTO
                    {
                        Status = 400,
                        Mensaje = "El Tipo de Documento seleccionado no existe.",
                        Vendedor = dto
                    });
                }

                if (pais == null)
                {
                    return BadRequest(new VendedorResponseDTO
                    {
                        Status = 400,
                        Mensaje = "El Pais seleccionado no existe.",
                        Vendedor = dto
                    });
                }

                var vendedor = new Vendedores
                {
                    Persona = new Persona
                    {
                        TipoDocumento = tipoDocumento,
                        Pais = pais,
                        NroDocumento = dto.NroDocumento,
                        Cuil = dto.Cuil,
                        Apellido = dto.Apellido,
                        Nombres = dto.Nombres,
                        FechaNacimiento = dto.FechaNacimiento
                    },
                    Domicilio = dto.Domicilio,
                    Telefono = dto.Telefono,
                    Mail = dto.Mail
                };

                var usuarioVendedor = await _context.Usuarios
                    .FirstOrDefaultAsync(x => x.UserName == vendedor.Mail);

                if (usuarioVendedor == null)
                {
                    await _context.Personas.AddAsync(vendedor.Persona);
                    await _context.Vendedores.AddAsync(vendedor);

                    Usuario nuevoUsuario = new Usuario
                    {
                        UserName = vendedor.Mail,
                        Email = vendedor.Mail,
                        Mail = vendedor.Mail
                    };

                    await _userService.CreateAsync(
                        nuevoUsuario,
                        vendedor.Persona.NroDocumento.ToString());

                    nuevoUsuario.Vendedores = vendedor;
                    nuevoUsuario.Personas = vendedor.Persona;

                    _context.Usuarios.Update(nuevoUsuario);
                    await _context.SaveChangesAsync();

                    return Ok(new VendedorResponseDTO
                    {
                        Status = 200,
                        Mensaje = "Se cargo correctamente el Cliente.",
                        Vendedor = MapToVendedorDTO(vendedor)
                    });
                }
                else
                {
                    if (usuarioVendedor.Personas == null)
                    {
                        await _context.Personas.AddAsync(vendedor.Persona);
                        usuarioVendedor.Personas = vendedor.Persona;
                    }
                    else
                    {
                        usuarioVendedor.Personas.TipoDocumento = tipoDocumento;
                        usuarioVendedor.Personas.NroDocumento = vendedor.Persona.NroDocumento;
                        usuarioVendedor.Personas.Apellido = vendedor.Persona.Apellido;
                        usuarioVendedor.Personas.Nombres = vendedor.Persona.Nombres;
                        usuarioVendedor.Personas.Cuil = vendedor.Persona.Cuil;
                        usuarioVendedor.Personas.Pais = pais;
                        usuarioVendedor.Personas.FechaNacimiento = vendedor.Persona.FechaNacimiento;

                        _context.Personas.Update(usuarioVendedor.Personas);
                    }

                    if (usuarioVendedor.Vendedores == null)
                    {
                        vendedor.Persona = usuarioVendedor.Personas;
                        await _context.Vendedores.AddAsync(vendedor);
                        usuarioVendedor.Vendedores = vendedor;
                    }
                    else
                    {
                        usuarioVendedor.Vendedores.Mail = vendedor.Mail;
                        usuarioVendedor.Vendedores.Domicilio = vendedor.Domicilio;
                        usuarioVendedor.Vendedores.Telefono = vendedor.Telefono;

                        _context.Vendedores.Update(usuarioVendedor.Vendedores);
                    }

                    _context.Usuarios.Update(usuarioVendedor);
                    await _context.SaveChangesAsync();

                    return Ok(new VendedorResponseDTO
                    {
                        Status = 200,
                        Mensaje = "Se cargo correctamente el Cliente.",
                        Vendedor = MapToVendedorDTO(usuarioVendedor.Vendedores)
                    });
                }
            }
            catch (Exception)
            {
                return StatusCode(500, new VendedorResponseDTO
                {
                    Status = 500,
                    Mensaje = "Hubo un error al crear el Vendedor. Intentelo nuevamente mas tarde.",
                    Vendedor = dto
                });
            }
        }

        // PUT: endpoint/vendedores/5
        // Replica la lógica del POST _Update(Vendedores vendedor)
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] VendedorDTO dto)
        {
            if (dto == null)
            {
                return BadRequest(new VendedorResponseDTO
                {
                    Status = 400,
                    Mensaje = "Los datos enviados son inválidos.",
                    Vendedor = null
                });
            }

            if (id != dto.Id)
            {
                return BadRequest(new VendedorResponseDTO
                {
                    Status = 400,
                    Mensaje = "El Id enviado por la URL no coincide con el Id del cuerpo de la petición.",
                    Vendedor = dto
                });
            }

            if (dto.TipoDocumentoId == null || dto.TipoDocumentoId <= 0)
            {
                return BadRequest(new VendedorResponseDTO
                {
                    Status = 400,
                    Mensaje = "Debe seleccionar un Tipo de Documento.",
                    Vendedor = dto
                });
            }

            if (dto.PaisId == null || dto.PaisId <= 0)
            {
                return BadRequest(new VendedorResponseDTO
                {
                    Status = 400,
                    Mensaje = "Debe seleccionar un Pais.",
                    Vendedor = dto
                });
            }

            try
            {
                var vendedorEditar = await _context.Vendedores
                    .Include(x => x.Persona)
                    .FirstOrDefaultAsync(x => x.Id == dto.Id);

                if (vendedorEditar == null)
                {
                    return NotFound(new VendedorResponseDTO
                    {
                        Status = 404,
                        Mensaje = "Hubo un error al editar el Vendedor. Intentelo nuevamente mas tarde.",
                        Vendedor = dto
                    });
                }

                var tipoDocumento = await _context.TipoDocumento.FindAsync(dto.TipoDocumentoId.Value);
                var pais = await _context.Paises.FindAsync(dto.PaisId.Value);

                if (tipoDocumento == null)
                {
                    return BadRequest(new VendedorResponseDTO
                    {
                        Status = 400,
                        Mensaje = "El Tipo de Documento seleccionado no existe.",
                        Vendedor = dto
                    });
                }

                if (pais == null)
                {
                    return BadRequest(new VendedorResponseDTO
                    {
                        Status = 400,
                        Mensaje = "El Pais seleccionado no existe.",
                        Vendedor = dto
                    });
                }

                vendedorEditar.Domicilio = dto.Domicilio;
                vendedorEditar.Telefono = dto.Telefono;

                if (vendedorEditar.Persona == null)
                {
                    vendedorEditar.Persona = new Persona();
                }

                vendedorEditar.Persona.TipoDocumento = tipoDocumento;
                vendedorEditar.Persona.NroDocumento = dto.NroDocumento;
                vendedorEditar.Persona.Apellido = dto.Apellido;
                vendedorEditar.Persona.Nombres = dto.Nombres;
                vendedorEditar.Persona.Cuil = dto.Cuil;
                vendedorEditar.Persona.Pais = pais;
                vendedorEditar.Persona.FechaNacimiento = dto.FechaNacimiento;

                var usuarioVendedor = await _context.Usuarios
                    .FirstOrDefaultAsync(x => x.UserName == dto.Mail);

                if (usuarioVendedor == null)
                {
                    Usuario user = new Usuario
                    {
                        UserName = dto.Mail,
                        Email = dto.Mail,
                        Mail = dto.Mail
                    };

                    await _userService.CreateAsync(
                        user,
                        dto.NroDocumento.ToString());

                    usuarioVendedor = user;
                }

                usuarioVendedor.Vendedores = vendedorEditar;
                usuarioVendedor.Personas = vendedorEditar.Persona;

                _context.Usuarios.Update(usuarioVendedor);
                await _context.SaveChangesAsync();

                return Ok(new VendedorResponseDTO
                {
                    Status = 200,
                    Mensaje = "Se editó correctamente el Vendedor " + GetNombreCompleto(vendedorEditar.Persona) + ".",
                    Vendedor = MapToVendedorDTO(vendedorEditar)
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new VendedorResponseDTO
                {
                    Status = 500,
                    Mensaje = "Hubo un error al editar el Vendedor. Intentelo nuevamente mas tarde.",
                    Vendedor = dto
                });
            }
        }

        // DELETE: endpoint/vendedores/5
        // Replica Delete(int id)
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var vendedor = await _context.Vendedores
                    .Include(x => x.Persona)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (vendedor == null)
                {
                    return NotFound(new VendedorResponseDTO
                    {
                        Status = 404,
                        Mensaje = "No se encontró el vendedor a eliminar.",
                        Vendedor = null
                    });
                }

                var usuarioVendedor = await _context.Usuarios
                    .FirstOrDefaultAsync(x => x.UserName == vendedor.Mail);

                if (usuarioVendedor != null)
                {
                    usuarioVendedor.Vendedores = null;
                    _context.Usuarios.Update(usuarioVendedor);
                }

                _context.Vendedores.Remove(vendedor);
                await _context.SaveChangesAsync();

                return Ok(new VendedorResponseDTO
                {
                    Status = 200,
                    Mensaje = "Se eliminó correctamente el Vendedor.",
                    Vendedor = null
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new VendedorResponseDTO
                {
                    Status = 500,
                    Mensaje = "Hubo un error al eliminar el Vendedor.",
                    Vendedor = null
                });
            }
        }

        private VendedorDTO MapToVendedorDTO(Vendedores vendedor)
        {
            if (vendedor == null)
                return null;

            return new VendedorDTO
            {
                Id = vendedor.Id,

                TipoDocumentoId = vendedor.Persona?.TipoDocumento?.Id,
                TipoDocumento = vendedor.Persona?.TipoDocumento?.Descripcion,

                PaisId = vendedor.Persona?.Pais?.Id,
                Pais = vendedor.Persona?.Pais?.Nombre,

                NroDocumento = vendedor.Persona?.NroDocumento,
                Cuil = vendedor.Persona?.Cuil,
                Apellido = vendedor.Persona?.Apellido,
                Nombres = vendedor.Persona?.Nombres,
                FechaNacimiento = vendedor.Persona?.FechaNacimiento,

                Domicilio = vendedor.Domicilio,
                Telefono = vendedor.Telefono,
                Mail = vendedor.Mail
            };
        }

        private string GetNombreCompleto(Persona persona)
        {
            if (persona == null)
                return "---";

            return ((persona.Apellido ?? "") + " " + (persona.Nombres ?? "")).Trim();
        }
    }
}