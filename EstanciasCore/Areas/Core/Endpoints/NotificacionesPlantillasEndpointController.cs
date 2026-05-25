using DAL.Data;
using DAL.DTOs;
using DAL.Models;
using DAL.Models.Core;
using EstanciasCore.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.Endpoints
{
    [Route("endpoint/notificaciones-plantillas")]
    [ApiController]
    public class NotificacionesPlantillasEndpointController : ControllerBase
    {
        private readonly EstanciasContext _context;
        private readonly IPushService _wonderPushService;

        public NotificacionesPlantillasEndpointController(
            EstanciasContext context,
            IPushService wonderPushService)
        {
            _context = context;
            _wonderPushService = wonderPushService;
        }

        // GET: endpoint/notificaciones-plantillas/manuales?buscar=
        [HttpGet("manuales")]
        public async Task<IActionResult> GetManuales([FromQuery] string buscar = null)
        {
            var query = _context.NotificacionesPlantillas
                .Where(x =>
                    !x.NotificacionAutomatica &&
                    x.Activo);

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                var texto = buscar.Trim();

                query = query.Where(x =>
                    x.Nombre != null &&
                    x.Nombre.Contains(texto));
            }

            var data = await query
                .OrderBy(x => x.Nombre)
                .Select(x => new NotificacionesPlantillasDTO
                {
                    Id = x.Id,
                    Nombre = x.Nombre,
                    Titulo = x.Titulo,
                    Mensaje = x.Mensaje,
                    ImagenUrl = x.ImagenUrl,
                    ImagenIcon = x.Icon,
                    DeepLink = x.DeepLink,
                    PreferLargeImage = x.PreferLargeImage,
                    Activo = x.Activo,
                    NotificacionAutomatica = x.NotificacionAutomatica
                })
                .ToListAsync();

            return Ok(new
            {
                ok = true,
                titulo = "Listado Plantillas Manuales",
                botonNuevo = true,
                botonBorrar = true,
                data
            });
        }

        // GET: endpoint/notificaciones-plantillas/automaticas?buscar=
        [HttpGet("automaticas")]
        public async Task<IActionResult> GetAutomaticas([FromQuery] string buscar = null)
        {
            var query = _context.NotificacionesPlantillas
                .Where(x =>
                    x.NotificacionAutomatica &&
                    x.Activo);

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                var texto = buscar.Trim();

                query = query.Where(x =>
                    x.Nombre != null &&
                    x.Nombre.Contains(texto));
            }

            var data = await query
                .OrderBy(x => x.Nombre)
                .Select(x => new NotificacionesPlantillasDTO
                {
                    Id = x.Id,
                    Nombre = x.Nombre,
                    Titulo = x.Titulo,
                    Mensaje = x.Mensaje,
                    ImagenUrl = x.ImagenUrl,
                    ImagenIcon = x.Icon,
                    DeepLink = x.DeepLink,
                    PreferLargeImage = x.PreferLargeImage,
                    Activo = x.Activo,
                    NotificacionAutomatica = x.NotificacionAutomatica
                })
                .ToListAsync();

            return Ok(new
            {
                ok = true,
                titulo = "Listado Plantillas Automáticas",
                botonNuevo = false,
                botonBorrar = false,
                data
            });
        }

        // GET: endpoint/notificaciones-plantillas/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var plantilla = await _context.NotificacionesPlantillas
                .Where(x => x.Id == id)
                .Select(x => new NotificacionesPlantillasDTO
                {
                    Id = x.Id,
                    Nombre = x.Nombre,
                    Titulo = x.Titulo,
                    Mensaje = x.Mensaje,
                    ImagenUrl = x.ImagenUrl,
                    ImagenIcon = x.Icon,
                    DeepLink = x.DeepLink,
                    PreferLargeImage = x.PreferLargeImage,
                    Activo = x.Activo,
                    NotificacionAutomatica = x.NotificacionAutomatica
                })
                .FirstOrDefaultAsync();

            if (plantilla == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "Plantilla no encontrada."
                });
            }

            return Ok(new
            {
                ok = true,
                data = plantilla
            });
        }

        // POST: endpoint/notificaciones-plantillas
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NotificacionesPlantillasRequestDTO model)
        {
            if (model == null)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Datos inválidos."
                });
            }

            var plantilla = new NotificacionesPlantillas
            {
                Nombre = model.Nombre,
                Titulo = model.Titulo,
                Mensaje = model.Mensaje,
                ImagenUrl = model.ImagenUrl,
                PreferLargeImage = model.PreferLargeImage,
                DeepLink = model.DeepLink,
                Activo = true,
                Icon = model.ImagenIcon,

                // En el controller original no se setea, por default queda false.
                // Lo dejo así para que las creadas desde acá sean manuales.
                NotificacionAutomatica = false
            };

            await _context.NotificacionesPlantillas.AddAsync(plantilla);
            await _context.SaveChangesAsync();

            var data = new NotificacionesPlantillasDTO
            {
                Id = plantilla.Id,
                Nombre = plantilla.Nombre,
                Titulo = plantilla.Titulo,
                Mensaje = plantilla.Mensaje,
                ImagenUrl = plantilla.ImagenUrl,
                ImagenIcon = plantilla.Icon,
                DeepLink = plantilla.DeepLink,
                PreferLargeImage = plantilla.PreferLargeImage,
                Activo = plantilla.Activo,
                NotificacionAutomatica = plantilla.NotificacionAutomatica
            };

            return Ok(new
            {
                ok = true,
                message = "Plantilla creada con éxito.",
                data
            });
        }

        // PUT: endpoint/notificaciones-plantillas/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] NotificacionesPlantillasRequestDTO model)
        {
            if (model == null)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Datos inválidos."
                });
            }

            var plantilla = await _context.NotificacionesPlantillas
                .FirstOrDefaultAsync(x => x.Id == id);

            if (plantilla == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "Plantilla no encontrada."
                });
            }

            plantilla.Nombre = model.Nombre;
            plantilla.Titulo = model.Titulo;
            plantilla.Mensaje = model.Mensaje;
            plantilla.ImagenUrl = model.ImagenUrl;
            plantilla.Icon = model.ImagenIcon;
            plantilla.PreferLargeImage = model.PreferLargeImage;
            plantilla.DeepLink = model.DeepLink;

            _context.NotificacionesPlantillas.Update(plantilla);
            await _context.SaveChangesAsync();

            var data = new NotificacionesPlantillasDTO
            {
                Id = plantilla.Id,
                Nombre = plantilla.Nombre,
                Titulo = plantilla.Titulo,
                Mensaje = plantilla.Mensaje,
                ImagenUrl = plantilla.ImagenUrl,
                ImagenIcon = plantilla.Icon,
                DeepLink = plantilla.DeepLink,
                PreferLargeImage = plantilla.PreferLargeImage,
                Activo = plantilla.Activo,
                NotificacionAutomatica = plantilla.NotificacionAutomatica
            };

            return Ok(new
            {
                ok = true,
                message = "Plantilla actualizada con éxito.",
                data
            });
        }

        // DELETE: endpoint/notificaciones-plantillas/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var estaEnUso = await _context.Notificaciones
                    .AnyAsync(x =>
                        x.NotificacionesPlantillas != null &&
                        x.NotificacionesPlantillas.Id == id);

                if (estaEnUso)
                {
                    return BadRequest(new
                    {
                        ok = false,
                        success = false,
                        message = "La plantilla está asignada a una notificación y no se puede borrar."
                    });
                }

                var seUso = await _context.EnvioNotificaciones
                    .AnyAsync(x =>
                        x.NotificacionesPlantillas != null &&
                        x.NotificacionesPlantillas.Id == id);

                var plantilla = await _context.NotificacionesPlantillas
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (plantilla == null)
                {
                    return NotFound(new
                    {
                        ok = false,
                        success = false,
                        message = "Plantilla no encontrada."
                    });
                }

                if (seUso)
                {
                    plantilla.Activo = false;
                    _context.NotificacionesPlantillas.Update(plantilla);
                }
                else
                {
                    _context.NotificacionesPlantillas.Remove(plantilla);
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    ok = true,
                    success = true,
                    message = "Borrada con éxito."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    ok = false,
                    success = false,
                    message = "Error al borrar: " + ex.Message
                });
            }
        }

        // GET: endpoint/notificaciones-plantillas/listas-distribucion
        [HttpGet("listas-distribucion")]
        public async Task<IActionResult> GetListasDistribucion()
        {
            var listas = await _context.ListaDistribucion
                .Select(x => new ListaDistribucionSimpleDTO
                {
                    Id = x.Id,
                    Nombre = x.Nombre
                })
                .OrderBy(x => x.Nombre)
                .ToListAsync();

            return Ok(new
            {
                ok = true,
                data = listas
            });
        }

        // POST: endpoint/notificaciones-plantillas/5/enviar-test
        [HttpPost("{id:int}/enviar-test")]
        public async Task<IActionResult> EnviarTest(int id, [FromBody] EnviarTestPlantillaRequestDTO request)
        {
            try
            {
                if (request == null || request.ListaDistribucionId <= 0)
                {
                    return BadRequest(new
                    {
                        ok = false,
                        success = false,
                        message = "Debe indicar una lista de distribución válida."
                    });
                }

                var plantilla = await _context.NotificacionesPlantillas
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (plantilla == null)
                {
                    return NotFound(new
                    {
                        ok = false,
                        success = false,
                        message = "Plantilla no encontrada."
                    });
                }

                var testList = await _context.ListaDistribucion
                    .FirstOrDefaultAsync(x => x.Id == request.ListaDistribucionId);

                if (testList == null)
                {
                    return NotFound(new
                    {
                        ok = false,
                        success = false,
                        message = "Lista de distribución no encontrada."
                    });
                }

                var deviceIds = new List<string>();
                var destinatarios = new List<Usuario>();

                if (testList.Id == 1)
                {
                    var allUsers = await _context.Usuarios
                        .Where(x => x.DeviceId != null)
                        .ToListAsync();

                    foreach (var user in allUsers)
                    {
                        if (!string.IsNullOrEmpty(user.DeviceId))
                        {
                            deviceIds.Add(user.DeviceId);
                            destinatarios.Add(user);
                        }
                    }
                }
                else
                {
                    var distDestinatarios = await _context.DistribucionDestinatarios
                        .Include(x => x.Destinatario)
                        .Where(x => x.ListaDistribucion.Id == testList.Id)
                        .ToListAsync();

                    foreach (var item in distDestinatarios)
                    {
                        if (item.Destinatario != null &&
                            !string.IsNullOrEmpty(item.Destinatario.DeviceId))
                        {
                            deviceIds.Add(item.Destinatario.DeviceId);
                            destinatarios.Add(item.Destinatario);
                        }
                    }
                }

                if (!deviceIds.Any())
                {
                    return BadRequest(new
                    {
                        ok = false,
                        success = false,
                        message = $"La lista '{testList.Nombre}' no tiene destinatarios con DeviceId."
                    });
                }

                var dto = new NotificacionViewModelDTO
                {
                    Titulo = plantilla.Titulo,
                    Mensaje = plantilla.Mensaje,
                    ImagenUrl = plantilla.ImagenUrl,
                    ImagenIcon = plantilla.Icon,
                    PreferLargeImage = plantilla.PreferLargeImage,
                    DeepLink = plantilla.DeepLink
                };

                var result = await _wonderPushService.EnviarNotificacionAIds(dto, deviceIds);

                var envioNotificacion = new EnvioNotificaciones
                {
                    Titulo = plantilla.Titulo,
                    Texto = plantilla.Mensaje,
                    Fecha = DateTime.Now,
                    Envio = result,
                    NotificacionesPlantillas = plantilla,
                    Foto = null
                };

                await _context.EnvioNotificaciones.AddAsync(envioNotificacion);
                await _context.SaveChangesAsync();

                var envioDestinatarios = new List<EnvioNotificacionesDestinatarios>();

                foreach (var user in destinatarios)
                {
                    envioDestinatarios.Add(new EnvioNotificacionesDestinatarios
                    {
                        Notificacion = envioNotificacion,
                        Destinatario = user,
                        Envio = result
                    });
                }

                await _context.EnvioNotificacionesDestinatarios.AddRangeAsync(envioDestinatarios);
                await _context.SaveChangesAsync();

                if (result)
                {
                    return Ok(new
                    {
                        ok = true,
                        success = true,
                        message = "Notificación de prueba enviada y registrada.",
                        cantidadDestinatarios = destinatarios.Count
                    });
                }

                return BadRequest(new
                {
                    ok = false,
                    success = false,
                    message = "Error al enviar la notificación a WonderPush. Registrado como fallido.",
                    cantidadDestinatarios = destinatarios.Count
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    ok = false,
                    success = false,
                    message = "Error: " + ex.Message
                });
            }
        }
    }

}