using DAL.Data;
using DAL.DTOs;
using DAL.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.Areas.Core.Endpoints
{
    [Area("Core")]
    [Route("api/endpoint/banners")]
    [ApiController]
    public class BannersEndpointController : ControllerBase
    {
        private readonly EstanciasContext _context;
        private readonly IHostingEnvironment _env;

        public BannersEndpointController(EstanciasContext context, IHostingEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public IActionResult GetAll(
            string buscar = "",
            int pagina = 1,
            int cantidadPorPagina = 10)
        {
            try
            {
                if (pagina <= 0)
                    pagina = 1;

                if (cantidadPorPagina <= 0)
                    cantidadPorPagina = 10;

                var usuario = _context.Usuarios.FirstOrDefault(x => x.Email == User.Identity.Name);

                var query = _context.Banners.AsQueryable();

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
                    string texto = buscar.Trim().ToLower();

                    query = query.Where(x =>
                        ((x.Titulo ?? "").ToLower().Contains(texto)) ||
                        ((x.Texto ?? "").ToLower().Contains(texto))
                    );
                }

                int total = query.Count();

                var banners = query
                    .OrderBy(x => x.Orden)
                    .ThenByDescending(x => x.Id)
                    .Skip((pagina - 1) * cantidadPorPagina)
                    .Take(cantidadPorPagina)
                    .ToList()
                    .Select(x => MapearBannerDTO(x))
                    .ToList();

                var response = new BannersListadoDTO
                {
                    Status = 200,
                    Mensaje = "Banners obtenidos correctamente.",
                    Banners = banners,
                    TotalRegistros = total,
                    Pagina = pagina,
                    CantidadPorPagina = cantidadPorPagina
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new RequestApi
                {
                    Status = 400,
                    Mensaje = "Hubo un error al obtener los banners. " + ex.Message
                });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var banner = _context.Banners.FirstOrDefault(x => x.Id == id);

                if (banner == null)
                {
                    return NotFound(new RequestApi
                    {
                        Status = 404,
                        Mensaje = "No se encontró el banner."
                    });
                }

                return Ok(new BannerResponseDTO
                {
                    Status = 200,
                    Mensaje = "Banner obtenido correctamente.",
                    Banner = MapearBannerDTO(banner)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new RequestApi
                {
                    Status = 400,
                    Mensaje = "Hubo un error al obtener el banner. " + ex.Message
                });
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] BannerCrearDTO dto)
        {
            try
            {
                var usuario = _context.Usuarios.FirstOrDefault(x => x.Email == User.Identity.Name);

                var banner = new Banners();

                banner.Titulo = dto.Titulo;
                banner.Subtitulo = string.IsNullOrWhiteSpace(dto.Subtitulo) ? " " : dto.Subtitulo;
                banner.Texto = string.IsNullOrWhiteSpace(dto.Texto) ? " " : dto.Texto;
                banner.TextoBoton = string.IsNullOrWhiteSpace(dto.TextoBoton) ? " " : dto.TextoBoton;

                banner.Fecha = dto.Fecha;
                banner.FechaDesde = dto.FechaDesde;

                banner.Publico = dto.Publico;
                banner.Link = dto.Link;
                banner.LinkExterno = dto.LinkExterno;

                banner.BannerFijo = dto.BannerFijo;

                banner.Vencimiento = false;

                if (dto.TieneFechaVencimiento)
                {
                    banner.Vencimiento = true;
                    banner.FechaHasta = dto.FechaHasta;
                }
                else
                {
                    banner.FechaHasta = null;
                }

                banner.EsVideo = false;
                banner.Video = null;
                banner.Orden = dto.Orden;

                if (usuario != null && usuario.Clientes != null && usuario.Clientes.Empresa != null)
                {
                    banner.Empresa = usuario.Clientes.Empresa;
                }

                _context.Banners.Add(banner);
                _context.SaveChanges();

                return Ok(new BannerResponseDTO
                {
                    Status = 200,
                    Mensaje = "Se registró correctamente el banner.",
                    Banner = MapearBannerDTO(banner)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new RequestApi
                {
                    Status = 400,
                    Mensaje = "Hubo un error al registrar el banner. " + ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] BannerEditarDTO dto)
        {
            try
            {
                var banner = _context.Banners.FirstOrDefault(x => x.Id == id);

                if (banner == null)
                {
                    return NotFound(new RequestApi
                    {
                        Status = 404,
                        Mensaje = "No se encontró el banner."
                    });
                }

                banner.Titulo = dto.Titulo;
                banner.Subtitulo = string.IsNullOrWhiteSpace(dto.Subtitulo) ? " " : dto.Subtitulo;
                banner.Texto = string.IsNullOrWhiteSpace(dto.Texto) ? " " : dto.Texto;
                banner.TextoBoton = string.IsNullOrWhiteSpace(dto.TextoBoton) ? " " : dto.TextoBoton;

                banner.Fecha = dto.Fecha;
                banner.Publico = dto.Publico;
                banner.Link = dto.Link;
                banner.FechaDesde = dto.FechaDesde;
                banner.LinkExterno = dto.LinkExterno;

                banner.BannerFijo = dto.BannerFijo;

                if (dto.TieneFechaVencimiento)
                {
                    banner.FechaHasta = dto.FechaHasta;
                    banner.Vencimiento = true;
                }
                else
                {
                    banner.FechaHasta = null;
                    banner.Vencimiento = false;
                }

                banner.Orden = dto.Orden;

                _context.SaveChanges();

                return Ok(new BannerResponseDTO
                {
                    Status = 200,
                    Mensaje = "Se modificó correctamente el banner.",
                    Banner = MapearBannerDTO(banner)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new RequestApi
                {
                    Status = 400,
                    Mensaje = "Hubo un error al modificar el banner. " + ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var banner = _context.Banners.FirstOrDefault(x => x.Id == id);

                if (banner == null)
                {
                    return NotFound(new RequestApi
                    {
                        Status = 404,
                        Mensaje = "No se encontró el banner."
                    });
                }

                if (banner.EsVideo && !string.IsNullOrWhiteSpace(banner.Video))
                {
                    EliminarArchivoVideo(banner.Video);
                }

                _context.Banners.Remove(banner);
                _context.SaveChanges();

                return Ok(new RequestApi
                {
                    Status = 200,
                    Mensaje = "Se eliminó correctamente el banner."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new RequestApi
                {
                    Status = 400,
                    Mensaje = "Hubo un error al eliminar el banner. " + ex.Message
                });
            }
        }

        [HttpPost("{id}/imagen")]
        public async Task<IActionResult> CambiarImagen(int id, IFormFile file)
        {
            try
            {
                var banner = await _context.Banners.FindAsync(id);

                if (banner == null)
                {
                    return NotFound(new RequestApi
                    {
                        Status = 404,
                        Mensaje = "No se encontró el banner."
                    });
                }

                if (file == null || file.Length == 0)
                {
                    return BadRequest(new RequestApi
                    {
                        Status = 400,
                        Mensaje = "Debe enviar una imagen."
                    });
                }

                if (banner.EsVideo && !string.IsNullOrWhiteSpace(banner.Video))
                {
                    EliminarArchivoVideo(banner.Video);
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
                    Mensaje = "Se cargó correctamente la imagen.",
                    Banner = MapearBannerDTO(banner)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new RequestApi
                {
                    Status = 400,
                    Mensaje = "Hubo un error al cargar la imagen. " + ex.Message
                });
            }
        }

        [HttpPost("{id}/video")]
        public IActionResult CambiarVideo(int id, IFormFile file)
        {
            try
            {
                var banner = _context.Banners.FirstOrDefault(x => x.Id == id);

                if (banner == null)
                {
                    return NotFound(new RequestApi
                    {
                        Status = 404,
                        Mensaje = "No se encontró el banner."
                    });
                }

                if (file == null || file.Length == 0)
                {
                    return BadRequest(new RequestApi
                    {
                        Status = 400,
                        Mensaje = "Debe enviar un video."
                    });
                }

                if (banner.EsVideo && !string.IsNullOrWhiteSpace(banner.Video))
                {
                    EliminarArchivoVideo(banner.Video);
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

                var urlBase = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host;

                banner.Video = Url.Content(urlBase + "/uploads/" + uniqueFileName);
                banner.Foto = null;
                banner.EsVideo = true;

                _context.Banners.Update(banner);
                _context.SaveChanges();

                return Ok(new BannerResponseDTO
                {
                    Status = 200,
                    Mensaje = "Se cargó correctamente el video.",
                    Banner = MapearBannerDTO(banner)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new RequestApi
                {
                    Status = 400,
                    Mensaje = "Hubo un error al cargar el video. " + ex.Message
                });
            }
        }

        [HttpPost("modificar-orden")]
        public IActionResult ModificarOrden([FromBody] BannerOrdenDTO dto)
        {
            try
            {
                var banner = _context.Banners.FirstOrDefault(x => x.Id == dto.Id);

                if (banner == null)
                {
                    return NotFound(new RequestApi
                    {
                        Status = 404,
                        Mensaje = "No se encontró el banner."
                    });
                }

                banner.Orden = dto.Orden;

                _context.Banners.Update(banner);
                _context.SaveChanges();

                return Ok(new BannerResponseDTO
                {
                    Status = 200,
                    Mensaje = "Se modificó el orden del banner.",
                    Banner = MapearBannerDTO(banner)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new RequestApi
                {
                    Status = 400,
                    Mensaje = "No se pudo modificar el orden del banner. " + ex.Message
                });
            }
        }

        [HttpPost("{id}/notificacion")]
        public IActionResult Notificacion(int id)
        {
            try
            {
                var usuario = _context.Usuarios.FirstOrDefault(x => x.Email == User.Identity.Name);

                if (usuario != null && usuario.Clientes != null && usuario.Clientes.Empresa != null)
                {
                    var listaPush = _context.Clientes.Where(x => x.Empresa.Id == usuario.Clientes.Empresa.Id);
                    var banner = _context.Banners.FirstOrDefault(x => x.Id == id);

                    if (banner == null)
                    {
                        return NotFound(new RequestApi
                        {
                            Status = 404,
                            Mensaje = "No se encontró el banner."
                        });
                    }

                    return Ok(new RequestApi
                    {
                        Status = 200,
                        Mensaje = listaPush.Count().ToString() + " Notificaciones Enviadas!"
                    });
                }

                return BadRequest(new RequestApi
                {
                    Status = 400,
                    Mensaje = "No se pudieron enviar las notificaciones."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new RequestApi
                {
                    Status = 400,
                    Mensaje = "No se pudieron enviar las notificaciones. " + ex.Message
                });
            }
        }

        private BannersDTO MapearBannerDTO(Banners banner)
        {
            var dto = new BannersDTO();

            dto.Id = banner.Id;
            dto.Titulo = banner.Titulo ?? "";
            dto.Subtitulo = banner.Subtitulo ?? "";
            dto.Texto = banner.Texto ?? "";
            dto.TextoBoton = banner.TextoBoton ?? "";

            dto.Fecha = banner.Fecha;
            dto.FechaDesde = banner.FechaDesde;

            if (banner.FechaHasta.HasValue)
            {
                dto.FechaHasta = banner.FechaHasta.Value.ToString("yyyy-MM-dd");
            }
            else
            {
                dto.FechaHasta = "";
            }

            dto.Publico = banner.Publico;
            dto.LinkExterno = banner.LinkExterno;
            dto.BannerFijo = banner.BannerFijo;
            dto.Vencimiento = banner.Vencimiento;
            dto.EsVideo = banner.EsVideo;

            dto.Link = banner.Link ?? "";
            dto.Video = banner.Video ?? "";
            dto.Foto = banner.Foto ?? "";

            dto.Orden = banner.Orden;

            return dto;
        }

        private void EliminarArchivoVideo(string videoUrl)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(videoUrl))
                    return;

                Uri uri = new Uri(videoUrl);
                string nombreArchivo = Path.GetFileName(uri.LocalPath);

                if (string.IsNullOrWhiteSpace(nombreArchivo))
                    return;

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
    }
}