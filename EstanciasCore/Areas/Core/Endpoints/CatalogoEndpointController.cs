using DAL.Data;
using DAL.DTOs;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.Areas.Core.Endpoints
{
    [ApiController]
    [Route("endpoint/catalogo")]
    public class CatalogoEndpointController : ControllerBase
    {
        private readonly EstanciasContext _context;

        public CatalogoEndpointController(EstanciasContext context)
        {
            _context = context;
        }

        // GET: endpoint/catalogo/listar?pagina=1&cantidad=10&buscar=verano
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

                var query = _context.Catalogo.AsQueryable();

                if (!string.IsNullOrWhiteSpace(buscar))
                {
                    var texto = buscar.Trim().ToLower();

                    query = query.Where(c =>
                        ((c.Nombre ?? "").ToLower().Contains(texto)) ||
                        ((c.Descripcion ?? "").ToLower().Contains(texto)) ||
                        ((c.Link ?? "").ToLower().Contains(texto))
                    );
                }

                var totalRegistros = await query.CountAsync();

                var catalogos = await query
                    .OrderBy(c => c.Nombre)
                    .Skip((pagina - 1) * cantidad)
                    .Take(cantidad)
                    .Select(c => new CatalogoDTO
                    {
                        Id = c.Id,
                        Nombre = c.Nombre ?? "",
                        Descripcion = c.Descripcion ?? "",
                        Link = c.Link ?? "",
                        Activo = c.Activo
                    })
                    .ToListAsync();

                var totalPaginas = totalRegistros == 0
                    ? 1
                    : (int)Math.Ceiling(totalRegistros / (decimal)cantidad);

                var response = new CatalogoListadoResponseDTO
                {
                    Status = 200,
                    Mensaje = "Listado de catálogos obtenido correctamente.",
                    Data = new CatalogoListadoDTO
                    {
                        TotalRegistros = totalRegistros,
                        PaginaActual = pagina,
                        CantidadPorPagina = cantidad,
                        TotalPaginas = totalPaginas,
                        Catalogos = catalogos
                    }
                };

                return Ok(response);
            }
            catch (Exception)
            {
                return StatusCode(500, new CatalogoResponseDTO
                {
                    Status = 500,
                    Mensaje = "Hubo un error al obtener el listado de catálogos. Intentelo nuevamente mas tarde."
                });
            }
        }

        // GET: endpoint/catalogo/obtener/5
        [HttpGet("obtener/{id}")]
        public async Task<IActionResult> Obtener(int id)
        {
            try
            {
                var catalogo = await _context.Catalogo
                    .Where(c => c.Id == id)
                    .Select(c => new CatalogoDTO
                    {
                        Id = c.Id,
                        Nombre = c.Nombre ?? "",
                        Descripcion = c.Descripcion ?? "",
                        Link = c.Link ?? "",
                        Activo = c.Activo
                    })
                    .FirstOrDefaultAsync();

                if (catalogo == null)
                {
                    return NotFound(new CatalogoResponseDTO
                    {
                        Status = 404,
                        Mensaje = "No se encontró el Catálogo solicitado."
                    });
                }

                return Ok(new CatalogoItemResponseDTO
                {
                    Status = 200,
                    Mensaje = "Catálogo obtenido correctamente.",
                    Catalogo = catalogo
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new CatalogoResponseDTO
                {
                    Status = 500,
                    Mensaje = "Hubo un error al obtener el Catálogo. Intentelo nuevamente mas tarde."
                });
            }
        }

        // POST: endpoint/catalogo/crear
        [HttpPost("crear")]
        public async Task<IActionResult> Crear([FromBody] CatalogoCreateDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new CatalogoResponseDTO
                    {
                        Status = 400,
                        Mensaje = "Los datos del Catálogo son obligatorios."
                    });
                }

                if (string.IsNullOrWhiteSpace(dto.Nombre))
                {
                    return BadRequest(new CatalogoResponseDTO
                    {
                        Status = 400,
                        Mensaje = "El nombre del Catálogo es obligatorio."
                    });
                }

                var catalogo = new Catalogo
                {
                    Nombre = dto.Nombre.Trim(),
                    Descripcion = dto.Descripcion ?? "",
                    Link = dto.Link ?? ""
                };

                await _context.Catalogo.AddAsync(catalogo);
                await _context.SaveChangesAsync();

                return Ok(new CatalogoItemResponseDTO
                {
                    Status = 200,
                    Mensaje = "Se creó correctamente el Catálogo " + catalogo.Nombre + ".",
                    Catalogo = new CatalogoDTO
                    {
                        Id = catalogo.Id,
                        Nombre = catalogo.Nombre ?? "",
                        Descripcion = catalogo.Descripcion ?? "",
                        Link = catalogo.Link ?? "",
                        Activo = catalogo.Activo
                    }
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new CatalogoResponseDTO
                {
                    Status = 500,
                    Mensaje = "Hubo un error al crear el Catálogo. Intentelo nuevamente mas tarde."
                });
            }
        }

        // PUT: endpoint/catalogo/editar/5
        [HttpPut("editar/{id}")]
        public async Task<IActionResult> Editar(int id, [FromBody] CatalogoUpdateDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new CatalogoResponseDTO
                    {
                        Status = 400,
                        Mensaje = "Los datos del Catálogo son obligatorios."
                    });
                }

                if (id != dto.Id)
                {
                    return BadRequest(new CatalogoResponseDTO
                    {
                        Status = 400,
                        Mensaje = "El Id enviado por ruta no coincide con el Id del Catálogo."
                    });
                }

                if (string.IsNullOrWhiteSpace(dto.Nombre))
                {
                    return BadRequest(new CatalogoResponseDTO
                    {
                        Status = 400,
                        Mensaje = "El nombre del Catálogo es obligatorio."
                    });
                }

                var catalogoUpdate = await _context.Catalogo.FindAsync(id);

                if (catalogoUpdate == null)
                {
                    return NotFound(new CatalogoResponseDTO
                    {
                        Status = 404,
                        Mensaje = "No se encontró el Catálogo solicitado."
                    });
                }

                catalogoUpdate.Nombre = dto.Nombre.Trim();
                catalogoUpdate.Descripcion = dto.Descripcion ?? "";
                catalogoUpdate.Link = dto.Link ?? "";

                _context.Catalogo.Update(catalogoUpdate);
                await _context.SaveChangesAsync();

                return Ok(new CatalogoItemResponseDTO
                {
                    Status = 200,
                    Mensaje = "Se editó correctamente el Catálogo " + catalogoUpdate.Nombre + ".",
                    Catalogo = new CatalogoDTO
                    {
                        Id = catalogoUpdate.Id,
                        Nombre = catalogoUpdate.Nombre ?? "",
                        Descripcion = catalogoUpdate.Descripcion ?? "",
                        Link = catalogoUpdate.Link ?? "",
                        Activo = catalogoUpdate.Activo
                    }
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new CatalogoResponseDTO
                {
                    Status = 500,
                    Mensaje = "Hubo un error al editar el Catálogo. Intentelo nuevamente mas tarde."
                });
            }
        }

        // PUT: endpoint/catalogo/activar/5
        [HttpPut("activar/{id}")]
        public async Task<IActionResult> ActivarCatalogo(int id)
        {
            try
            {
                var existeCatalogo = await _context.Catalogo.AnyAsync(c => c.Id == id);

                if (!existeCatalogo)
                {
                    return NotFound(new CatalogoResponseDTO
                    {
                        Status = 404,
                        Mensaje = "No se encontró el Catálogo solicitado."
                    });
                }

                var listaCatalogo = await _context.Catalogo.ToListAsync();

                foreach (var item in listaCatalogo)
                {
                    if (item.Id == id)
                    {
                        item.Activo = true;
                    }
                    else
                    {
                        item.Activo = false;
                    }
                }

                _context.Catalogo.UpdateRange(listaCatalogo);
                await _context.SaveChangesAsync();

                return Ok(new CatalogoResponseDTO
                {
                    Status = 200,
                    Mensaje = "Se activó correctamente el Catálogo."
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new CatalogoResponseDTO
                {
                    Status = 500,
                    Mensaje = "Hubo un error al activar el Catálogo. Intentelo nuevamente mas tarde."
                });
            }
        }

        // DELETE: endpoint/catalogo/borrar/5
        [HttpDelete("borrar/{id}")]
        public async Task<IActionResult> Borrar(int id)
        {
            try
            {
                var catalogo = await _context.Catalogo
                    .Where(c => c.Id == id)
                    .FirstOrDefaultAsync();

                if (catalogo == null)
                {
                    return NotFound(new CatalogoResponseDTO
                    {
                        Status = 404,
                        Mensaje = "No se encontró el Catálogo solicitado."
                    });
                }

                _context.Catalogo.Remove(catalogo);
                await _context.SaveChangesAsync();

                return Ok(new CatalogoResponseDTO
                {
                    Status = 200,
                    Mensaje = "Se eliminó correctamente el Catálogo."
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new CatalogoResponseDTO
                {
                    Status = 500,
                    Mensaje = "Hubo un error al eliminar el Catálogo."
                });
            }
        }
    }
}