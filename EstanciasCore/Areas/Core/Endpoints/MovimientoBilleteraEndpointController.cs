using DAL.Data;
using DAL.DTOs;
using DAL.Models.Core;
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
    [Route("endpoint/movimiento-billetera")]
    public class MovimientoBilleteraEndpointController : ControllerBase
    {
        private readonly EstanciasContext _context;

        public MovimientoBilleteraEndpointController(EstanciasContext context)
        {
            _context = context;
        }

        // GET: endpoint/movimiento-billetera
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var movimientosBilletera = await _context.MovimientosBilletera
                    .Select(m => new MovimientoBilleteraDTO
                    {
                        Id = m.Id,
                        Fecha = m.Fecha,
                        Monto = m.Monto,
                        CBU = m.CBU,
                        TipoMovimiento = m.TipoMovimiento == null ? null : new TipoMovimientoBilleteraDTO
                        {
                            Id = m.TipoMovimiento.Id,
                            Nombre = m.TipoMovimiento.Nombre,
                            Credito = m.TipoMovimiento.Credito,
                            Debito = m.TipoMovimiento.Debito
                        }
                    })
                    .ToListAsync();

                return Ok(new MovimientoBilleteraResponseDTO
                {
                    Data = new MovimientoBilleteraListadoResponseDTO
                    {
                        MovimientosBilletera = movimientosBilletera
                    },
                    Status = 200,
                    Mensaje = "Listado de Movimientos de Billetera obtenido correctamente."
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new MovimientoBilleteraResponseDTO
                {
                    Data = null,
                    Status = 500,
                    Mensaje = "Hubo un error al obtener el listado de Movimientos de Billetera."
                });
            }
        }

        // GET: endpoint/movimiento-billetera/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var movimientoBilletera = await _context.MovimientosBilletera
                    .Where(m => m.Id == id)
                    .Select(m => new MovimientoBilleteraDTO
                    {
                        Id = m.Id,
                        Fecha = m.Fecha,
                        Monto = m.Monto,
                        CBU = m.CBU,
                        TipoMovimiento = m.TipoMovimiento == null ? null : new TipoMovimientoBilleteraDTO
                        {
                            Id = m.TipoMovimiento.Id,
                            Nombre = m.TipoMovimiento.Nombre,
                            Credito = m.TipoMovimiento.Credito,
                            Debito = m.TipoMovimiento.Debito
                        }
                    })
                    .FirstOrDefaultAsync();

                if (movimientoBilletera == null)
                {
                    return NotFound(new MovimientoBilleteraResponseDTO
                    {
                        Data = null,
                        Status = 404,
                        Mensaje = "No se encontró el Movimiento de Billetera solicitado."
                    });
                }

                return Ok(new MovimientoBilleteraResponseDTO
                {
                    Data = movimientoBilletera,
                    Status = 200,
                    Mensaje = "Movimiento de Billetera obtenido correctamente."
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new MovimientoBilleteraResponseDTO
                {
                    Data = null,
                    Status = 500,
                    Mensaje = "Hubo un error al obtener el Movimiento de Billetera."
                });
            }
        }

        // GET: endpoint/movimiento-billetera/tipos-movimiento
        [HttpGet("tipos-movimiento")]
        public async Task<IActionResult> GetTiposMovimiento()
        {
            try
            {
                var tiposMovimiento = await _context.TipoMovimientoBilletera
                    .Select(t => new TipoMovimientoBilleteraDTO
                    {
                        Id = t.Id,
                        Nombre = t.Nombre,
                        Credito = t.Credito,
                        Debito = t.Debito
                    })
                    .ToListAsync();

                return Ok(new MovimientoBilleteraResponseDTO
                {
                    Data = tiposMovimiento,
                    Status = 200,
                    Mensaje = "Listado de Tipos de Movimiento de Billetera obtenido correctamente."
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new MovimientoBilleteraResponseDTO
                {
                    Data = null,
                    Status = 500,
                    Mensaje = "Hubo un error al obtener los Tipos de Movimiento de Billetera."
                });
            }
        }

        // POST: endpoint/movimiento-billetera/crear
        [HttpPost("crear")]
        public async Task<IActionResult> Create([FromBody] MovimientoBilleteraCreateDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new MovimientoBilleteraResponseDTO
                    {
                        Data = null,
                        Status = 400,
                        Mensaje = "Los datos del Movimiento de Billetera son obligatorios."
                    });
                }

                var tipoMovimiento = await _context.TipoMovimientoBilletera
                    .FindAsync(dto.TipoMovimientoId);

                var movimientoBilletera = new MovimientoBilletera
                {
                    TipoMovimiento = tipoMovimiento,
                    Monto = dto.Monto,
                    CBU = dto.CBU
                };

                await _context.MovimientosBilletera.AddAsync(movimientoBilletera);
                await _context.SaveChangesAsync();

                return Ok(new MovimientoBilleteraResponseDTO
                {
                    Data = new MovimientoBilleteraDTO
                    {
                        Id = movimientoBilletera.Id,
                        Fecha = movimientoBilletera.Fecha,
                        Monto = movimientoBilletera.Monto,
                        CBU = movimientoBilletera.CBU,
                        TipoMovimiento = movimientoBilletera.TipoMovimiento == null ? null : new TipoMovimientoBilleteraDTO
                        {
                            Id = movimientoBilletera.TipoMovimiento.Id,
                            Nombre = movimientoBilletera.TipoMovimiento.Nombre,
                            Credito = movimientoBilletera.TipoMovimiento.Credito,
                            Debito = movimientoBilletera.TipoMovimiento.Debito
                        }
                    },
                    Status = 200,
                    Mensaje = "Se cargo correctamente el Movimiento de la Billetera."
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new MovimientoBilleteraResponseDTO
                {
                    Data = null,
                    Status = 500,
                    Mensaje = "Hubo un error al cargar el Movimiento de la Billetera. Intentelo nuevamente mas tarde."
                });
            }
        }

        // PUT: endpoint/movimiento-billetera/editar/5
        [HttpPut("editar/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] MovimientoBilleteraUpdateDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new MovimientoBilleteraResponseDTO
                    {
                        Data = null,
                        Status = 400,
                        Mensaje = "Los datos del Movimiento de Billetera son obligatorios."
                    });
                }

                if (id != dto.Id)
                {
                    return BadRequest(new MovimientoBilleteraResponseDTO
                    {
                        Data = null,
                        Status = 400,
                        Mensaje = "El Id enviado por ruta no coincide con el Id del Movimiento de Billetera."
                    });
                }

                var movimientoBilletera = await _context.MovimientosBilletera
                    .Where(m => m.Id == id)
                    .FirstOrDefaultAsync();

                if (movimientoBilletera == null)
                {
                    return NotFound(new MovimientoBilleteraResponseDTO
                    {
                        Data = null,
                        Status = 404,
                        Mensaje = "No se encontró el Movimiento de Billetera solicitado."
                    });
                }

                movimientoBilletera.TipoMovimiento = await _context.TipoMovimientoBilletera
                    .FindAsync(dto.TipoMovimientoId);

                movimientoBilletera.Monto = dto.Monto;
                movimientoBilletera.CBU = dto.CBU;

                _context.MovimientosBilletera.Update(movimientoBilletera);
                await _context.SaveChangesAsync();

                return Ok(new MovimientoBilleteraResponseDTO
                {
                    Data = new MovimientoBilleteraDTO
                    {
                        Id = movimientoBilletera.Id,
                        Fecha = movimientoBilletera.Fecha,
                        Monto = movimientoBilletera.Monto,
                        CBU = movimientoBilletera.CBU,
                        TipoMovimiento = movimientoBilletera.TipoMovimiento == null ? null : new TipoMovimientoBilleteraDTO
                        {
                            Id = movimientoBilletera.TipoMovimiento.Id,
                            Nombre = movimientoBilletera.TipoMovimiento.Nombre,
                            Credito = movimientoBilletera.TipoMovimiento.Credito,
                            Debito = movimientoBilletera.TipoMovimiento.Debito
                        }
                    },
                    Status = 200,
                    Mensaje = "Se modifico corretamente el Movimiento de la Billetera."
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new MovimientoBilleteraResponseDTO
                {
                    Data = null,
                    Status = 500,
                    Mensaje = "Hubo un error al modificar el Movimiento de la Billetera. Intentelo nuevamente mas tarde."
                });
            }
        }

        // DELETE: endpoint/movimiento-billetera/eliminar/5
        [HttpDelete("eliminar/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var movimientoBilletera = await _context.MovimientosBilletera
                    .Where(m => m.Id == id)
                    .FirstOrDefaultAsync();

                if (movimientoBilletera == null)
                {
                    return NotFound(new MovimientoBilleteraResponseDTO
                    {
                        Data = null,
                        Status = 404,
                        Mensaje = "No se encontró el Movimiento de Billetera solicitado."
                    });
                }

                _context.MovimientosBilletera.Remove(movimientoBilletera);
                await _context.SaveChangesAsync();

                return Ok(new MovimientoBilleteraResponseDTO
                {
                    Data = null,
                    Status = 200,
                    Mensaje = "Se eliminó correctamente el Movimiento de la Billetera."
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new MovimientoBilleteraResponseDTO
                {
                    Data = null,
                    Status = 500,
                    Mensaje = "Hubo un error al eliminar el Movimiento de la Billetera."
                });
            }
        }
    }
}