using DAL.Data;
using DAL.DTOs;
using DAL.Models;
using EstanciasCore.API.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.Endpoints
{
    [TypeFilter(typeof(EndpointUatAuthorizeAttribute))]
    [Route("endpoint/menu-mobile")]
    [ApiController]
    public class MenuMobileEndpointController : ControllerBase
    {
        private readonly EstanciasContext _context;

        public MenuMobileEndpointController(EstanciasContext context)
        {
            _context = context;
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var opcion = await _context.MenuMobile
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (opcion == null)
                {
                    return NotFound(new
                    {
                        Mensaje = "No se encontró la opción del menú mobile."
                    });
                }

                var asignaciones = await _context.MenuMobileUsuariosHabilitados
                    .Where(x => x.MenuMobile.Id == id)
                    .ToListAsync();

                if (asignaciones.Any())
                {
                    _context.MenuMobileUsuariosHabilitados.RemoveRange(asignaciones);
                }

                _context.MenuMobile.Remove(opcion);

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Mensaje = "La opción del menú mobile se eliminó correctamente.",
                    OpcionEliminada = new
                    {
                        opcion.Id,
                        opcion.Nombre,
                        CantidadAsignacionesEliminadas = asignaciones.Count
                    }
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    Mensaje = "Hubo un error al eliminar la opción del menú mobile."
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var opciones = await _context.MenuMobile
                    .AsNoTracking()
                    .OrderBy(x => x.Nombre)
                    .Select(x => new
                    {
                        x.Id,
                        x.Nombre,
                        x.Codigo,
                        x.Descripcion,
                        x.Activo
                    })
                    .ToListAsync();

                return Ok(opciones);
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    Mensaje = "Hubo un error al obtener las opciones del menú mobile."
                });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var opcion = await _context.MenuMobile
                    .AsNoTracking()
                    .Where(x => x.Id == id)
                    .Select(x => new
                    {
                        x.Id,
                        x.Nombre,
                        x.Codigo,
                        x.Descripcion,
                        x.Activo
                    })
                    .FirstOrDefaultAsync();

                if (opcion == null)
                {
                    return NotFound(new
                    {
                        Mensaje = "No se encontró la opción del menú mobile."
                    });
                }

                return Ok(opcion);
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    Mensaje = "Hubo un error al obtener la opción del menú mobile."
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CrearMenuMobileDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return ValidationProblem(ModelState);
                }

                var nuevaOpcion = new MenuMobile
                {
                    Nombre = request.Nombre,
                    Descripcion = request.Descripcion,
                    Activo = true
                };

                await _context.MenuMobile.AddAsync(nuevaOpcion);
                await _context.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = nuevaOpcion.Id },
                    new
                    {
                        Mensaje = "Se cargó correctamente la opción " +
                                  nuevaOpcion.Nombre + ".",
                        Opcion = new
                        {
                            nuevaOpcion.Id,
                            nuevaOpcion.Nombre,
                            nuevaOpcion.Codigo,
                            nuevaOpcion.Descripcion,
                            nuevaOpcion.Activo
                        }
                    });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    Mensaje = "Hubo un error al cargar la opción. Inténtelo nuevamente más tarde."
                });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] ActualizarMenuMobileDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return ValidationProblem(ModelState);
                }

                var opcionExistente = await _context.MenuMobile
                    .FindAsync(id);

                if (opcionExistente == null)
                {
                    return NotFound(new
                    {
                        Mensaje = "No se encontró la opción a modificar."
                    });
                }

                opcionExistente.Nombre = request.Nombre;
                opcionExistente.Descripcion = request.Descripcion;
                opcionExistente.Activo = request.Activo;
                opcionExistente.Codigo = request.Codigo;

                _context.MenuMobile.Update(opcionExistente);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Mensaje = "Se modificó correctamente la opción.",
                    Opcion = new
                    {
                        opcionExistente.Id,
                        opcionExistente.Nombre,
                        opcionExistente.Codigo,
                        opcionExistente.Descripcion,
                        opcionExistente.Activo
                    }
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    Mensaje = "Hubo un error al modificar la opción. Inténtelo nuevamente más tarde."
                });
            }
        }

        [HttpPatch("{id:int}/estado")]
        public async Task<IActionResult> ToggleEstado(int id)
        {
            try
            {
                var opcion = await _context.MenuMobile.FindAsync(id);

                if (opcion == null)
                {
                    return NotFound(new
                    {
                        Mensaje = "No se encontró la opción del menú mobile."
                    });
                }

                opcion.Activo = !opcion.Activo;

                _context.MenuMobile.Update(opcion);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Mensaje = "Se " +
                              (opcion.Activo ? "habilitó" : "deshabilitó") +
                              " correctamente la opción " +
                              opcion.Nombre + ".",
                    opcion.Id,
                    opcion.Nombre,
                    opcion.Activo
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    Mensaje = "Hubo un error al cambiar el estado de la opción. Inténtelo nuevamente más tarde."
                });
            }
        }

        [HttpGet("{id:int}/usuarios")]
        public async Task<IActionResult> GetUsuarios(int id)
        {
            try
            {
                var menuMobile = await _context.MenuMobile
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (menuMobile == null)
                {
                    return NotFound(new
                    {
                        Mensaje = "Opción no encontrada."
                    });
                }

                var asignados = await _context.MenuMobileUsuariosHabilitados
                    .AsNoTracking()
                    .Where(x => x.MenuMobile.Id == id)
                    .Select(x => new
                    {
                        AsignacionId = x.Id,
                        MenuMobileId = x.MenuMobile.Id,
                        UsuarioId = x.Usuario.Id,
                        x.Usuario.Email,
                        Dni = x.Usuario.Personas != null
                            ? x.Usuario.Personas.NroDocumento
                            : null
                    })
                    .ToListAsync();

                return Ok(new
                {
                    MenuMobile = new
                    {
                        menuMobile.Id,
                        menuMobile.Nombre,
                        menuMobile.Codigo,
                        menuMobile.Descripcion,
                        menuMobile.Activo
                    },
                    Usuarios = asignados
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    Mensaje = "Hubo un error al obtener los usuarios asignados."
                });
            }
        }

        [HttpPost("{id:int}/usuarios")]
        public async Task<IActionResult> AddUsuarioByDni(
            int id,
            [FromBody] AsignarUsuarioMenuMobileDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return ValidationProblem(ModelState);
                }

                var menuMobile = await _context.MenuMobile
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (menuMobile == null)
                {
                    return NotFound(new
                    {
                        Mensaje = "No se encontró la opción del menú mobile."
                    });
                }

                var usuario = await _context.Usuarios
                    .Include(x => x.Personas)
                    .FirstOrDefaultAsync(x =>
                        x.Personas != null &&
                        x.Personas.NroDocumento == request.Dni);

                if (usuario == null)
                {
                    return NotFound(new
                    {
                        Mensaje = "No se encontró ningún usuario activo con ese DNI."
                    });
                }

                var usuarioYaAsignado =
                    await _context.MenuMobileUsuariosHabilitados
                        .AnyAsync(x =>
                            x.MenuMobile.Id == id &&
                            x.Usuario.Id == usuario.Id);

                if (usuarioYaAsignado)
                {
                    return Conflict(new
                    {
                        Mensaje = "El usuario ya se encuentra asignado."
                    });
                }

                var asignacion = new MenuMobileUsuariosHabilitados
                {
                    MenuMobile = menuMobile,
                    Usuario = usuario
                };

                await _context.MenuMobileUsuariosHabilitados
                    .AddAsync(asignacion);

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Mensaje = "Usuario asignado correctamente.",
                    Asignacion = new
                    {
                        AsignacionId = asignacion.Id,
                        MenuMobileId = menuMobile.Id,
                        MenuMobileNombre = menuMobile.Nombre,
                        UsuarioId = usuario.Id,
                        usuario.Email,
                        Dni = usuario.Personas != null
                            ? usuario.Personas.NroDocumento
                            : null
                    }
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    Mensaje = "Ocurrió un error al asignar el usuario."
                });
            }
        }

        [HttpDelete("usuarios/{asignacionId:int}")]
        public async Task<IActionResult> RemoveUsuario(int asignacionId)
        {
            try
            {
                var asignacion = await _context
                    .MenuMobileUsuariosHabilitados
                    .Include(x => x.MenuMobile)
                    .Include(x => x.Usuario)
                    .FirstOrDefaultAsync(x => x.Id == asignacionId);

                if (asignacion == null)
                {
                    return NotFound(new
                    {
                        Mensaje = "No se encontró la asignación."
                    });
                }

                var menuMobileId = asignacion.MenuMobile.Id;
                var usuarioId = asignacion.Usuario.Id;

                _context.MenuMobileUsuariosHabilitados.Remove(asignacion);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Mensaje = "Usuario desasignado correctamente.",
                    AsignacionId = asignacionId,
                    MenuMobileId = menuMobileId,
                    UsuarioId = usuarioId
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    Mensaje = "Ocurrió un error al desasignar el usuario."
                });
            }
        }
    }
}