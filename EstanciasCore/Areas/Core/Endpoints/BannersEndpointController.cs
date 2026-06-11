using DAL.Data;
using DAL.DTOs;
using DAL.Models;
using EstanciasCore.API.Filters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.Areas.Core.Endpoints
{
    [TypeFilter(typeof(EndpointUatAuthorizeAttribute))]
    [ApiController]
    [Route("endpoint/banners")]
    public class BannersEndpointController : ControllerBase
    {
        private readonly EstanciasContext _context;
        private readonly IHostingEnvironment _env;

        public BannersEndpointController(EstanciasContext context, IHostingEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: endpoint/banners/listar?pagina=1&cantidad=10&buscar=promo
        [HttpGet("listar")]
        public async Task<IActionResult> Listar(
            [FromQuery] int pagina = 1,
            [FromQuery] int cantidad = 10,
            [FromQuery] string buscar = "")
        {
            try
            {
                if (pagina < 1)
                    pagina = 1;

                if (cantidad < 1)
                    cantidad = 10;

                buscar = buscar ?? "";

                string emailUsuario = User != null && User.Identity != null
                    ? User.Identity.Name
                    : null;

                var usuario = _context.Usuarios
                    .Include(x => x.Clientes)
                        .ThenInclude(x => x.Empresa)
                    .FirstOrDefault(x => x.Email == emailUsuario);

                var query = _context.Banners
                    .Include(x => x.Marcas)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(buscar))
                {
                    string texto = buscar.Trim();

                    query = query.Where(x =>
                        ((x.Titulo ?? "").Contains(texto)) ||
                        ((x.Texto ?? "").Contains(texto))
                    );
                }

                /*
                 * IMPORTANTE:
                 * Si el UAT no carga User.Identity.Name, usuario queda null.
                 * En ese caso NO filtramos por Empresa == null, porque si no trae 0 registros.
                 * Si sí encuentra usuario con empresa, trae solo los banners de esa empresa.
                 */
                if (usuario != null && usuario.Clientes != null && usuario.Clientes.Empresa != null)
                {
                    int empresaId = usuario.Clientes.Empresa.Id;

                    query = query.Where(x =>
                        x.Empresa != null &&
                        x.Empresa.Id == empresaId
                    );
                }

                var totalRegistros = await query.CountAsync();

                var banners = await query
                    .OrderBy(x => x.Orden)
                    .ThenByDescending(x => x.Id)
                    .Skip((pagina - 1) * cantidad)
                    .Take(cantidad)
                    .Select(x => new BannerListadoDTO
                    {
                        Id = x.Id,
                        Titulo = x.Titulo ?? "",
                        Subtitulo = x.Subtitulo ?? "",
                        Texto = x.Texto ?? "",
                        TextoBoton = x.TextoBoton ?? "",
                        Fecha = x.Fecha,
                        FechaDesde = x.FechaDesde,
                        FechaHasta = x.FechaHasta,
                        Publico = x.Publico,
                        Link = x.Link ?? "",
                        LinkExterno = x.LinkExterno,
                        BannerFijo = x.BannerFijo,
                        Vencimiento = x.Vencimiento,
                        EsVideo = x.EsVideo,
                        Video = x.Video ?? "",
                        Foto = x.Foto ?? "",
                        Orden = x.Orden,
                        Marca = x.Marcas == null ? null : new MarcaBannerDTO
                        {
                            Id = x.Marcas.Id,
                            Nombre = x.Marcas.Nombre ?? ""
                        }
                    })
                    .ToListAsync();

                var totalPaginas = totalRegistros == 0
                    ? 1
                    : (int)Math.Ceiling(totalRegistros / (decimal)cantidad);

                return Ok(new BannerListadoResponseDTO
                {
                    Status = 200,
                    Mensaje = "Listado de Banners obtenido correctamente.",
                    Data = new BannerListadoDataDTO
                    {
                        TotalRegistros = totalRegistros,
                        PaginaActual = pagina,
                        CantidadPorPagina = cantidad,
                        TotalPaginas = totalPaginas,
                        Banners = banners
                    }
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new BannerResponseDTO
                {
                    Status = 500,
                    Mensaje = "Hubo un error al obtener el listado de Banners. Intentelo nuevamente mas tarde."
                });
            }
        }

        // GET: endpoint/banners/obtener/5
        [HttpGet("obtener/{id}")]
        public async Task<IActionResult> Obtener(int id)
        {
            try
            {
                var banner = await _context.Banners
                    .Include(x => x.Marcas)
                    .Where(x => x.Id == id)
                    .Select(x => new BannerListadoDTO
                    {
                        Id = x.Id,
                        Titulo = x.Titulo ?? "",
                        Subtitulo = x.Subtitulo ?? "",
                        Texto = x.Texto ?? "",
                        TextoBoton = x.TextoBoton ?? "",
                        Fecha = x.Fecha,
                        FechaDesde = x.FechaDesde,
                        FechaHasta = x.FechaHasta,
                        Publico = x.Publico,
                        Link = x.Link ?? "",
                        LinkExterno = x.LinkExterno,
                        BannerFijo = x.BannerFijo,
                        Vencimiento = x.Vencimiento,
                        EsVideo = x.EsVideo,
                        Video = x.Video ?? "",
                        Foto = x.Foto ?? "",
                        Orden = x.Orden,
                        Marca = x.Marcas == null ? null : new MarcaBannerDTO
                        {
                            Id = x.Marcas.Id,
                            Nombre = x.Marcas.Nombre ?? ""
                        }
                    })
                    .FirstOrDefaultAsync();

                if (banner == null)
                {
                    return NotFound(new BannerResponseDTO
                    {
                        Status = 404,
                        Mensaje = "No se encontró el Banner solicitado."
                    });
                }

                return Ok(new BannerItemResponseDTO
                {
                    Status = 200,
                    Mensaje = "Banner obtenido correctamente.",
                    Banner = banner
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new BannerResponseDTO
                {
                    Status = 500,
                    Mensaje = "Hubo un error al obtener el Banner. Intentelo nuevamente mas tarde."
                });
            }
        }

        // GET: endpoint/banners/marcas
        [HttpGet("marcas")]
        public async Task<IActionResult> Marcas()
        {
            try
            {
                var marcas = await _context.Marcas
                    .Where(x => x.Activo)
                    .OrderBy(x => x.Orden)
                    .Select(x => new MarcaBannerDTO
                    {
                        Id = x.Id,
                        Nombre = x.Nombre ?? ""
                    })
                    .ToListAsync();

                return Ok(new
                {
                    Status = 200,
                    Mensaje = "Listado de marcas obtenido correctamente.",
                    Data = marcas
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new BannerResponseDTO
                {
                    Status = 500,
                    Mensaje = "Hubo un error al obtener las Marcas. Intentelo nuevamente mas tarde."
                });
            }
        }

        // POST: endpoint/banners/crear
        [HttpPost("crear")]
        public async Task<IActionResult> Crear([FromBody] BannerCrearDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new BannerResponseDTO
                    {
                        Status = 400,
                        Mensaje = "Los datos del Banner son obligatorios."
                    });
                }

                string emailUsuario = User != null && User.Identity != null
                    ? User.Identity.Name
                    : null;

                var usuario = _context.Usuarios
                    .Include(x => x.Clientes)
                        .ThenInclude(x => x.Empresa)
                    .FirstOrDefault(x => x.Email == emailUsuario);

                var banner = new Banners();

                banner.Titulo = dto.Titulo ?? "";
                banner.Subtitulo = dto.Subtitulo ?? "";
                banner.Texto = dto.Texto ?? "";
                banner.TextoBoton = dto.TextoBoton ?? "";

                banner.Fecha = dto.Fecha ?? DateTime.Now;
                banner.FechaDesde = dto.FechaDesde ?? DateTime.Now;
                banner.FechaHasta = dto.FechaHasta;

                banner.Publico = dto.Publico;
                banner.Link = dto.Link ?? "";
                banner.LinkExterno = dto.LinkExterno;
                banner.Orden = dto.Orden;

                if (usuario != null && usuario.Clientes != null)
                {
                    banner.Empresa = usuario.Clientes.Empresa;
                }

                banner.Vencimiento = false;

                if (dto.BannersFijo == 1)
                {
                    banner.BannerFijo = true;
                }
                else
                {
                    banner.BannerFijo = false;
                }

                if (dto.TieneFechaVencimiento == "on")
                {
                    banner.Vencimiento = true;
                }

                if (dto.MarcasId.HasValue && dto.MarcasId.Value > 0)
                {
                    banner.Marcas = await _context.Marcas.FindAsync(dto.MarcasId.Value);
                }

                banner.EsVideo = false;

                await _context.Banners.AddAsync(banner);
                await _context.SaveChangesAsync();

                return Ok(new BannerItemResponseDTO
                {
                    Status = 200,
                    Mensaje = "Se registró correctamente el Banner.",
                    Banner = new BannerListadoDTO
                    {
                        Id = banner.Id,
                        Titulo = banner.Titulo ?? "",
                        Subtitulo = banner.Subtitulo ?? "",
                        Texto = banner.Texto ?? "",
                        TextoBoton = banner.TextoBoton ?? "",
                        Fecha = banner.Fecha,
                        FechaDesde = banner.FechaDesde,
                        FechaHasta = banner.FechaHasta,
                        Publico = banner.Publico,
                        Link = banner.Link ?? "",
                        LinkExterno = banner.LinkExterno,
                        BannerFijo = banner.BannerFijo,
                        Vencimiento = banner.Vencimiento,
                        EsVideo = banner.EsVideo,
                        Video = banner.Video ?? "",
                        Foto = banner.Foto ?? "",
                        Orden = banner.Orden,
                        Marca = banner.Marcas == null ? null : new MarcaBannerDTO
                        {
                            Id = banner.Marcas.Id,
                            Nombre = banner.Marcas.Nombre ?? ""
                        }
                    }
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new BannerResponseDTO
                {
                    Status = 500,
                    Mensaje = "Hubo un error al registrar el Banner. Intentelo nuevamente mas tarde."
                });
            }
        }

        // PUT: endpoint/banners/editar/5
        [HttpPut("editar/{id}")]
        public async Task<IActionResult> Editar(int id, [FromBody] BannerEditarDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new BannerResponseDTO
                    {
                        Status = 400,
                        Mensaje = "Los datos del Banner son obligatorios."
                    });
                }

                if (id != dto.Id)
                {
                    return BadRequest(new BannerResponseDTO
                    {
                        Status = 400,
                        Mensaje = "El Id enviado por ruta no coincide con el Id del Banner."
                    });
                }

                Banners bannerUpdate = await _context.Banners
                    .Include(x => x.Marcas)
                    .Where(s => s.Id == id)
                    .FirstOrDefaultAsync();

                if (bannerUpdate == null)
                {
                    return NotFound(new BannerResponseDTO
                    {
                        Status = 404,
                        Mensaje = "No se encontró el Banner solicitado."
                    });
                }

                bannerUpdate.Titulo = dto.Titulo ?? "";
                bannerUpdate.Subtitulo = dto.Subtitulo == null ? " " : dto.Subtitulo;
                bannerUpdate.Texto = dto.Texto == null ? " " : dto.Texto;
                bannerUpdate.TextoBoton = dto.TextoBoton == null ? " " : dto.TextoBoton;

                bannerUpdate.Fecha = dto.Fecha ?? bannerUpdate.Fecha;
                bannerUpdate.FechaDesde = dto.FechaDesde ?? bannerUpdate.FechaDesde;

                bannerUpdate.Publico = dto.Publico;
                bannerUpdate.Link = dto.Link ?? "";
                bannerUpdate.LinkExterno = dto.LinkExterno;
                bannerUpdate.Orden = dto.Orden;

                if (dto.BannerFijo == 1)
                {
                    bannerUpdate.BannerFijo = true;
                }
                else
                {
                    bannerUpdate.BannerFijo = false;
                }

                if (dto.TieneFechaVencimiento == "on")
                {
                    bannerUpdate.FechaHasta = dto.FechaHasta;
                    bannerUpdate.Vencimiento = true;
                }
                else
                {
                    bannerUpdate.FechaHasta = null;
                    bannerUpdate.Vencimiento = false;
                }

                if (dto.MarcasId.HasValue && dto.MarcasId.Value > 0)
                {
                    bannerUpdate.Marcas = await _context.Marcas.FindAsync(dto.MarcasId.Value);
                }
                else
                {
                    bannerUpdate.Marcas = null;
                }

                _context.Banners.Update(bannerUpdate);
                await _context.SaveChangesAsync();

                return Ok(new BannerItemResponseDTO
                {
                    Status = 200,
                    Mensaje = "Se modificó correctamente el Banner.",
                    Banner = new BannerListadoDTO
                    {
                        Id = bannerUpdate.Id,
                        Titulo = bannerUpdate.Titulo ?? "",
                        Subtitulo = bannerUpdate.Subtitulo ?? "",
                        Texto = bannerUpdate.Texto ?? "",
                        TextoBoton = bannerUpdate.TextoBoton ?? "",
                        Fecha = bannerUpdate.Fecha,
                        FechaDesde = bannerUpdate.FechaDesde,
                        FechaHasta = bannerUpdate.FechaHasta,
                        Publico = bannerUpdate.Publico,
                        Link = bannerUpdate.Link ?? "",
                        LinkExterno = bannerUpdate.LinkExterno,
                        BannerFijo = bannerUpdate.BannerFijo,
                        Vencimiento = bannerUpdate.Vencimiento,
                        EsVideo = bannerUpdate.EsVideo,
                        Video = bannerUpdate.Video ?? "",
                        Foto = bannerUpdate.Foto ?? "",
                        Orden = bannerUpdate.Orden,
                        Marca = bannerUpdate.Marcas == null ? null : new MarcaBannerDTO
                        {
                            Id = bannerUpdate.Marcas.Id,
                            Nombre = bannerUpdate.Marcas.Nombre ?? ""
                        }
                    }
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new BannerResponseDTO
                {
                    Status = 500,
                    Mensaje = "Hubo un error al modificar el Banner. Intentelo nuevamente mas tarde."
                });
            }
        }

        // DELETE: endpoint/banners/borrar/5
        [HttpDelete("borrar/{id}")]
        public async Task<IActionResult> Borrar(int id)
        {
            try
            {
                var banner = await _context.Banners
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync();

                if (banner == null)
                {
                    return NotFound(new BannerResponseDTO
                    {
                        Status = 404,
                        Mensaje = "No se encontró el Banner solicitado."
                    });
                }

                _context.Banners.Remove(banner);
                await _context.SaveChangesAsync();

                return Ok(new BannerResponseDTO
                {
                    Status = 200,
                    Mensaje = "Se eliminó correctamente el Banner."
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new BannerResponseDTO
                {
                    Status = 500,
                    Mensaje = "Hubo un error al eliminar el Banner."
                });
            }
        }

        // POST: endpoint/banners/cambiar-imagen/5
        [HttpPost("cambiar-imagen/{id}")]
        public async Task<IActionResult> CambiarImagen(int id, IFormFile file)
        {
            try
            {
                var banner = await _context.Banners.FindAsync(id);

                if (banner == null)
                {
                    return NotFound(new BannerResponseDTO
                    {
                        Status = 404,
                        Mensaje = "No se encontró el Banner solicitado."
                    });
                }

                if (file == null || file.Length == 0)
                {
                    return BadRequest(new BannerResponseDTO
                    {
                        Status = 400,
                        Mensaje = "Debe enviar una imagen."
                    });
                }

                if (banner.EsVideo && banner.Video != null)
                {
                    Uri uri = new Uri(banner.Video);
                    string nombreArchivo = Path.GetFileName(uri.LocalPath);

                    try
                    {
                        string uploadsPath = Path.Combine(_env.WebRootPath, "uploads");
                        string rutaArchivo = Path.Combine(uploadsPath, nombreArchivo);

                        if (System.IO.File.Exists(rutaArchivo))
                        {
                            System.IO.File.Delete(rutaArchivo);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }

                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);

                    banner.EsVideo = false;
                    banner.Video = null;
                    banner.Foto = Convert.ToBase64String(memoryStream.ToArray());

                    await _context.SaveChangesAsync();
                }

                return Ok(new BannerResponseDTO
                {
                    Status = 200,
                    Mensaje = "Se cambió correctamente la imagen del Banner."
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new BannerResponseDTO
                {
                    Status = 500,
                    Mensaje = "Hubo un error al cambiar la imagen del Banner."
                });
            }
        }

        // POST: endpoint/banners/cambiar-video/5
        [HttpPost("cambiar-video/{id}")]
        public async Task<IActionResult> CambiarVideo(int id, IFormFile file)
        {
            try
            {
                var banner = await _context.Banners.FindAsync(id);

                if (banner == null)
                {
                    return NotFound(new BannerResponseDTO
                    {
                        Status = 404,
                        Mensaje = "No se encontró el Banner solicitado."
                    });
                }

                if (file == null || file.Length == 0)
                {
                    return BadRequest(new BannerResponseDTO
                    {
                        Status = 400,
                        Mensaje = "Debe enviar un video."
                    });
                }

                if (banner.EsVideo && banner.Video != null)
                {
                    Uri uri = new Uri(banner.Video);
                    string nombreArchivo = Path.GetFileName(uri.LocalPath);

                    try
                    {
                        string uploadsPath = Path.Combine(_env.WebRootPath, "uploads");
                        string rutaArchivo = Path.Combine(uploadsPath, nombreArchivo);

                        if (System.IO.File.Exists(rutaArchivo))
                        {
                            System.IO.File.Delete(rutaArchivo);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                Directory.CreateDirectory(uploadsFolder);

                string cadenaSinEspacios = file.FileName.Replace(" ", "_");
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + cadenaSinEspacios;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                var urlBase = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

                banner.Video = Url.Content(urlBase + "/uploads/" + uniqueFileName);
                banner.Foto = null;
                banner.EsVideo = true;

                _context.Banners.Update(banner);
                await _context.SaveChangesAsync();

                return Ok(new BannerResponseDTO
                {
                    Status = 200,
                    Mensaje = "Se cargó correctamente el video."
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new BannerResponseDTO
                {
                    Status = 500,
                    Mensaje = "Hubo un error al cargar el video."
                });
            }
        }

        // POST: endpoint/banners/notificacion/5
        [HttpPost("notificacion/{id}")]
        public IActionResult Notificacion(int id)
        {
            try
            {
                string emailUsuario = User != null && User.Identity != null
                    ? User.Identity.Name
                    : null;

                var usuario = _context.Usuarios
                    .Include(x => x.Clientes)
                        .ThenInclude(x => x.Empresa)
                    .FirstOrDefault(x => x.Email == emailUsuario);

                var existeBanner = _context.Banners.Any(x => x.Id == id);

                if (!existeBanner)
                {
                    return NotFound(new BannerResponseDTO
                    {
                        Status = 404,
                        Mensaje = "No se encontró el Banner solicitado."
                    });
                }

                if (usuario != null && usuario.Clientes != null && usuario.Clientes.Empresa != null)
                {
                    var listaPush = _context.Clientes
                        .Where(x => x.Empresa.Id == usuario.Clientes.Empresa.Id);

                    return Ok(new BannerResponseDTO
                    {
                        Status = 200,
                        Mensaje = listaPush.Count().ToString() + " Notificaciones Enviadas!"
                    });
                }

                return BadRequest(new BannerResponseDTO
                {
                    Status = 400,
                    Mensaje = "No se pudieron enviar las notificaciones porque no se encontró empresa asociada al usuario."
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new BannerResponseDTO
                {
                    Status = 500,
                    Mensaje = "No se pudieron enviar las notificaciones."
                });
            }
        }

        // POST: endpoint/banners/modificar-orden
        [HttpPost("modificar-orden")]
        public async Task<IActionResult> ModificarOrden([FromBody] BannerOrdenDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new BannerResponseDTO
                    {
                        Status = 400,
                        Mensaje = "Los datos son obligatorios."
                    });
                }

                var banner = await _context.Banners
                    .Where(x => x.Id == dto.Id)
                    .FirstOrDefaultAsync();

                if (banner == null)
                {
                    return NotFound(new BannerResponseDTO
                    {
                        Status = 404,
                        Mensaje = "No se encontró el Banner solicitado."
                    });
                }

                banner.Orden = dto.Orden;

                _context.Banners.Update(banner);
                await _context.SaveChangesAsync();

                return Ok(new BannerResponseDTO
                {
                    Status = 200,
                    Mensaje = "Se modificó el orden del Banner."
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new BannerResponseDTO
                {
                    Status = 500,
                    Mensaje = "No se pudo modificar el Banner."
                });
            }
        }
    }
}