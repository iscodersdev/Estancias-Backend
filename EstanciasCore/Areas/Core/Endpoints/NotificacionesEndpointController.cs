using DAL.Data;
using DAL.DTOs;
using DAL.Models;
using DAL.Models.Core;
using EstanciasCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace EstanciasCore.Endpoints
{
    [Route("endpoint/notificaciones")]
    [ApiController]
    [AllowAnonymous]
    public class NotificacionesEndpointController : ControllerBase
    {
        private readonly EstanciasContext _context;

        public NotificacionesEndpointController(EstanciasContext context)
        {
            _context = context;
        }

        [HttpGet("manuales")]
        public async Task<IActionResult> ObtenerNotificaciones(
            string buscar = null,
            int page = 1,
            int pageSize = 10)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;

                var query = _context.Notificaciones
                    .Include(x => x.TipoNotificacionesProcedimientos)
                    .Include(x => x.NotificacionesPlantillas)
                    .Include(x => x.ListaDistribucion)
                    .Where(x =>
                        x.TipoNotificacionesProcedimientos.Codigo == "MA" &&
                        !x.Borrado &&
                        (string.IsNullOrEmpty(buscar) || x.Nombre.Contains(buscar)));

                var total = await query.CountAsync();

                var data = await query
                    .OrderBy(x => x.Nombre)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => new
                    {
                        x.Id,
                        x.Nombre,
                        x.Descripcion,
                        x.Activo,
                        x.Borrado,
                        x.FechaEjecucion,
                        x.FechaUltimaEjecucion,

                        TipoNotificacionesProcedimientosId = x.TipoNotificacionesProcedimientos != null
                            ? x.TipoNotificacionesProcedimientos.Id
                            : (int?)null,

                        TipoNotificacionesProcedimientos = x.TipoNotificacionesProcedimientos != null
                            ? x.TipoNotificacionesProcedimientos.Nombre
                            : null,

                        CodigoTipoNotificacion = x.TipoNotificacionesProcedimientos != null
                            ? x.TipoNotificacionesProcedimientos.Codigo
                            : null,

                        PlantillaId = x.NotificacionesPlantillas != null
                            ? x.NotificacionesPlantillas.Id
                            : (int?)null,

                        Plantilla = x.NotificacionesPlantillas != null
                            ? x.NotificacionesPlantillas.Nombre
                            : null,

                        ListaDistribucionId = x.ListaDistribucion != null
                            ? x.ListaDistribucion.Id
                            : (int?)null,

                        ListaDistribucion = x.ListaDistribucion != null
                            ? x.ListaDistribucion.Nombre
                            : null
                    })
                    .ToListAsync();

                return Ok(new
                {
                    ok = true,
                    total,
                    page,
                    pageSize,
                    data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Error: " + ex.Message
                });
            }
        }

        [HttpGet("automaticas")]
        public async Task<IActionResult> ObtenerNotificacionesAutomaticas(
            string buscar = null,
            int page = 1,
            int pageSize = 10)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;

                var query = _context.Notificaciones
                    .Include(x => x.TipoNotificacionesProcedimientos)
                    .Include(x => x.NotificacionesPlantillas)
                    .Include(x => x.ListaDistribucion)
                    .Where(x =>
                        x.TipoNotificacionesProcedimientos.Codigo == "AU" &&
                        (string.IsNullOrEmpty(buscar) || x.Nombre.Contains(buscar)));

                var total = await query.CountAsync();

                var data = await query
                    .OrderBy(x => x.Nombre)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => new
                    {
                        x.Id,
                        x.Nombre,
                        x.Descripcion,
                        x.Activo,
                        x.Borrado,
                        x.FechaEjecucion,
                        x.FechaUltimaEjecucion,

                        TipoNotificacionesProcedimientosId = x.TipoNotificacionesProcedimientos != null
                            ? x.TipoNotificacionesProcedimientos.Id
                            : (int?)null,

                        TipoNotificacionesProcedimientos = x.TipoNotificacionesProcedimientos != null
                            ? x.TipoNotificacionesProcedimientos.Nombre
                            : null,

                        CodigoTipoNotificacion = x.TipoNotificacionesProcedimientos != null
                            ? x.TipoNotificacionesProcedimientos.Codigo
                            : null,

                        PlantillaId = x.NotificacionesPlantillas != null
                            ? x.NotificacionesPlantillas.Id
                            : (int?)null,

                        Plantilla = x.NotificacionesPlantillas != null
                            ? x.NotificacionesPlantillas.Nombre
                            : null,

                        ListaDistribucionId = x.ListaDistribucion != null
                            ? x.ListaDistribucion.Id
                            : (int?)null,

                        ListaDistribucion = x.ListaDistribucion != null
                            ? x.ListaDistribucion.Nombre
                            : null
                    })
                    .ToListAsync();

                return Ok(new
                {
                    ok = true,
                    total,
                    page,
                    pageSize,
                    data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Error: " + ex.Message
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            try
            {
                var notificacion = await _context.Notificaciones
                    .Include(x => x.TipoNotificacionesProcedimientos)
                    .Include(x => x.NotificacionesPlantillas)
                    .Include(x => x.ListaDistribucion)
                    .Where(x => x.Id == id)
                    .Select(x => new
                    {
                        x.Id,
                        x.Nombre,
                        x.Descripcion,
                        x.Activo,
                        x.Borrado,
                        x.FechaEjecucion,
                        x.FechaUltimaEjecucion,

                        TipoNotificacionesProcedimientosId = x.TipoNotificacionesProcedimientos != null
                            ? x.TipoNotificacionesProcedimientos.Id
                            : (int?)null,

                        TipoNotificacionesProcedimientos = x.TipoNotificacionesProcedimientos != null
                            ? x.TipoNotificacionesProcedimientos.Nombre
                            : null,

                        CodigoTipoNotificacion = x.TipoNotificacionesProcedimientos != null
                            ? x.TipoNotificacionesProcedimientos.Codigo
                            : null,

                        PlantillaId = x.NotificacionesPlantillas != null
                            ? x.NotificacionesPlantillas.Id
                            : (int?)null,

                        Plantilla = x.NotificacionesPlantillas != null
                            ? x.NotificacionesPlantillas.Nombre
                            : null,

                        ListaDistribucionId = x.ListaDistribucion != null
                            ? x.ListaDistribucion.Id
                            : (int?)null,

                        ListaDistribucion = x.ListaDistribucion != null
                            ? x.ListaDistribucion.Nombre
                            : null
                    })
                    .FirstOrDefaultAsync();

                if (notificacion == null)
                {
                    return NotFound(new
                    {
                        ok = false,
                        message = "Notificación no encontrada."
                    });
                }

                return Ok(new
                {
                    ok = true,
                    data = notificacion
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Error: " + ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Notificaciones notificaciones)
        {
            try
            {
                var tipoNotificacion = await _context.TipoNotificacionesProcedimientos
                    .Where(x => x.Codigo == "MA")
                    .FirstOrDefaultAsync();

                notificaciones.TipoNotificacionesProcedimientos = tipoNotificacion;
                notificaciones.Activo = true;

                await _context.Notificaciones.AddAsync(notificaciones);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    ok = true,
                    message = "Notificación creada correctamente.",
                    data = new
                    {
                        notificaciones.Id
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Error: " + ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Notificaciones notificaciones)
        {
            try
            {
                Notificaciones notificacionesUpdate = await _context.Notificaciones
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync();

                if (notificacionesUpdate == null)
                {
                    return NotFound(new
                    {
                        ok = false,
                        message = "Notificación no encontrada."
                    });
                }

                notificacionesUpdate.Nombre = notificaciones.Nombre;
                notificacionesUpdate.Descripcion = notificaciones.Descripcion;
                notificacionesUpdate.FechaEjecucion = notificaciones.FechaEjecucion;
                notificacionesUpdate.FechaUltimaEjecucion = DateTime.MinValue;

                _context.Notificaciones.Update(notificacionesUpdate);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    ok = true,
                    message = "Notificación actualizada correctamente."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Error: " + ex.Message
                });
            }
        }

        [HttpGet("historial")]
        public async Task<IActionResult> HistorialNotificaciones(
            int page = 1,
            int pageSize = 10)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;

                var query = _context.EnvioNotificaciones.AsQueryable();

                var total = await query.CountAsync();

                var data = await query
                    .OrderByDescending(x => x.Fecha)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => new
                    {
                        x.Id,
                        x.Titulo,
                        x.Texto,
                        x.Fecha,
                        x.Envio,
                        TieneFoto = x.Foto != null
                    })
                    .ToListAsync();

                return Ok(new
                {
                    ok = true,
                    total,
                    page,
                    pageSize,
                    data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Error: " + ex.Message
                });
            }
        }

        [HttpGet("destinatarios-combo")]
        public IActionResult DestinatariosComboJson(string q)
        {
            try
            {
                q = q ?? "";

                var items = _context.Usuarios
                    .Where(x => x.Personas.NroDocumento.Contains(q))
                    .Select(x => new
                    {
                        Text = $"{x.Personas.Apellido}, {x.Personas.Nombres}",
                        Value = x.Id,
                        Subtext = $"{x.UserName}",
                        Icon = "fa fa-user"
                    })
                    .Take(10)
                    .ToArray();

                return Ok(items);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Error: " + ex.Message
                });
            }
        }


        [HttpPost("enviar")]
        public async Task<IActionResult> EnvioDeNotificacion([FromForm] EnvioNotificacionDTO notificacion)
        {
            try
            {
                List<Usuario> usuarios = new List<Usuario>();
                List<string> deviceList = new List<string>();
                byte[] imagen = null;

                if (notificacion.TipoDeEnvio == 1)
                {
                    Usuario user = await _context.Usuarios
                        .Where(x => x.Id == notificacion.NroDocumentoNotificacion)
                        .FirstOrDefaultAsync();

                    if (user == null)
                    {
                        return BadRequest(new
                        {
                            ok = false,
                            message = "Hubo un error, El Dni Ingresado no es válido."
                        });
                    }

                    deviceList.Add(user.DeviceId);
                    usuarios.Add(user);
                }
                else
                {
                    ListaDistribucion lista = await _context.ListaDistribucion
                        .Where(x => x.Id == Convert.ToInt32(notificacion.DistribucionNotificacion))
                        .FirstOrDefaultAsync();

                    if (lista.Nombre != "Todos")
                    {
                        List<DistribucionDestinatarios> destinatariosEnvio = await _context.DistribucionDestinatarios
                            .Where(x => x.ListaDistribucion.Id == lista.Id)
                            .ToListAsync();

                        deviceList = destinatariosEnvio
                            .Select(x => x.Destinatario.DeviceId)
                            .ToList();

                        usuarios = destinatariosEnvio
                            .Select(x => x.Destinatario)
                            .ToList();
                    }
                    else
                    {
                        usuarios = await _context.Usuarios
                            .Where(x => x.DeviceId != null)
                            .ToListAsync();

                        deviceList = usuarios
                            .Select(x => x.DeviceId)
                            .ToList();
                    }
                }

                string[] deviceIds = deviceList.ToArray();

                if (notificacion.File != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await notificacion.File.CopyToAsync(memoryStream);
                        imagen = memoryStream.ToArray();
                    }
                }

                HttpStatusCode response = common.EnviaNotificationWonderPushId(
                    notificacion.TituloNotificacion,
                    notificacion.TextoNotificacion,
                    deviceIds,
                    imagen);

                if (response == HttpStatusCode.Accepted)
                {
                    GuardarNotificacion(usuarios, notificacion, imagen);
                }

                return Ok(new
                {
                    ok = true,
                    message = "Se enviaron las Notificaciones correctamente.",
                    statusCode = response
                });
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Hubo un error, " + e.Message
                });
            }
        }

        [HttpGet("imagen/{id}")]
        public async Task<IActionResult> Imagen(int id)
        {
            try
            {
                EnvioNotificaciones notificacion = await _context.EnvioNotificaciones
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync();

                if (notificacion == null || notificacion.Foto == null)
                {
                    return NotFound(new
                    {
                        ok = false,
                        message = "Imagen no encontrada."
                    });
                }

                return File(notificacion.Foto, "image/jpeg");
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Error: " + ex.Message
                });
            }
        }

        [HttpGet("historial-destinatarios/{id}")]
        public async Task<IActionResult> HistorialDestinatariosNotificaciones(
            int id,
            int page = 1,
            int pageSize = 10)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;

                var query = _context.EnvioNotificacionesDestinatarios
                    .Where(x => x.Id == id)
                    .Select(x => x.Destinatario);

                var total = await query.CountAsync();

                var data = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => new
                    {
                        x.Id,
                        x.UserName,
                        Persona = x.Personas != null
                            ? x.Personas.Apellido + ", " + x.Personas.Nombres
                            : null,
                        NroDocumento = x.Personas != null
                            ? x.Personas.NroDocumento
                            : null,
                        x.DeviceId
                    })
                    .ToListAsync();

                return Ok(new
                {
                    ok = true,
                    total,
                    page,
                    pageSize,
                    data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Error: " + ex.Message
                });
            }
        }

        [HttpGet("{id}/plantillas-manuales")]
        public async Task<IActionResult> AsignarPlantilla(int id)
        {
            try
            {
                var notificacion = await _context.Notificaciones
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (notificacion == null)
                {
                    return NotFound(new
                    {
                        ok = false,
                        message = "Notificación no encontrada."
                    });
                }

                var plantillas = await _context.NotificacionesPlantillas
                    .Where(x => x.Activo && !x.NotificacionAutomatica)
                    .Select(x => new
                    {
                        Text = x.Nombre,
                        Value = x.Id.ToString(),
                        Selected = notificacion.NotificacionesPlantillas != null &&
                                   notificacion.NotificacionesPlantillas.Id == x.Id
                    })
                    .ToListAsync();

                return Ok(new
                {
                    ok = true,
                    notificacionId = id,
                    data = plantillas
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Error: " + ex.Message
                });
            }
        }

        [HttpGet("{id}/plantillas-automaticas")]
        public async Task<IActionResult> AsignarPlantillaAutomaticas(int id)
        {
            try
            {
                var notificacion = await _context.Notificaciones
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (notificacion == null)
                {
                    return NotFound(new
                    {
                        ok = false,
                        message = "Notificación no encontrada."
                    });
                }

                var plantillas = await _context.NotificacionesPlantillas
                    .Where(x => x.Activo && x.NotificacionAutomatica)
                    .Select(x => new
                    {
                        Text = x.Nombre,
                        Value = x.Id.ToString(),
                        Selected = notificacion.NotificacionesPlantillas != null &&
                                   notificacion.NotificacionesPlantillas.Id == x.Id
                    })
                    .ToListAsync();

                return Ok(new
                {
                    ok = true,
                    notificacionId = id,
                    data = plantillas
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Error: " + ex.Message
                });
            }
        }

        [HttpPost("asignar-plantilla")]
        public async Task<IActionResult> AsignarPlantilla(int notificacionId, int plantillaId)
        {
            try
            {
                var notificacion = await _context.Notificaciones
                    .Include(x => x.NotificacionesPlantillas)
                    .FirstOrDefaultAsync(x => x.Id == notificacionId);

                if (notificacion == null)
                {
                    return Ok(new
                    {
                        success = false,
                        message = "Notificación no encontrada."
                    });
                }

                if (plantillaId <= 0)
                {
                    notificacion.NotificacionesPlantillas = null;

                    _context.Notificaciones.Update(notificacion);
                    await _context.SaveChangesAsync();

                    return Ok(new
                    {
                        success = true,
                        message = "Plantilla desasignada correctamente."
                    });
                }

                var plantilla = await _context.NotificacionesPlantillas
                    .FirstOrDefaultAsync(x => x.Id == plantillaId);

                if (plantilla != null)
                {
                    notificacion.NotificacionesPlantillas = plantilla;

                    _context.Notificaciones.Update(notificacion);
                    await _context.SaveChangesAsync();

                    return Ok(new
                    {
                        success = true,
                        message = "Plantilla asignada correctamente."
                    });
                }

                return Ok(new
                {
                    success = false,
                    message = "Error al asignar la plantilla. Verifique los datos."
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = false,
                    message = "Error: " + ex.Message
                });
            }
        }

        [HttpGet("{id}/listas-distribucion")]
        public async Task<IActionResult> AsignarListaDistribucion(int id)
        {
            try
            {
                var notificacion = await _context.Notificaciones
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (notificacion == null)
                {
                    return NotFound(new
                    {
                        ok = false,
                        message = "Notificación no encontrada."
                    });
                }

                var listas = await _context.ListaDistribucion
                    .Select(x => new
                    {
                        Text = x.Nombre,
                        Value = x.Id.ToString(),
                        Selected = notificacion.ListaDistribucion != null &&
                                   notificacion.ListaDistribucion.Id == x.Id
                    })
                    .ToListAsync();

                return Ok(new
                {
                    ok = true,
                    notificacionId = id,
                    data = listas
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Error: " + ex.Message
                });
            }
        }

        [HttpPost("asignar-lista-distribucion")]
        public async Task<IActionResult> AsignarListaDistribucion(int notificacionId, int listaDistribucionId)
        {
            try
            {
                var notificacion = await _context.Notificaciones
                    .Include(x => x.ListaDistribucion)
                    .FirstOrDefaultAsync(x => x.Id == notificacionId);

                if (notificacion == null)
                {
                    return Ok(new
                    {
                        success = false,
                        message = "Notificación no encontrada."
                    });
                }

                if (listaDistribucionId <= 0)
                {
                    notificacion.ListaDistribucion = null;

                    _context.Notificaciones.Update(notificacion);
                    await _context.SaveChangesAsync();

                    return Ok(new
                    {
                        success = true,
                        message = "Lista desasignada correctamente."
                    });
                }

                var lista = await _context.ListaDistribucion
                    .FirstOrDefaultAsync(x => x.Id == listaDistribucionId);

                if (lista != null)
                {
                    notificacion.ListaDistribucion = lista;

                    _context.Notificaciones.Update(notificacion);
                    await _context.SaveChangesAsync();

                    return Ok(new
                    {
                        success = true,
                        message = "Lista asignada correctamente."
                    });
                }

                return Ok(new
                {
                    success = false,
                    message = "Error al asignar la lista. Verifique los datos."
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = false,
                    message = "Error: " + ex.Message
                });
            }
        }

        [HttpPost("toggle-estado/{id}")]
        public async Task<IActionResult> ToggleEstado(int id)
        {
            try
            {
                var notificacion = await _context.Notificaciones
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (notificacion != null)
                {
                    notificacion.Activo = !notificacion.Activo;

                    _context.Notificaciones.Update(notificacion);
                    await _context.SaveChangesAsync();

                    return Ok(new
                    {
                        success = true,
                        message = "Estado actualizado correctamente."
                    });
                }

                return Ok(new
                {
                    success = false,
                    message = "Notificación no encontrada."
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = false,
                    message = "Error: " + ex.Message
                });
            }
        }

        [HttpPost("borrar-permanente/{id}")]
        public async Task<IActionResult> BorrarPermanente(int id)
        {
            try
            {
                var notificacion = await _context.Notificaciones
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (notificacion != null)
                {
                    notificacion.Borrado = true;

                    _context.Notificaciones.Update(notificacion);
                    await _context.SaveChangesAsync();

                    return Ok(new
                    {
                        success = true,
                        message = "Estado actualizado correctamente."
                    });
                }

                return Ok(new
                {
                    success = false,
                    message = "Notificación no encontrada."
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = false,
                    message = "Error: " + ex.Message
                });
            }
        }

        [HttpPost("cambiar-fecha")]
        public async Task<IActionResult> CambiarFecha(int id, int dia, DateTime hora)
        {
            try
            {
                var notificacion = await _context.Notificaciones
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (notificacion != null)
                {
                    var baseDate = notificacion.FechaEjecucion;

                    try
                    {
                        var newDate = new DateTime(
                            baseDate.Year,
                            baseDate.Month,
                            dia,
                            hora.Hour,
                            hora.Minute,
                            0);

                        notificacion.FechaEjecucion = newDate;
                        notificacion.FechaUltimaEjecucion = new DateTime(1111, 1, 1, 0, 0, 0);

                        _context.Notificaciones.Update(notificacion);
                        await _context.SaveChangesAsync();

                        return Ok(new
                        {
                            success = true,
                            message = "Fecha actualizada correctamente."
                        });
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        return Ok(new
                        {
                            success = false,
                            message = "El día seleccionado no es válido para el mes actual."
                        });
                    }
                }

                return Ok(new
                {
                    success = false,
                    message = "Notificación no encontrada."
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = false,
                    message = "Error: " + ex.Message
                });
            }
        }

        private bool GuardarNotificacion(List<Usuario> destinatarios, EnvioNotificacionDTO notificacion, byte[] imagen = null)
        {
            EnvioNotificaciones envioNotificaciones = new EnvioNotificaciones()
            {
                Titulo = notificacion.TituloNotificacion,
                Texto = notificacion.TextoNotificacion,
                Foto = imagen,
                Fecha = DateTime.Now,
                Envio = true,
            };

            _context.EnvioNotificaciones.Add(envioNotificaciones);

            foreach (var item in destinatarios)
            {
                EnvioNotificacionesDestinatarios notificacionDestinatario = new EnvioNotificacionesDestinatarios()
                {
                    Notificacion = envioNotificaciones,
                    Destinatario = item,
                    Envio = true,
                };

                _context.EnvioNotificacionesDestinatarios.Add(notificacionDestinatario);
            }

            _context.SaveChanges();

            return true;
        }
    }
}