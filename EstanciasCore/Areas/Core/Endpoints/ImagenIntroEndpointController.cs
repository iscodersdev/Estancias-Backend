using DAL.Data;
using DAL.DTOs;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.Areas.Core.Endpoints
{
    [Area("Core")]
    [Route("endpoint/imagen-intro")]
    [ApiController]
    public class ImagenIntroEndpointController : ControllerBase
    {
        private readonly EstanciasContext _context;

        public ImagenIntroEndpointController(EstanciasContext context)
        {
            _context = context;
        }

        [HttpGet("listar")]
        public async Task<IActionResult> Listar(
            string buscar = "",
            int pagina = 1,
            int cantidad = 10)
        {
            try
            {
                if (pagina < 1)
                {
                    pagina = 1;
                }

                if (cantidad < 1)
                {
                    cantidad = 10;
                }

                if (buscar == null)
                {
                    buscar = string.Empty;
                }

                buscar = buscar.Trim();

                var usuario = await _context.Usuarios
                    .Include(x => x.Clientes)
                    .ThenInclude(x => x.Empresa)
                    .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

                var query = _context.ImagenIntro
                    .Include(x => x.Empresa)
                    .AsQueryable();

                if (usuario == null || usuario.Clientes == null || usuario.Clientes.Empresa == null)
                {
                    query = query.Where(x => x.Empresa == null);
                }
                else
                {
                    int empresaId = usuario.Clientes.Empresa.Id;
                    query = query.Where(x => x.Empresa != null && x.Empresa.Id == empresaId);
                }

                if (!string.IsNullOrWhiteSpace(buscar))
                {
                    query = query.Where(x => x.Titulo.Contains(buscar));
                }

                var totalRegistros = await query.CountAsync();

                var items = await query
                    .OrderBy(x => x.Orden)
                    .ThenByDescending(x => x.Fecha)
                    .Skip((pagina - 1) * cantidad)
                    .Take(cantidad)
                    .Select(x => new ImagenIntroDTO
                    {
                        Id = x.Id,
                        Titulo = x.Titulo,
                        Fecha = x.Fecha,
                        Orden = x.Orden,
                        Foto = x.Foto,
                        EsVideo = x.EsVideo
                    })
                    .ToListAsync();

                var totalPaginas = (int)Math.Ceiling((double)totalRegistros / cantidad);

                var response = new ImagenIntroListadoDTO
                {
                    Items = items,
                    TotalRegistros = totalRegistros,
                    PaginaActual = pagina,
                    CantidadPorPagina = cantidad,
                    TotalPaginas = totalPaginas,
                    Buscar = buscar
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ImagenIntroResponseDTO
                {
                    Status = 500,
                    Mensaje = "Hubo un error al obtener el listado de Imagen Intro. " + ex.Message
                });
            }
        }

        [HttpGet("obtener/{id}")]
        public async Task<IActionResult> Obtener(int id)
        {
            try
            {
                var imagenIntro = await _context.ImagenIntro
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (imagenIntro == null)
                {
                    return NotFound(new ImagenIntroResponseDTO
                    {
                        Status = 404,
                        Mensaje = "No se encontró la Imagen Intro solicitada."
                    });
                }

                var response = new ImagenIntroResponseDTO
                {
                    Status = 200,
                    Mensaje = "Imagen Intro obtenida correctamente.",
                    Data = new ImagenIntroDTO
                    {
                        Id = imagenIntro.Id,
                        Titulo = imagenIntro.Titulo,
                        Fecha = imagenIntro.Fecha,
                        Orden = imagenIntro.Orden,
                        Foto = imagenIntro.Foto,
                        EsVideo = imagenIntro.EsVideo
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ImagenIntroResponseDTO
                {
                    Status = 500,
                    Mensaje = "Hubo un error al obtener la Imagen Intro. " + ex.Message
                });
            }
        }

        [HttpPost("crear")]
        public async Task<IActionResult> Crear([FromBody] ImagenIntroCreateDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new ImagenIntroResponseDTO
                    {
                        Status = 400,
                        Mensaje = "Los datos enviados no son válidos."
                    });
                }

                if (string.IsNullOrWhiteSpace(dto.Titulo))
                {
                    return BadRequest(new ImagenIntroResponseDTO
                    {
                        Status = 400,
                        Mensaje = "El título es obligatorio."
                    });
                }

                ImagenIntro modificarAnterior = await _context.ImagenIntro
                    .Where(s => s.Orden == dto.Orden)
                    .FirstOrDefaultAsync();

                if (modificarAnterior != null)
                {
                    modificarAnterior.Orden = 0;
                    _context.ImagenIntro.Update(modificarAnterior);
                    await _context.SaveChangesAsync();
                }

                var usuario = await _context.Usuarios
                    .Include(x => x.Clientes)
                    .ThenInclude(x => x.Empresa)
                    .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

                var imagenIntro = new ImagenIntro
                {
                    Titulo = dto.Titulo,
                    Fecha = dto.Fecha,
                    Orden = dto.Orden,
                    Empresa = usuario != null && usuario.Clientes != null
                        ? usuario.Clientes.Empresa
                        : null
                };

                _context.ImagenIntro.Add(imagenIntro);
                await _context.SaveChangesAsync();

                var response = new ImagenIntroResponseDTO
                {
                    Status = 200,
                    Mensaje = "Se registró correctamente la Imagen Intro.",
                    Data = new ImagenIntroDTO
                    {
                        Id = imagenIntro.Id,
                        Titulo = imagenIntro.Titulo,
                        Fecha = imagenIntro.Fecha,
                        Orden = imagenIntro.Orden,
                        Foto = imagenIntro.Foto,
                        EsVideo = imagenIntro.EsVideo
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ImagenIntroResponseDTO
                {
                    Status = 500,
                    Mensaje = "Hubo un error al registrar Imagen Intro. " + ex.Message
                });
            }
        }

        [HttpPut("editar")]
        public async Task<IActionResult> Editar([FromBody] ImagenIntroUpdateDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new ImagenIntroResponseDTO
                    {
                        Status = 400,
                        Mensaje = "Los datos enviados no son válidos."
                    });
                }

                if (string.IsNullOrWhiteSpace(dto.Titulo))
                {
                    return BadRequest(new ImagenIntroResponseDTO
                    {
                        Status = 400,
                        Mensaje = "El título es obligatorio."
                    });
                }

                ImagenIntro modificarAnterior = await _context.ImagenIntro
                    .Where(s => s.Orden == dto.Orden)
                    .FirstOrDefaultAsync();

                if (modificarAnterior != null)
                {
                    modificarAnterior.Orden = 0;
                    _context.ImagenIntro.Update(modificarAnterior);
                    await _context.SaveChangesAsync();
                }

                ImagenIntro imagenIntro = await _context.ImagenIntro
                    .Where(s => s.Id == dto.Id)
                    .FirstOrDefaultAsync();

                if (imagenIntro == null)
                {
                    return NotFound(new ImagenIntroResponseDTO
                    {
                        Status = 404,
                        Mensaje = "No se encontró la Imagen Intro solicitada."
                    });
                }

                imagenIntro.Titulo = dto.Titulo;
                imagenIntro.Orden = dto.Orden;
                imagenIntro.Fecha = dto.Fecha;

                _context.ImagenIntro.Update(imagenIntro);
                await _context.SaveChangesAsync();

                var response = new ImagenIntroResponseDTO
                {
                    Status = 200,
                    Mensaje = "Se actualizó correctamente la Imagen Intro.",
                    Data = new ImagenIntroDTO
                    {
                        Id = imagenIntro.Id,
                        Titulo = imagenIntro.Titulo,
                        Fecha = imagenIntro.Fecha,
                        Orden = imagenIntro.Orden,
                        Foto = imagenIntro.Foto,
                        EsVideo = imagenIntro.EsVideo
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ImagenIntroResponseDTO
                {
                    Status = 500,
                    Mensaje = "Hubo un error al actualizar Imagen Intro. " + ex.Message
                });
            }
        }

        [HttpDelete("borrar/{id}")]
        public async Task<IActionResult> Borrar(int id)
        {
            try
            {
                ImagenIntro imagenIntro = await _context.ImagenIntro
                    .Where(s => s.Id == id)
                    .FirstOrDefaultAsync();

                if (imagenIntro == null)
                {
                    return NotFound(new ImagenIntroDeleteResponseDTO
                    {
                        Status = 404,
                        Mensaje = "No se encontró la Imagen Intro solicitada.",
                        Eliminado = false
                    });
                }

                _context.ImagenIntro.Remove(imagenIntro);
                await _context.SaveChangesAsync();

                return Ok(new ImagenIntroDeleteResponseDTO
                {
                    Status = 200,
                    Mensaje = "Se eliminó correctamente la Imagen Intro.",
                    Eliminado = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ImagenIntroDeleteResponseDTO
                {
                    Status = 500,
                    Mensaje = "Hubo un error al eliminar Imagen Intro. " + ex.Message,
                    Eliminado = false
                });
            }
        }

        [HttpPost("editar-imagen/{id}")]
        public async Task<IActionResult> EditarImagen(int id, IFormFile file)
        {
            try
            {
                var imagenIntro = await _context.ImagenIntro.FindAsync(id);

                if (imagenIntro == null)
                {
                    return NotFound(new ImagenIntroResponseDTO
                    {
                        Status = 404,
                        Mensaje = "No se encontró la Imagen Intro solicitada."
                    });
                }

                if (file == null || file.Length == 0)
                {
                    return BadRequest(new ImagenIntroResponseDTO
                    {
                        Status = 400,
                        Mensaje = "Debe seleccionar una imagen."
                    });
                }

                imagenIntro.EsVideo = false;

                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    imagenIntro.Foto = Convert.ToBase64String(memoryStream.ToArray());
                }

                _context.ImagenIntro.Update(imagenIntro);
                await _context.SaveChangesAsync();

                return Ok(new ImagenIntroResponseDTO
                {
                    Status = 200,
                    Mensaje = "Se actualizó correctamente la imagen.",
                    Data = new ImagenIntroDTO
                    {
                        Id = imagenIntro.Id,
                        Titulo = imagenIntro.Titulo,
                        Fecha = imagenIntro.Fecha,
                        Orden = imagenIntro.Orden,
                        Foto = imagenIntro.Foto,
                        EsVideo = imagenIntro.EsVideo
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ImagenIntroResponseDTO
                {
                    Status = 500,
                    Mensaje = "Hubo un error al actualizar la imagen. " + ex.Message
                });
            }
        }

        [HttpPost("editar-video/{id}")]
        public async Task<IActionResult> EditarVideo(int id, IFormFile file)
        {
            try
            {
                var imagenIntro = await _context.ImagenIntro.FindAsync(id);

                if (imagenIntro == null)
                {
                    return NotFound(new ImagenIntroResponseDTO
                    {
                        Status = 404,
                        Mensaje = "No se encontró la Imagen Intro solicitada."
                    });
                }

                if (file == null || file.Length == 0)
                {
                    return BadRequest(new ImagenIntroResponseDTO
                    {
                        Status = 400,
                        Mensaje = "Debe seleccionar un video."
                    });
                }

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                Directory.CreateDirectory(uploadsFolder);

                string cadenaSinEspacios = file.FileName.Replace(" ", "_");
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + cadenaSinEspacios;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                var urlBase = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

                imagenIntro.Foto = urlBase + "/uploads/" + uniqueFileName;
                imagenIntro.EsVideo = true;

                _context.ImagenIntro.Update(imagenIntro);
                await _context.SaveChangesAsync();

                return Ok(new ImagenIntroResponseDTO
                {
                    Status = 200,
                    Mensaje = "Se cargó correctamente el video.",
                    Data = new ImagenIntroDTO
                    {
                        Id = imagenIntro.Id,
                        Titulo = imagenIntro.Titulo,
                        Fecha = imagenIntro.Fecha,
                        Orden = imagenIntro.Orden,
                        Foto = imagenIntro.Foto,
                        EsVideo = imagenIntro.EsVideo
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ImagenIntroResponseDTO
                {
                    Status = 500,
                    Mensaje = "Hubo un error al cargar el video. " + ex.Message
                });
            }
        }
    }
}