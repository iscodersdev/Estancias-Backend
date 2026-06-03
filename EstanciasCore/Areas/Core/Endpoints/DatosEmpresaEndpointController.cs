using DAL.Data;
using DAL.DTOs;
using EstanciasCore.API.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.Endpoints
{
    [TypeFilter(typeof(EndpointUatAuthorizeAttribute))]
    [ApiController]
    [Route("endpoint/datos-empresa")]
    public class DatosEmpresaEndpointController : ControllerBase
    {
        private readonly EstanciasContext _context;

        public DatosEmpresaEndpointController(EstanciasContext context)
        {
            _context = context;
        }

        // GET: endpoint/datos-empresa
        // Replica la lógica de _listadoDatosEmpresa, pero llevada a API.
        // Incluye búsqueda y paginación para reemplazar el comportamiento del listado MVC/DataTable.
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string buscar = "",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (page < 1)
                page = 1;

            if (pageSize < 1)
                pageSize = 10;

            var query = _context.DatosEstructura.AsQueryable();

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                var texto = buscar.Trim().ToLower();

                query = query.Where(x =>
                    ((x.Alias ?? "").ToLower().Contains(texto)) ||
                    ((x.CBU ?? "").ToLower().Contains(texto)) ||
                    ((x.Telefono ?? "").ToLower().Contains(texto))
                );
            }

            var totalRegistros = await query.CountAsync();

            var datos = await query
                .OrderBy(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new DatosEmpresaDTO
                {
                    Id = x.Id,
                    Alias = x.Alias,
                    CBU = x.CBU,
                    Whatsapp = x.Telefono
                })
                .ToListAsync();

            var response = new DatosEmpresaListadoDTO
            {
                TotalRegistros = totalRegistros,
                PaginaActual = page,
                RegistrosPorPagina = pageSize,
                Datos = datos
            };

            return Ok(response);
        }

        // GET: endpoint/datos-empresa/1
        // Replica la lógica del GET _Update(int Id)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var datosEmpresa = await _context.DatosEstructura
                .FirstOrDefaultAsync(x => x.Id == id);

            if (datosEmpresa == null)
            {
                return NotFound(new DatosEmpresaResponseDTO
                {
                    Status = 404,
                    Mensaje = "No se encontraron los datos de empresa solicitados.",
                    DatoEmpresa = null
                });
            }

            var dto = new DatosEmpresaDTO
            {
                Id = datosEmpresa.Id,
                Alias = datosEmpresa.Alias,
                CBU = datosEmpresa.CBU,
                Whatsapp = datosEmpresa.Telefono
            };

            return Ok(new DatosEmpresaResponseDTO
            {
                Status = 200,
                Mensaje = "Datos de empresa obtenidos correctamente.",
                DatoEmpresa = dto
            });
        }

        // PUT: endpoint/datos-empresa/1
        // Replica la lógica del POST _Update(DatosEmpresa datos)
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] DatosEmpresaDTO datos)
        {
            if (datos == null)
            {
                return BadRequest(new DatosEmpresaResponseDTO
                {
                    Status = 400,
                    Mensaje = "Los datos enviados son inválidos.",
                    DatoEmpresa = null
                });
            }

            if (id != datos.Id)
            {
                return BadRequest(new DatosEmpresaResponseDTO
                {
                    Status = 400,
                    Mensaje = "El Id enviado por la URL no coincide con el Id del cuerpo de la petición.",
                    DatoEmpresa = datos
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new DatosEmpresaResponseDTO
                {
                    Status = 400,
                    Mensaje = "El modelo enviado no es válido.",
                    DatoEmpresa = datos
                });
            }

            try
            {
                var datosEmpresa = await _context.DatosEstructura
                    .FirstOrDefaultAsync(x => x.Id == datos.Id);

                if (datosEmpresa == null)
                {
                    return NotFound(new DatosEmpresaResponseDTO
                    {
                        Status = 404,
                        Mensaje = "No se encontraron los datos de empresa a editar.",
                        DatoEmpresa = datos
                    });
                }

                datosEmpresa.Alias = datos.Alias;
                datosEmpresa.Telefono = datos.Whatsapp;
                datosEmpresa.CBU = datos.CBU;

                _context.DatosEstructura.Update(datosEmpresa);
                await _context.SaveChangesAsync();

                var responseDto = new DatosEmpresaDTO
                {
                    Id = datosEmpresa.Id,
                    Alias = datosEmpresa.Alias,
                    CBU = datosEmpresa.CBU,
                    Whatsapp = datosEmpresa.Telefono
                };

                return Ok(new DatosEmpresaResponseDTO
                {
                    Status = 200,
                    Mensaje = "Se editó correctamente los datos de empresa.",
                    DatoEmpresa = responseDto
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new DatosEmpresaResponseDTO
                {
                    Status = 500,
                    Mensaje = "Hubo un error al editar el dato de empresa. Inténtelo nuevamente más tarde.",
                    DatoEmpresa = datos
                });
            }
        }
    }
}