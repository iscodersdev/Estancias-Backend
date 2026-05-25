using DAL.Data;
using DAL.DTOs;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.Endpoints
{
    [Route("endpoint/categorias")]
    [ApiController]
    public class CategoriasEndpointController : ControllerBase
    {
        private readonly EstanciasContext _context;

        public CategoriasEndpointController(EstanciasContext context)
        {
            _context = context;
        }

        // GET: endpoint/categorias
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var categorias = await _context.Categorias
                    .Select(c => new CategoriaDTO
                    {
                        Id = c.Id,
                        Nombre = c.Nombre,
                        Activo = c.Activo
                    })
                    .ToListAsync();

                return Ok(new
                {
                    Status = 200,
                    Mensaje = "Categorías obtenidas correctamente.",
                    Categorias = categorias
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    Status = 500,
                    Mensaje = "Hubo un error al obtener las Categorías. Intentelo nuevamente mas tarde."
                });
            }
        }

        // GET: endpoint/categorias/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var categoria = await _context.Categorias
                    .Where(c => c.Id == id)
                    .Select(c => new CategoriaDTO
                    {
                        Id = c.Id,
                        Nombre = c.Nombre,
                        Activo = c.Activo
                    })
                    .FirstOrDefaultAsync();

                if (categoria == null)
                {
                    return NotFound(new
                    {
                        Status = 404,
                        Mensaje = "No se encontró la Categoría solicitada."
                    });
                }

                return Ok(new
                {
                    Status = 200,
                    Mensaje = "Categoría obtenida correctamente.",
                    Categoria = categoria
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    Status = 500,
                    Mensaje = "Hubo un error al obtener la Categoría. Intentelo nuevamente mas tarde."
                });
            }
        }

        // POST: endpoint/categorias
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoriaDTO dto)
        {
            ModelState.Remove("Id");

            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Status = 400,
                    Mensaje = "Los datos enviados no son válidos.",
                    Errores = ModelState
                });
            }

            try
            {
                var categoria = new Categorias
                {
                    Nombre = dto.Nombre,
                    Activo = dto.Activo
                };

                await _context.Categorias.AddAsync(categoria);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Status = 200,
                    Mensaje = "Se creó correctamente la Categoría " + categoria.Nombre + ".",
                    Categoria = new CategoriaDTO
                    {
                        Id = categoria.Id,
                        Nombre = categoria.Nombre,
                        Activo = categoria.Activo
                    }
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    Status = 500,
                    Mensaje = "Hubo un error al crear la Categoría. Intentelo nuevamente mas tarde."
                });
            }
        }

        // PUT: endpoint/categorias/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoriaDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Status = 400,
                    Mensaje = "Los datos enviados no son válidos.",
                    Errores = ModelState
                });
            }

            try
            {
                var categoria = await _context.Categorias.FindAsync(id);

                if (categoria == null)
                {
                    return NotFound(new
                    {
                        Status = 404,
                        Mensaje = "No se encontró la Categoría solicitada."
                    });
                }

                categoria.Nombre = dto.Nombre;
                categoria.Activo = dto.Activo;

                _context.Categorias.Update(categoria);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Status = 200,
                    Mensaje = "Se editó correctamente la Categoría " + categoria.Nombre + ".",
                    Categoria = new CategoriaDTO
                    {
                        Id = categoria.Id,
                        Nombre = categoria.Nombre,
                        Activo = categoria.Activo
                    }
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    Status = 500,
                    Mensaje = "Hubo un error al editar la Categoría. Intentelo nuevamente mas tarde."
                });
            }
        }

        // DELETE: endpoint/categorias/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var categoria = await _context.Categorias
                    .Where(c => c.Id == id)
                    .FirstOrDefaultAsync();

                if (categoria == null)
                {
                    return NotFound(new
                    {
                        Status = 404,
                        Mensaje = "No se encontró la Categoría solicitada."
                    });
                }

                _context.Categorias.Remove(categoria);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Status = 200,
                    Mensaje = "Se eliminó correctamente la Categoría."
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    Status = 500,
                    Mensaje = "Hubo un error al eliminar la Categoría."
                });
            }
        }
    }
}