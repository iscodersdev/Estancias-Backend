using DAL.Data;
using DAL.DTOs;
using DAL.Models;
using EstanciasCore.API.Filters;
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
    [Route("endpoint/marcas")]
    [ApiController]
    public class MarcasEndpointController : ControllerBase
    {
        private readonly EstanciasContext _context;

        public MarcasEndpointController(EstanciasContext context)
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
                if (pagina <= 0)
                    pagina = 1;

                if (cantidad <= 0)
                    cantidad = 10;

                var query = _context.Marcas
                    .Where(x => x.Activo)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(buscar))
                {
                    buscar = buscar.Trim();

                    query = query.Where(x =>
                        x.Nombre.Contains(buscar) ||
                        x.NomAliasbre.Contains(buscar) ||
                        x.CBU.Contains(buscar) ||
                        x.WhatsApp.Contains(buscar)
                    );
                }

                var totalRegistros = await query.CountAsync();

                var marcas = await query
                    .OrderBy(x => x.Orden)
                    .Skip((pagina - 1) * cantidad)
                    .Take(cantidad)
                    .Select(x => new MarcaListDTO
                    {
                        Id = x.Id,
                        Nombre = x.Nombre,
                        Alias = x.NomAliasbre,
                        CBU = x.CBU,
                        WhatsApp = x.WhatsApp,
                        Orden = x.Orden,
                        Activo = x.Activo,
                        Imagen = x.Imagen != null ? "data:image/jpeg;base64" + Convert.ToBase64String(x.Imagen) : null

                    })
                    .ToListAsync();

                var totalPaginas = (int)Math.Ceiling(totalRegistros / (double)cantidad);

                return Ok(new
                {
                    Items = marcas,
                    TotalRegistros = totalRegistros,
                    PaginaActual = pagina,
                    CantidadPorPagina = cantidad,
                    TotalPaginas = totalPaginas,
                    Buscar = buscar
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    Mensaje = "Hubo un error al listar las Marcas. Inténtelo nuevamente más tarde."
                });
            }
        }

        [HttpGet("obtener/{id}")]
        public async Task<IActionResult> Obtener(int id)
        {
            try
            {
                var marca = await _context.Marcas.FindAsync(id);

                if (marca == null)
                {
                    return NotFound(new
                    {
                        Mensaje = "No se encontró la Marca."
                    });
                }

                return Ok(new MarcaDTO
                {
                    Id = marca.Id,
                    Nombre = marca.Nombre,
                    Alias = marca.NomAliasbre,
                    CBU = marca.CBU,
                    WhatsApp = marca.WhatsApp,
                    Orden = marca.Orden,
                    Activo = marca.Activo,
                    Imagen = marca.Imagen != null ? "data:image/jpeg;base64" + Convert.ToBase64String(marca.Imagen) : null
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    Mensaje = "Hubo un error al obtener la Marca. Inténtelo nuevamente más tarde."
                });
            }
        }

        [HttpPost("crear")]
        public async Task<IActionResult> Crear([FromBody] MarcaCreateDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        Mensaje = "Los datos enviados no son válidos.",
                        Errores = ModelState
                    });
                }

                var nuevaMarca = new Marcas
                {
                    Nombre = dto.Nombre,
                    Orden = dto.Orden,
                    NomAliasbre = dto.Alias,
                    CBU = dto.CBU,
                    WhatsApp = dto.WhatsApp,
                    Activo = true
                };

                await _context.Marcas.AddAsync(nuevaMarca);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Mensaje = "Se cargó correctamente la Marca " + nuevaMarca.Nombre + ".",
                    Item = new MarcaDTO
                    {
                        Id = nuevaMarca.Id,
                        Nombre = nuevaMarca.Nombre,
                        Alias = nuevaMarca.NomAliasbre,
                        CBU = nuevaMarca.CBU,
                        WhatsApp = nuevaMarca.WhatsApp,
                        Orden = nuevaMarca.Orden,
                        Activo = nuevaMarca.Activo
                    }
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    Mensaje = "Hubo un error al cargar la Marca. Inténtelo nuevamente más tarde."
                });
            }
        }

        [HttpPut("editar/{id}")]
        public async Task<IActionResult> Editar(int id, [FromBody] MarcaUpdateDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        Mensaje = "Los datos enviados no son válidos.",
                        Errores = ModelState
                    });
                }

                var marcaExistente = await _context.Marcas.FindAsync(id);

                if (marcaExistente == null)
                {
                    return NotFound(new
                    {
                        Mensaje = "No se encontró la Marca a modificar."
                    });
                }

                marcaExistente.Nombre = dto.Nombre;
                marcaExistente.Orden = dto.Orden;
                marcaExistente.Activo = dto.Activo;
                marcaExistente.NomAliasbre = dto.Alias;
                marcaExistente.CBU = dto.CBU;
                marcaExistente.WhatsApp = dto.WhatsApp;

                _context.Marcas.Update(marcaExistente);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Mensaje = "Se modificó correctamente la Marca.",
                    Item = new MarcaDTO
                    {
                        Id = marcaExistente.Id,
                        Nombre = marcaExistente.Nombre,
                        Alias = marcaExistente.NomAliasbre,
                        CBU = marcaExistente.CBU,
                        WhatsApp = marcaExistente.WhatsApp,
                        Orden = marcaExistente.Orden,
                        Activo = marcaExistente.Activo
                    }
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    Mensaje = "Hubo un error al modificar la Marca. Inténtelo nuevamente más tarde."
                });
            }
        }

        [HttpDelete("eliminar/{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var marca = await _context.Marcas.FindAsync(id);

                if (marca == null)
                {
                    return NotFound(new
                    {
                        Mensaje = "Hubo un error al eliminar la Marca. Inténtelo nuevamente más tarde."
                    });
                }

                marca.Activo = false;

                _context.Marcas.Update(marca);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Mensaje = "Se eliminó correctamente la Marca " + marca.Nombre + "."
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    Mensaje = "Hubo un error al eliminar la Marca. Inténtelo nuevamente más tarde."
                });
            }
        }

        [HttpPost("cargar-imagen/{id}")]
        public async Task<IActionResult> CargarImagen(int id, IFormFile FotoMarca)
        {
            try
            {
                var marcaEdit = await _context.Marcas.FindAsync(id);

                if (marcaEdit == null)
                {
                    return NotFound(new
                    {
                        Mensaje = "No se encontró la Marca para cargar la imagen."
                    });
                }

                if (FotoMarca != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await FotoMarca.CopyToAsync(memoryStream);
                        marcaEdit.Imagen = memoryStream.ToArray();
                    }
                }

                _context.Marcas.Update(marcaEdit);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Mensaje = "Se cargó correctamente la Imagen de la Marca.",
                    Id = marcaEdit.Id,
                    TieneImagen = marcaEdit.Imagen != null
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    Mensaje = "Hubo un error al cargar la Imagen. Inténtelo nuevamente más tarde."
                });
            }
        }

        [HttpGet("imagen/{id}")]
        public async Task<IActionResult> Imagen(int id)
        {
            var marca = await _context.Marcas.FindAsync(id);

            if (marca != null && marca.Imagen != null)
            {
                return File(marca.Imagen, "image/jpeg");
            }

            return NotFound();
        }
    }
}