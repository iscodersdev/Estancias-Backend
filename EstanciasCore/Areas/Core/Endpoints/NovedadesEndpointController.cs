using DAL.Data;
using DAL.DTOs;
using DAL.Models;
using EstanciasCore.API.Filters;
using EstanciasCore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace EstanciasCore.Endpoints
{
    [TypeFilter(typeof(EndpointUatAuthorizeAttribute))]
    [ApiController]
    [Route("endpoint/novedades")]
    public class NovedadesEndpointController : ControllerBase
    {
        private readonly EstanciasContext _context;
        private readonly NotificacionAPIService _notificacionPush;

        public NovedadesEndpointController(
            EstanciasContext context,
            NotificacionAPIService notificacionPush)
        {
            _context = context;
            _notificacionPush = notificacionPush;
        }

        [HttpGet("obtener-novedades")]
        public async Task<IActionResult> ObtenerNovedades(
     [FromQuery] string buscar = "",
     [FromQuery] int page = 1,
     [FromQuery] int pageSize = 10)
        {
            try
            {
                if (page < 1)
                    page = 1;

                if (pageSize < 1)
                    pageSize = 10;

                var searchText = buscar ?? "";

                string usuarioEmail = null;

                if (Request.Headers.ContainsKey("UsuarioEmail"))
                    usuarioEmail = Request.Headers["UsuarioEmail"].ToString();

                if (string.IsNullOrWhiteSpace(usuarioEmail))
                    usuarioEmail = User?.Identity?.Name;

                var usuario = await _context.Usuarios
                    .Include(x => x.Clientes)
                        .ThenInclude(x => x.Empresa)
                    .FirstOrDefaultAsync(x => x.Email == usuarioEmail);

                var query = _context.Novedades
                    .Include(x => x.Empresa)
                    .Include(x => x.Color)
                    .AsQueryable();

                if (usuario == null || usuario.Clientes == null || usuario.Clientes.Empresa == null)
                {
                    query = query.Where(x =>
                        x.Empresa == null &&
                        (
                            string.IsNullOrEmpty(searchText) ||
                            x.Titulo.Contains(searchText) ||
                            x.Texto.Contains(searchText)
                        )
                    );
                }
                else
                {
                    var empresaId = usuario.Clientes.Empresa.Id;

                    query = query.Where(x =>
                        x.Empresa != null &&
                        x.Empresa.Id == empresaId &&
                        (
                            string.IsNullOrEmpty(searchText) ||
                            x.Titulo.Contains(searchText) ||
                            x.Texto.Contains(searchText)
                        )
                    );
                }

                var totalRegistros = await query.CountAsync();

                var novedades = await query
                    .OrderBy(x => x.Fecha)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => new NovedadesDTO
                    {
                        Id = x.Id,
                        Fecha = x.Fecha,
                        Titulo = x.Titulo,
                        Subtitulo = x.Subtitulo,
                        Foto = x.Foto,
                        Texto = x.Texto,
                        TextoBoton = x.TextoBoton,
                        Publica = x.Publica,
                        EmpresaId = x.Empresa != null ? x.Empresa.Id : (int?)null,
                        ColorId = x.Color != null ? x.Color.Id : (int?)null
                    })
                    .ToListAsync();

                return Ok(new NovedadesListadoDTO
                {
                    TotalRegistros = totalRegistros,
                    PaginaActual = page,
                    RegistrosPorPagina = pageSize,
                    Novedades = novedades
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Status = 400,
                    Mensaje = "Error al obtener el listado de novedades.",
                    Error = ex.Message
                });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var novedad = await _context.Novedades
                .Include(x => x.Empresa)
                .Include(x => x.Color)
                .Where(x => x.Id == id)
                .Select(x => new NovedadesDTO
                {
                    Id = x.Id,
                    Fecha = x.Fecha,
                    Titulo = x.Titulo,
                    Subtitulo = x.Subtitulo,
                    Foto = x.Foto,
                    Texto = x.Texto,
                    TextoBoton = x.TextoBoton,
                    Publica = x.Publica,
                    EmpresaId = x.Empresa != null ? x.Empresa.Id : (int?)null,
                    ColorId = x.Color != null ? x.Color.Id : (int?)null
                })
                .FirstOrDefaultAsync();

            if (novedad == null)
            {
                return NotFound(new
                {
                    Status = 404,
                    Mensaje = "No se encontró la novedad solicitada."
                });
            }

            return Ok(novedad);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NovedadesDTO dto)
        {
            if (dto == null)
            {
                return BadRequest(new NovedadesResponseDTO
                {
                    Status = 400,
                    Mensaje = "Los datos enviados son inválidos.",
                    Novedad = null
                });
            }

            try
            {
                var usuario = await _context.Usuarios
                    .Include(x => x.Clientes)
                        .ThenInclude(x => x.Empresa)
                    .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

                var novedad = new Novedades
                {
                    Fecha = dto.Fecha,
                    Titulo = dto.Titulo,
                    Subtitulo = dto.Subtitulo,
                    Foto = dto.Foto,
                    Texto = dto.Texto,
                    TextoBoton = dto.TextoBoton,
                    Publica = dto.Publica,
                    Empresa = usuario != null && usuario.Clientes != null
                        ? usuario.Clientes.Empresa
                        : null
                };

                _context.Novedades.Add(novedad);
                await _context.SaveChangesAsync();

                return Ok(new NovedadesResponseDTO
                {
                    Status = 200,
                    Mensaje = "Se registró correctamente la novedad.",
                    Novedad = MapToDTO(novedad)
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new NovedadesResponseDTO
                {
                    Status = 500,
                    Mensaje = "Hubo un error al registrar la novedad.",
                    Novedad = dto
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] NovedadesDTO dto)
        {
            if (dto == null)
            {
                return BadRequest(new NovedadesResponseDTO
                {
                    Status = 400,
                    Mensaje = "Los datos enviados son inválidos.",
                    Novedad = null
                });
            }

            if (id != dto.Id)
            {
                return BadRequest(new NovedadesResponseDTO
                {
                    Status = 400,
                    Mensaje = "El Id enviado por la URL no coincide con el Id del cuerpo de la petición.",
                    Novedad = dto
                });
            }

            try
            {
                var novedad = await _context.Novedades
                    .FirstOrDefaultAsync(x => x.Id == dto.Id);

                if (novedad == null)
                {
                    return NotFound(new NovedadesResponseDTO
                    {
                        Status = 404,
                        Mensaje = "No se encontró la novedad a editar.",
                        Novedad = dto
                    });
                }

                novedad.Titulo = dto.Titulo;
                novedad.Subtitulo = dto.Subtitulo == null ? " " : dto.Subtitulo;
                novedad.Texto = dto.Texto == null ? " " : dto.Texto;
                novedad.TextoBoton = dto.TextoBoton == null ? " " : dto.TextoBoton;
                novedad.Fecha = dto.Fecha;
                novedad.Publica = dto.Publica;

                await _context.SaveChangesAsync();

                return Ok(new NovedadesResponseDTO
                {
                    Status = 200,
                    Mensaje = "Se editó correctamente la novedad.",
                    Novedad = MapToDTO(novedad)
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new NovedadesResponseDTO
                {
                    Status = 500,
                    Mensaje = "Hubo un error al editar la novedad.",
                    Novedad = dto
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var novedad = await _context.Novedades
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (novedad == null)
                {
                    return NotFound(new NovedadesResponseDTO
                    {
                        Status = 404,
                        Mensaje = "No se encontró la novedad a eliminar.",
                        Novedad = null
                    });
                }

                _context.Novedades.Remove(novedad);
                await _context.SaveChangesAsync();

                return Ok(new NovedadesResponseDTO
                {
                    Status = 200,
                    Mensaje = "Se eliminó correctamente la novedad.",
                    Novedad = null
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new NovedadesResponseDTO
                {
                    Status = 500,
                    Mensaje = "Hubo un error al eliminar la novedad.",
                    Novedad = null
                });
            }
        }

        [HttpPost("{id}/imagen")]
        public async Task<IActionResult> CambiarImagen(int id, [FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new NovedadesResponseDTO
                {
                    Status = 400,
                    Mensaje = "Debe seleccionar una imagen.",
                    Novedad = null
                });
            }

            var novedad = await _context.Novedades.FindAsync(id);

            if (novedad == null)
            {
                return NotFound(new NovedadesResponseDTO
                {
                    Status = 404,
                    Mensaje = "No se encontró la novedad solicitada.",
                    Novedad = null
                });
            }

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    novedad.Foto = Convert.ToBase64String(memoryStream.ToArray());
                    await _context.SaveChangesAsync();
                }

                return Ok(new NovedadesResponseDTO
                {
                    Status = 200,
                    Mensaje = "Se cambió correctamente la imagen de la novedad.",
                    Novedad = MapToDTO(novedad)
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new NovedadesResponseDTO
                {
                    Status = 500,
                    Mensaje = "Hubo un error al cambiar la imagen de la novedad.",
                    Novedad = MapToDTO(novedad)
                });
            }
        }

        [HttpPost("{id}/notificacion")]
        public async Task<IActionResult> EnviarNotificacion(int id)
        {
            try
            {
                var usuario = await _context.Usuarios
                    .Include(x => x.Clientes)
                        .ThenInclude(x => x.Empresa)
                    .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

                HttpStatusCode resultStatusCode = HttpStatusCode.BadRequest;

                if (usuario != null && usuario.Clientes != null && usuario.Clientes.Empresa != null)
                {
                    int empresaId = usuario.Clientes.Empresa.Id;

                    var listaPush = await _context.Clientes
                        .Include(x => x.Empresa)
                        .Include(x => x.Usuario)
                        .Where(x => x.Empresa.Id == empresaId)
                        .ToListAsync();

                    var novedad = await _context.Novedades.FindAsync(id);

                    if (novedad == null)
                    {
                        return NotFound(new NotificacionNovedadResponseDTO
                        {
                            Status = 404,
                            Mensaje = "No se encontró la novedad.",
                            CantidadEnviadas = 0
                        });
                    }

                    foreach (var item in listaPush)
                    {
                        if (item.Usuario != null && !string.IsNullOrWhiteSpace(item.Usuario.DeviceId))
                        {
                            resultStatusCode = _notificacionPush.Envia_Push(
                                item.Usuario.DeviceId,
                                novedad.Titulo,
                                "texto");
                        }
                    }

                    if (resultStatusCode != HttpStatusCode.OK)
                    {
                        return BadRequest(new NotificacionNovedadResponseDTO
                        {
                            Status = 400,
                            Mensaje = "No se pudieron enviar las notificaciones.",
                            CantidadEnviadas = 0
                        });
                    }

                    return Ok(new NotificacionNovedadResponseDTO
                    {
                        Status = 200,
                        Mensaje = listaPush.Count.ToString() + " Notificaciones Enviadas!",
                        CantidadEnviadas = listaPush.Count
                    });
                }

                return BadRequest(new NotificacionNovedadResponseDTO
                {
                    Status = 400,
                    Mensaje = "No se pudieron enviar las notificaciones.",
                    CantidadEnviadas = 0
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new NotificacionNovedadResponseDTO
                {
                    Status = 500,
                    Mensaje = "No se pudieron enviar las notificaciones.",
                    CantidadEnviadas = 0
                });
            }
        }

        private NovedadesDTO MapToDTO(Novedades novedad)
        {
            if (novedad == null)
                return null;

            return new NovedadesDTO
            {
                Id = novedad.Id,
                Fecha = novedad.Fecha,
                Titulo = novedad.Titulo,
                Subtitulo = novedad.Subtitulo,
                Foto = novedad.Foto,
                Texto = novedad.Texto,
                TextoBoton = novedad.TextoBoton,
                Publica = novedad.Publica,
                EmpresaId = novedad.Empresa != null ? novedad.Empresa.Id : (int?)null,
                ColorId = novedad.Color != null ? novedad.Color.Id : (int?)null
            };
        }
    }
}