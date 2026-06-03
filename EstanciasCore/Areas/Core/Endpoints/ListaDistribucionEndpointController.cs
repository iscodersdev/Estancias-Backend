using DAL.Data;
using DAL.DTOs;
using DAL.Models.Core;
using EstanciasCore.API.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.Endpoints
{
    [TypeFilter(typeof(EndpointUatAuthorizeAttribute))]
    [Route("endpoint/lista-distribucion")]
    [ApiController]
    public class ListaDistribucionEndpointController : ControllerBase
    {
        private readonly EstanciasContext _context;

        public ListaDistribucionEndpointController(EstanciasContext context)
        {
            _context = context;
        }

        // GET: endpoint/lista-distribucion
        // GET: endpoint/lista-distribucion?buscar=algo
        [HttpGet]
        public async Task<IActionResult> GetAll(string buscar = null)
        {
            var query = _context.ListaDistribucion.AsQueryable();

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                var texto = buscar.Trim().ToLower();

                query = query.Where(l =>
                    (l.Nombre ?? "").ToLower().Contains(texto) ||
                    (l.Descripcion ?? "").ToLower().Contains(texto)
                );
            }

            var listas = await query
                .OrderBy(l => l.Nombre)
                .Select(l => new ListaDistribucionDTO
                {
                    Id = l.Id,
                    Nombre = l.Nombre,
                    Descripcion = l.Descripcion,
                    Activo = l.Activo,
                    CantidadDestinatarios = _context.DistribucionDestinatarios
                        .Count(d => d.ListaDistribucion.Id == l.Id)
                })
                .ToListAsync();

            return Ok(new
            {
                ok = true,
                data = listas
            });
        }

        // GET: endpoint/lista-distribucion/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var lista = await _context.ListaDistribucion
                .Where(l => l.Id == id)
                .Select(l => new ListaDistribucionDTO
                {
                    Id = l.Id,
                    Nombre = l.Nombre,
                    Descripcion = l.Descripcion,
                    Activo = l.Activo,
                    CantidadDestinatarios = _context.DistribucionDestinatarios
                        .Count(d => d.ListaDistribucion.Id == l.Id)
                })
                .FirstOrDefaultAsync();

            if (lista == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró la Lista de Distribución."
                });
            }

            return Ok(new
            {
                ok = true,
                data = lista
            });
        }

        // POST: endpoint/lista-distribucion
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ListaDistribucionCreateUpdateDTO dto)
        {
            if (dto == null)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Los datos enviados son inválidos."
                });
            }

            try
            {
                var lista = new ListaDistribucion
                {
                    Nombre = dto.Nombre,
                    Descripcion = dto.Descripcion,
                    Activo = dto.Activo
                };

                await _context.ListaDistribucion.AddAsync(lista);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    ok = true,
                    message = "Se creó correctamente la Lista de Distribución " + lista.Nombre + ".",
                    data = new ListaDistribucionDTO
                    {
                        Id = lista.Id,
                        Nombre = lista.Nombre,
                        Descripcion = lista.Descripcion,
                        Activo = lista.Activo,
                        CantidadDestinatarios = 0
                    }
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    ok = false,
                    message = "Hubo un error al crear la Lista de Distribución. Intentelo nuevamente más tarde."
                });
            }
        }


        // PUT: endpoint/lista-distribucion/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ListaDistribucionCreateUpdateDTO dto)
        {
            if (dto == null)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Los datos enviados son inválidos."
                });
            }

            var lista = await _context.ListaDistribucion.FindAsync(id);

            if (lista == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró la Lista de Distribución."
                });
            }

            try
            {
                lista.Nombre = dto.Nombre;
                lista.Descripcion = dto.Descripcion;
                lista.Activo = dto.Activo;

                _context.ListaDistribucion.Update(lista);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    ok = true,
                    message = "Se editó correctamente la Lista de Distribución " + lista.Nombre + ".",
                    data = new ListaDistribucionDTO
                    {
                        Id = lista.Id,
                        Nombre = lista.Nombre,
                        Descripcion = lista.Descripcion,
                        Activo = lista.Activo,
                        CantidadDestinatarios = _context.DistribucionDestinatarios
                            .Count(d => d.ListaDistribucion.Id == lista.Id)
                    }
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    ok = false,
                    message = "Hubo un error al editar la Lista de Distribución. Intentelo nuevamente más tarde."
                });
            }
        }

        // DELETE: endpoint/lista-distribucion/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var lista = await _context.ListaDistribucion
                    .Where(l => l.Id == id)
                    .FirstOrDefaultAsync();

                if (lista == null)
                {
                    return NotFound(new
                    {
                        ok = false,
                        message = "No se encontró la Lista de Distribución."
                    });
                }

                var destinatarios = await _context.DistribucionDestinatarios
                    .Where(d => d.ListaDistribucion.Id == lista.Id)
                    .ToListAsync();

                _context.DistribucionDestinatarios.RemoveRange(destinatarios);
                _context.ListaDistribucion.Remove(lista);

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    ok = true,
                    message = "Se eliminó correctamente la Lista."
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    ok = false,
                    message = "Hubo un error al eliminar la Lista."
                });
            }
        }

        /* ------------------------------------------------------------
         * Destinatarios
         * ------------------------------------------------------------ */

        // GET: endpoint/lista-distribucion/5/destinatarios
        [HttpGet("{id:int}/destinatarios")]
        public async Task<IActionResult> GetDestinatarios(int id)
        {
            var lista = await _context.ListaDistribucion
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lista == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró la Lista de Distribución."
                });
            }

            var destinatarios = await _context.DistribucionDestinatarios
                .Include(d => d.ListaDistribucion)
                .Include(d => d.Destinatario)
                    .ThenInclude(u => u.Personas)
                .Where(d => d.ListaDistribucion.Id == id)
                .Select(d => new DistribucionDestinatarioDTO
                {
                    Id = d.Id,
                    ListaDistribucionId = d.ListaDistribucion.Id,
                    ListaDistribucionNombre = d.ListaDistribucion.Nombre,

                    DestinatarioId = d.Destinatario.Id,
                    UserName = d.Destinatario.UserName,

                    Apellido = d.Destinatario.Personas.Apellido,
                    Nombres = d.Destinatario.Personas.Nombres,
                    NroDocumento = d.Destinatario.Personas.NroDocumento,

                    NombreCompleto = d.Destinatario.Personas.Apellido + ", " + d.Destinatario.Personas.Nombres
                })
                .ToListAsync();

            return Ok(new
            {
                ok = true,
                lista = new
                {
                    lista.Id,
                    lista.Nombre,
                    lista.Descripcion,
                    lista.Activo
                },
                data = destinatarios
            });
        }

        // POST: endpoint/lista-distribucion/5/destinatarios
        [HttpPost("{id:int}/destinatarios")]
        public async Task<IActionResult> CreateDestinatario(int id, [FromBody] CrearDestinatarioDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.DestinatarioId))
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Hubo un error al agregar el Destinatario. Intentelo nuevamente más tarde."
                });
            }

            try
            {
                var lista = await _context.ListaDistribucion
                    .FirstOrDefaultAsync(l => l.Id == id);

                if (lista == null)
                {
                    return NotFound(new
                    {
                        ok = false,
                        message = "No se encontró la Lista de Distribución."
                    });
                }

                var usuario = await _context.Usuarios
                    .Include(u => u.Personas)
                    .FirstOrDefaultAsync(u => u.Id == dto.DestinatarioId);

                if (usuario == null)
                {
                    return NotFound(new
                    {
                        ok = false,
                        message = "No se encontró el Destinatario."
                    });
                }

                var destinatario = new DistribucionDestinatarios
                {
                    Id = 0,
                    ListaDistribucion = lista,
                    Destinatario = usuario
                };

                await _context.DistribucionDestinatarios.AddAsync(destinatario);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    ok = true,
                    message = "Se agregó correctamente el Destinatario " + usuario.UserName + ".",
                    data = new DistribucionDestinatarioDTO
                    {
                        Id = destinatario.Id,
                        ListaDistribucionId = lista.Id,
                        ListaDistribucionNombre = lista.Nombre,

                        DestinatarioId = usuario.Id,
                        UserName = usuario.UserName,

                        Apellido = usuario.Personas?.Apellido,
                        Nombres = usuario.Personas?.Nombres,
                        NroDocumento = usuario.Personas?.NroDocumento,

                        NombreCompleto = usuario.Personas == null
                            ? usuario.UserName
                            : usuario.Personas.Apellido + ", " + usuario.Personas.Nombres
                    }
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    ok = false,
                    message = "Hubo un error al agregar el Destinatario. Intentelo nuevamente más tarde."
                });
            }
        }

        // DELETE: endpoint/lista-distribucion/destinatarios/10
        [HttpDelete("destinatarios/{destinatarioDistribucionId:int}")]
        public async Task<IActionResult> DeleteDestinatario(int destinatarioDistribucionId)
        {
            var destinatario = await _context.DistribucionDestinatarios
                .Include(d => d.ListaDistribucion)
                .FirstOrDefaultAsync(d => d.Id == destinatarioDistribucionId);

            if (destinatario == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el Destinatario."
                });
            }

            try
            {
                _context.DistribucionDestinatarios.Remove(destinatario);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    ok = true,
                    message = "Se eliminó correctamente el Destinatario.",
                    listaId = destinatario.ListaDistribucion.Id
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    ok = false,
                    message = "Hubo un error al eliminar el Destinatario."
                });
            }
        }

        // DELETE: endpoint/lista-distribucion/5/destinatarios/vaciar
        [HttpDelete("{id:int}/destinatarios/vaciar")]
        public async Task<IActionResult> VaciarListaDestinatarios(int id)
        {
            try
            {
                var listaExiste = await _context.ListaDistribucion.AnyAsync(l => l.Id == id);

                if (!listaExiste)
                {
                    return NotFound(new
                    {
                        ok = false,
                        message = "No se encontró la Lista de Distribución."
                    });
                }

                var destinatarios = await _context.DistribucionDestinatarios
                    .Where(d => d.ListaDistribucion.Id == id)
                    .ToListAsync();

                if (destinatarios.Any())
                {
                    _context.DistribucionDestinatarios.RemoveRange(destinatarios);
                    await _context.SaveChangesAsync();

                    return Ok(new
                    {
                        ok = true,
                        message = "Se ha vaciado la lista de destinatarios correctamente."
                    });
                }

                return Ok(new
                {
                    ok = true,
                    message = "La lista ya estaba vacía."
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    ok = false,
                    message = "Hubo un error al vaciar la lista de destinatarios."
                });
            }
        }

        // GET: endpoint/lista-distribucion/destinatarios/combo?q=123
        [HttpGet("destinatarios/combo")]
        public async Task<IActionResult> DestinatariosComboJson(string q)
        {
            q = q ?? "";

            var items = await _context.Usuarios
                .Include(u => u.Personas)
                .Where(u => u.Personas.NroDocumento.Contains(q))
                .Select(u => new
                {
                    Text = u.Personas.Apellido + ", " + u.Personas.Nombres,
                    Value = u.Id,
                    Subtext = u.UserName,
                    Icon = "fa fa-user"
                })
                .Take(10)
                .ToListAsync();

            return Ok(new
            {
                ok = true,
                data = items
            });
        }

        // POST: endpoint/lista-distribucion/5/destinatarios/importar
        [HttpPost("{id:int}/destinatarios/importar")]
        public async Task<IActionResult> ImportarDestinatarios(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Por favor seleccione un archivo."
                });
            }

            try
            {
                var lista = await _context.ListaDistribucion.FindAsync(id);

                if (lista == null)
                {
                    return NotFound(new
                    {
                        ok = false,
                        message = "No se encontró la Lista de Distribución."
                    });
                }

                int addedCount = 0;
                int notFoundCount = 0;
                int existingCount = 0;

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);

                    using (var package = new ExcelPackage(stream))
                    {
                        if (package.Workbook.Worksheets.Count == 0)
                        {
                            return BadRequest(new
                            {
                                ok = false,
                                message = "El archivo Excel no contiene hojas."
                            });
                        }

                        ExcelWorksheet worksheet = package.Workbook.Worksheets.First();
                        var rowCount = worksheet.Dimension?.Rows ?? 0;

                        if (rowCount < 2)
                        {
                            return BadRequest(new
                            {
                                ok = false,
                                message = "El archivo no contiene datos."
                            });
                        }

                        for (int row = 2; row <= rowCount; row++)
                        {
                            var dni = worksheet.Cells[row, 1].Value?.ToString()?.Trim();

                            if (!string.IsNullOrEmpty(dni))
                            {
                                var usuario = await _context.Usuarios
                                    .Include(u => u.Personas)
                                    .FirstOrDefaultAsync(u => u.Personas.NroDocumento == dni);

                                if (usuario != null)
                                {
                                    bool exists = await _context.DistribucionDestinatarios
                                        .AnyAsync(dd =>
                                            dd.ListaDistribucion.Id == id &&
                                            dd.Destinatario.Id == usuario.Id);

                                    if (!exists)
                                    {
                                        var nuevoDestinatario = new DistribucionDestinatarios
                                        {
                                            ListaDistribucion = lista,
                                            Destinatario = usuario
                                        };

                                        _context.DistribucionDestinatarios.Add(nuevoDestinatario);
                                        addedCount++;
                                    }
                                    else
                                    {
                                        existingCount++;
                                    }
                                }
                                else
                                {
                                    notFoundCount++;
                                }
                            }
                        }

                        if (addedCount > 0)
                        {
                            await _context.SaveChangesAsync();
                        }
                    }
                }

                string msg = $"Proceso finalizado. Agregados: {addedCount}.";

                if (existingCount > 0)
                {
                    msg += $" Ya existían: {existingCount}.";
                }

                if (notFoundCount > 0)
                {
                    msg += $" No encontrados (DNI inexistente): {notFoundCount}.";
                }

                return Ok(new
                {
                    ok = true,
                    message = msg,
                    agregados = addedCount,
                    existentes = existingCount,
                    noEncontrados = notFoundCount
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    ok = false,
                    message = "Error al importar el archivo: " + ex.Message
                });
            }
        }

        // GET: endpoint/lista-distribucion/destinatarios/plantilla
        [HttpGet("destinatarios/plantilla")]
        public IActionResult DescargarPlantilla()
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Plantilla");

                worksheet.Cells[1, 1].Value = "DNI";
                worksheet.Cells[1, 1].Style.Font.Bold = true;

                var content = package.GetAsByteArray();

                return File(
                    content,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "PlantillaDestinatarios.xlsx"
                );
            }
        }
    }
}