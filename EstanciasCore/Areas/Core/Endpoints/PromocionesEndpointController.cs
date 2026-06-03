using DAL.Data;
using DAL.DTOs;
using DAL.Models;
using EstanciasCore.API.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.Areas.Core.Endpoints
{
    [Area("Core")]
    [TypeFilter(typeof(EndpointUatAuthorizeAttribute))]
    [Route("endpoint/promociones")]
    [ApiController]
    public class PromocionesEndpointController : ControllerBase
    {
        private readonly EstanciasContext _context;

        public PromocionesEndpointController(EstanciasContext context)
        {
            _context = context;
        }

        // GET: endpoint/promociones?buscar=&pagina=1&cantidad=10
        [HttpGet]
        public async Task<IActionResult> ObtenerPromociones(
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
                    buscar = "";
                }

                var usuario = await _context.Usuarios
                    .Include(x => x.Clientes)
                        .ThenInclude(x => x.Empresa)
                    .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

                IQueryable<Promociones> query;

                if (usuario == null || usuario.Clientes == null || usuario.Clientes.Empresa == null)
                {
                    query = _context.Promociones
                        .Include(x => x.Empresa)
                        .Where(x =>
                            x.Empresa == null &&
                            (
                                (x.Titulo ?? "").Contains(buscar) ||
                                (x.Texto ?? "").Contains(buscar)
                            )
                        )
                        .OrderBy(x => x.Orden);
                }
                else
                {
                    query = _context.Promociones
                        .Include(x => x.Empresa)
                        .Where(x =>
                            (x.Titulo ?? "").Contains(buscar) ||
                            (x.Texto ?? "").Contains(buscar)
                        )
                        .OrderBy(x => x.Orden);
                }

                var total = await query.CountAsync();

                var promociones = await query
                    .Skip((pagina - 1) * cantidad)
                    .Take(cantidad)
                    .Select(x => new PromocionDTO
                    {
                        Id = x.Id,
                        Titulo = x.Titulo,
                        Subtitulo = x.Subtitulo,
                        Texto = x.Texto,
                        TextoBoton = x.TextoBoton,
                        Link = x.Link,
                        Fecha = x.Fecha,
                        FechaDesde = x.FechaDesde,
                        FechaHasta = x.FechaHasta,
                        Publica = x.Publica,
                        Vencimiento = x.Vencimiento,
                        PromocionFija = x.PromocionFija,
                        QR = x.QR,
                        Orden = x.Orden,
                        Foto = x.Foto,
                        EmpresaId = x.Empresa != null ? (int?)x.Empresa.Id : null,
                        Estado = x.Vencimiento && x.FechaHasta.Date < DateTime.Now.Date
                            ? "Vencida"
                            : "Habilitada"
                    })
                    .ToListAsync();

                var response = new PromocionListadoResponseDTO
                {
                    Total = total,
                    Pagina = pagina,
                    CantidadPorPagina = cantidad,
                    TotalPaginas = (int)Math.Ceiling(total / (double)cantidad),
                    Promociones = promociones
                };

                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    mensaje = "Hubo un error al obtener las promociones.",
                    error = e.Message
                });
            }
        }

        // GET: endpoint/promociones/5
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPromocion(int id)
        {
            try
            {
                var promocion = await _context.Promociones
                    .Include(x => x.Empresa)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (promocion == null)
                {
                    return NotFound(new { mensaje = "No se encontró la promoción." });
                }

                var dto = new PromocionDTO
                {
                    Id = promocion.Id,
                    Titulo = promocion.Titulo,
                    Subtitulo = promocion.Subtitulo,
                    Texto = promocion.Texto,
                    TextoBoton = promocion.TextoBoton,
                    Link = promocion.Link,
                    Fecha = promocion.Fecha,
                    FechaDesde = promocion.FechaDesde,
                    FechaHasta = promocion.FechaHasta,
                    Publica = promocion.Publica,
                    Vencimiento = promocion.Vencimiento,
                    PromocionFija = promocion.PromocionFija,
                    QR = promocion.QR,
                    Orden = promocion.Orden,
                    Foto = promocion.Foto,
                    EmpresaId = promocion.Empresa != null ? (int?)promocion.Empresa.Id : null,
                    Estado = promocion.Vencimiento && promocion.FechaHasta.Date < DateTime.Now.Date
                        ? "Vencida"
                        : "Habilitada"
                };

                return Ok(dto);
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    mensaje = "Hubo un error al obtener la promoción.",
                    error = e.Message
                });
            }
        }

        // POST: endpoint/promociones
        [HttpPost]
        public async Task<IActionResult> CrearPromocion([FromBody] PromocionCreateDTO dto)
        {
            try
            {
                var usuario = await _context.Usuarios
                    .Include(x => x.Clientes)
                        .ThenInclude(x => x.Empresa)
                    .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

                var promocion = new Promociones
                {
                    Titulo = dto.Titulo,
                    Subtitulo = dto.Subtitulo,
                    Texto = dto.Texto,
                    TextoBoton = dto.TextoBoton,
                    Link = dto.Link,
                    Fecha = dto.Fecha,

                    FechaDesde = dto.FechaDesde,
                    FechaHasta = dto.FechaHasta,

                    Publica = dto.Publica,
                    Orden = dto.Orden,

                    Empresa = usuario != null && usuario.Clientes != null
                        ? usuario.Clientes.Empresa
                        : null,

                    QR = false,

                    Vencimiento = dto.TieneFechaVencimiento,

                    PromocionFija = dto.PromocionFija == 1
                };

                _context.Promociones.Add(promocion);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    mensaje = "Se registró correctamente la promoción.",
                    promocionId = promocion.Id
                });
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    mensaje = "Hubo un error al registrar la promoción.",
                    error = e.Message
                });
            }
        }

        // PUT: endpoint/promociones/5
        [HttpPut("{id}")]
        public async Task<IActionResult> EditarPromocion(int id, [FromBody] PromocionUpdateDTO dto)
        {
            try
            {
                var promocion = await _context.Promociones
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (promocion == null)
                {
                    return NotFound(new { mensaje = "No se encontró la promoción." });
                }

                promocion.Titulo = dto.Titulo;
                promocion.Subtitulo = dto.Subtitulo == null ? " " : dto.Subtitulo;
                promocion.Texto = dto.Texto == null ? " " : dto.Texto;
                promocion.TextoBoton = dto.TextoBoton == null ? " " : dto.TextoBoton;
                promocion.Fecha = dto.Fecha;
                promocion.Publica = dto.Publica;
                promocion.Link = dto.Link;

                if (dto.TieneFechaVencimiento)
                {
                    promocion.FechaHasta = dto.FechaHasta;
                    promocion.FechaDesde = dto.FechaDesde;
                    promocion.Vencimiento = true;
                }
                else
                {
                    promocion.Vencimiento = false;
                }

                if (dto.PromocionFija == 1)
                {
                    promocion.PromocionFija = true;
                }
                else
                {
                    promocion.PromocionFija = false;
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    mensaje = "Se modificó correctamente la promoción.",
                    promocionId = promocion.Id
                });
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    mensaje = "Hubo un error al modificar la promoción.",
                    error = e.Message
                });
            }
        }

        // POST: endpoint/promociones/5/imagen
        [HttpPost("{id}/imagen")]
        public async Task<IActionResult> CambiarImagen(int id, [FromForm] IFormFile file)
        {
            try
            {
                var promocion = await _context.Promociones.FindAsync(id);

                if (promocion == null)
                {
                    return NotFound(new { mensaje = "No se encontró la promoción." });
                }

                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { mensaje = "Debe enviar una imagen." });
                }

                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    promocion.Foto = Convert.ToBase64String(memoryStream.ToArray());
                    await _context.SaveChangesAsync();
                }

                return Ok(new
                {
                    mensaje = "Se modificó correctamente la imagen de la promoción.",
                    promocionId = promocion.Id
                });
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    mensaje = "Hubo un error al modificar la imagen de la promoción.",
                    error = e.Message
                });
            }
        }

        // PUT: endpoint/promociones/5/habilitar-qr
        [HttpPut("{id}/habilitar-qr")]
        public async Task<IActionResult> HabilitarQR(int id)
        {
            try
            {
                var promocion = await _context.Promociones.FindAsync(id);

                if (promocion == null)
                {
                    return NotFound(new { mensaje = "No se encontró la promoción." });
                }

                promocion.QR = true;

                _context.Promociones.Update(promocion);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    mensaje = "Se habilitó el QR correctamente.",
                    promocionId = promocion.Id
                });
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    mensaje = "No se pudo habilitar el QR.",
                    error = e.Message
                });
            }
        }

        // PUT: endpoint/promociones/5/deshabilitar-qr
        [HttpPut("{id}/deshabilitar-qr")]
        public async Task<IActionResult> DeshabilitarQR(int id)
        {
            try
            {
                var promocion = await _context.Promociones.FindAsync(id);

                if (promocion == null)
                {
                    return NotFound(new { mensaje = "No se encontró la promoción." });
                }

                promocion.QR = false;

                _context.Promociones.Update(promocion);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    mensaje = "Se deshabilitó el QR correctamente.",
                    promocionId = promocion.Id
                });
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    mensaje = "No se pudo deshabilitar el QR.",
                    error = e.Message
                });
            }
        }

        // GET: endpoint/promociones/5/qr
        [HttpGet("{id}/qr")]
        public async Task<IActionResult> ObtenerQR(int id)
        {
            try
            {
                var promocion = await _context.Promociones
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (promocion == null)
                {
                    return NotFound(new { mensaje = "No se encontró la promoción." });
                }

                var qrs = await _context.PromocionesQR
                    .Where(x => x.Promociones.Id == id)
                    .Select(x => new PromocionQRDTO
                    {
                        Id = x.Id,
                        PromocionId = id,
                        Hash = x.Hash,
                        Activo = x.Activo,
                        FechaUtilizado = x.FechaUtilizado
                    })
                    .ToListAsync();

                return Ok(new
                {
                    promocionId = promocion.Id,
                    titulo = promocion.Titulo,
                    qrHabilitado = promocion.QR,
                    qrs = qrs
                });
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    mensaje = "Hubo un error al obtener la información del QR.",
                    error = e.Message
                });
            }
        }

        // GET: endpoint/promociones/validar?hash=xxxxx
        [AllowAnonymous]
        [HttpGet("validar")]
        public async Task<IActionResult> ValidarPromocion(string hash)
        {
            try
            {
                var promocionQR = await _context.PromocionesQR
                    .FirstOrDefaultAsync(x => x.Hash == hash);

                if (promocionQR != null)
                {
                    if (promocionQR.Activo == true)
                    {
                        promocionQR.Activo = false;
                        promocionQR.FechaUtilizado = DateTime.Now;

                        await _context.SaveChangesAsync();

                        return Ok(new ValidarPromocionResponseDTO
                        {
                            EsActivo = true,
                            Texto = "La promoción se utilizo con éxito."
                        });
                    }
                    else
                    {
                        return Ok(new ValidarPromocionResponseDTO
                        {
                            EsActivo = true,
                            Texto = "Esta promocion ya no es válida."
                        });
                    }
                }
                else
                {
                    return Ok(new ValidarPromocionResponseDTO
                    {
                        EsActivo = true,
                        Texto = "La promoción no existe"
                    });
                }
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    EsActivo = true,
                    Texto = "Error al leer la promoción",
                    error = e.Message
                });
            }
        }

        // POST: endpoint/promociones/notificacion/5
        [HttpPost("notificacion/{id}")]
        public async Task<IActionResult> EnviarNotificacion(int id)
        {
            try
            {
                var usuario = await _context.Usuarios
                    .Include(x => x.Clientes)
                        .ThenInclude(x => x.Empresa)
                    .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

                if (usuario != null && usuario.Clientes != null && usuario.Clientes.Empresa != null)
                {
                    var listaPush = _context.Clientes
                        .Where(x => x.Empresa.Id == usuario.Clientes.Empresa.Id);

                    var promocion = await _context.Promociones.FindAsync(id);

                    if (promocion == null)
                    {
                        return NotFound(new { mensaje = "No se encontró la promoción." });
                    }

                    return Ok(new
                    {
                        mensaje = listaPush.Count().ToString() + " Notificaciones Enviadas!",
                        cantidad = listaPush.Count()
                    });
                }

                return BadRequest(new
                {
                    mensaje = "No se pudieron enviar las notificaciones."
                });
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    mensaje = "No se pudieron enviar las notificaciones.",
                    error = e.Message
                });
            }
        }

        // PUT: endpoint/promociones/orden
        [HttpPut("orden")]
        public async Task<IActionResult> ModificarOrden([FromBody] PromocionCambiarOrdenDTO dto)
        {
            try
            {
                var promocion = await _context.Promociones
                    .FirstOrDefaultAsync(x => x.Id == dto.Id);

                if (promocion == null)
                {
                    return NotFound(new { mensaje = "No se encontró la promoción." });
                }

                promocion.Orden = dto.Orden;

                _context.Promociones.Update(promocion);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    mensaje = "Se modificó el orden de la promoción.",
                    promocionId = promocion.Id,
                    orden = promocion.Orden
                });
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    mensaje = "No se pudo modificar la promoción.",
                    error = e.Message
                });
            }
        }

        // DELETE: endpoint/promociones/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> BorrarPromocion(int id)
        {
            try
            {
                List<PromocionesQR> promocionesQR = await _context.PromocionesQR
                    .Where(x => x.Promociones.Id == id)
                    .ToListAsync();

                if (promocionesQR != null)
                {
                    foreach (var itemQR in promocionesQR)
                    {
                        _context.PromocionesQR.Remove(itemQR);
                    }

                    await _context.SaveChangesAsync();
                }

                var promocion = await _context.Promociones
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync();

                if (promocion == null)
                {
                    return NotFound(new { mensaje = "No se encontró la promoción." });
                }

                _context.Promociones.Remove(promocion);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    mensaje = "Se eliminó correctamente la promoción.",
                    promocionId = id
                });
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    mensaje = "No se pudo eliminar la promoción.",
                    error = e.Message
                });
            }
        }
    }
}