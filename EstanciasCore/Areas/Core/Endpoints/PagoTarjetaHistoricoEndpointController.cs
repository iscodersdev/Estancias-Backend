using DAL.Data;
using DAL.DTOs;
using DAL.Models;
using DAL.Models.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.Endpoints
{
    [Route("endpoint/pago-tarjeta-historico")]
    [ApiController]
    public class PagoTarjetaHistoricoEndpointController : ControllerBase
    {
        private readonly EstanciasContext _context;

        public PagoTarjetaHistoricoEndpointController(EstanciasContext context)
        {
            _context = context;
        }

        // GET: endpoint/pago-tarjeta-historico
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string buscar = null,
            [FromQuery] int? estado = null,
            [FromQuery] DateTime? fecha = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (page < 1)
                page = 1;

            if (pageSize < 1)
                pageSize = 10;

            if (pageSize > 100)
                pageSize = 100;

            var today = DateTime.Today.AddDays(-7);

            var query = _context.PagoTarjeta
                .AsNoTracking()
                .Include(p => p.Persona)
                .Where(p => p.FechaComprobante < today)
                .AsQueryable();

            // Filtro barra de búsqueda: cliente o documento
            if (!string.IsNullOrWhiteSpace(buscar))
            {
                var texto = buscar.Trim().ToLower();

                query = query.Where(p =>
                    p.Persona != null &&
                    (
                        ((p.Persona.Apellido ?? "") + " " + (p.Persona.Nombres ?? "")).ToLower().Contains(texto) ||
                        ((p.Persona.Nombres ?? "") + " " + (p.Persona.Apellido ?? "")).ToLower().Contains(texto) ||
                        (p.Persona.NroDocumento ?? "").ToLower().Contains(texto)
                    )
                );
            }

            // Filtro por estado
            if (estado.HasValue)
            {
                query = query.Where(p => (int)p.EstadoPago == estado.Value);
            }

            // Filtro por fecha de comprobante
            if (fecha.HasValue)
            {
                var desde = fecha.Value.Date;
                var hasta = desde.AddDays(1);

                query = query.Where(p =>
                    p.FechaComprobante.HasValue &&
                    p.FechaComprobante.Value >= desde &&
                    p.FechaComprobante.Value < hasta
                );
            }

            var total = await query.CountAsync();

            var pagos = await query
                .OrderByDescending(p => p.FechaComprobante)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PagoTarjetaDataTableDTO
                {
                    Id = p.Id,

                    Persona = p.Persona != null
                        ? p.Persona.Apellido + " " + p.Persona.Nombres
                        : "---",

                    NroDocumento = p.Persona != null
                        ? p.Persona.NroDocumento
                        : "---",

                    Usuario = p.Persona != null
                        ? p.Persona.Email
                        : "---",

                    NroTarjeta = p.Persona != null
                        ? p.Persona.NroTarjeta
                        : "---",

                    MontoAdeudado = p.MontoAdeudado.ToString().Replace(".", ","),
                    MontoInformado = p.MontoInformado.ToString().Replace(".", ","),

                    FechaVencimiento = p.FechaVencimiento.HasValue
                        ? p.FechaVencimiento.Value.ToString("dd/MM/yyyy")
                        : "",

                    FechaComprobante = p.FechaComprobante.HasValue
                        ? p.FechaComprobante.Value.ToString("dd/MM/yyyy")
                        : "",

                    FechaDePago = p.FechaDePago.HasValue
                        ? p.FechaDePago.Value.ToString("dd/MM/yyyy")
                        : "",

                    FechaPagoProximaCuota = p.FechaPagoProximaCuota.HasValue
                        ? p.FechaPagoProximaCuota.Value.ToString("dd/MM/yyyy")
                        : "",

                    EstadoPago = p.EstadoPago.ToString(),
                    EstadoPagoId = (int)p.EstadoPago,

                    // Importante: NO traer el byte[]
                    ComprobantePago = p.ComprobantePago != null,

                    FechaOrden = p.FechaComprobante.HasValue
                        ? Convert.ToInt32(p.FechaComprobante.Value.ToString("yyyyMMdd"))
                        : 0,

                    Observacion = p.Observacion ?? ""
                })
                .ToListAsync();

            return Ok(new
            {
                ok = true,
                total,
                page,
                pageSize,
                totalPages = (int)Math.Ceiling(total / (double)pageSize),
                data = pagos
            });
        }

        // GET: endpoint/pago-tarjeta-historico/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var pago = await _context.PagoTarjeta
                .AsNoTracking()
                .Include(p => p.Persona)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pago == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el pago."
                });
            }

            var dto = new PagoTarjetaDTO
            {
                Id = pago.Id,

                Persona = pago.Persona != null
                    ? pago.Persona.Apellido + " " + pago.Persona.Nombres
                    : "---",

                NroDocumento = pago.Persona != null
                    ? pago.Persona.NroDocumento
                    : "---",

                Usuario = pago.Persona != null
                    ? pago.Persona.Email
                    : "---",

                NroTarjeta = pago.Persona != null
                    ? pago.Persona.NroTarjeta
                    : "---",

                FechaVencimiento = pago.FechaVencimiento ?? DateTime.MinValue,
                FechaPagoProximaCuota = pago.FechaPagoProximaCuota ?? DateTime.MinValue,
                MontoAdeudado = pago.MontoAdeudado,
                EstadoPago = pago.EstadoPago,

                // Acá sí se puede devolver porque es detalle.
                ComprobantePago = pago.ComprobantePago
            };

            return Ok(new
            {
                ok = true,
                data = dto
            });
        }

        // GET: endpoint/pago-tarjeta-historico/{id}/comprobante
        [HttpGet("{id}/comprobante")]
        public async Task<IActionResult> VerComprobante(int id)
        {
            var pagoTarjeta = await _context.PagoTarjeta
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pagoTarjeta == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el pago."
                });
            }

            if (pagoTarjeta.ComprobantePago == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "El pago no tiene comprobante cargado."
                });
            }

            var comprobante = Convert.ToBase64String(pagoTarjeta.ComprobantePago);

            return Ok(new
            {
                ok = true,
                data = comprobante
            });
        }

        // PUT: endpoint/pago-tarjeta-historico/{id}/aprobar
        [HttpPut("{id}/aprobar")]
        public async Task<IActionResult> AprobarComprobante(int id)
        {
            try
            {
                var pagoTarjeta = await _context.PagoTarjeta
                    .Include(p => p.Persona)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (pagoTarjeta == null)
                {
                    return NotFound(new
                    {
                        ok = false,
                        message = "No se encontró el pago."
                    });
                }

                pagoTarjeta.EstadoPago = EstadoPago.Aprobado;

                _context.PagoTarjeta.Update(pagoTarjeta);
                await _context.SaveChangesAsync();

                var cliente = await _context.Clientes
                    .Include(c => c.Persona)
                    .FirstOrDefaultAsync(x => x.Persona.Id == pagoTarjeta.Persona.Id);

                if (cliente != null)
                {
                    var notificacion = new NotificacionesPersonas
                    {
                        Cliente = cliente,
                        Titulo = "Pago Aprobado",
                        Descripcion = "Se aprobo su comprobante de Pago",
                        FechaHora = DateTime.Now,
                        TomaConocimiento = null
                    };

                    _context.NotificacionesPersonas.Add(notificacion);
                    await _context.SaveChangesAsync();
                }

                return Ok(new
                {
                    ok = true,
                    message = "El pago fue aprobado correctamente."
                });
            }
            catch
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Hubo un error al aprobar el pago."
                });
            }
        }

        // PUT: endpoint/pago-tarjeta-historico/rechazar
        [HttpPut("rechazar")]
        public async Task<IActionResult> RechazarComprobante([FromBody] RechazarComprobanteDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new
                    {
                        ok = false,
                        message = "Datos inválidos."
                    });
                }

                var pagoTarjeta = await _context.PagoTarjeta
                    .Include(p => p.Persona)
                    .FirstOrDefaultAsync(p => p.Id == dto.Id);

                if (pagoTarjeta == null)
                {
                    return NotFound(new
                    {
                        ok = false,
                        message = "No se encontró el pago."
                    });
                }

                pagoTarjeta.EstadoPago = EstadoPago.Rechazado;
                pagoTarjeta.Observacion = dto.Observacion;

                _context.PagoTarjeta.Update(pagoTarjeta);
                await _context.SaveChangesAsync();

                var cliente = await _context.Clientes
                    .Include(c => c.Persona)
                    .FirstOrDefaultAsync(x => x.Persona.Id == pagoTarjeta.Persona.Id);

                if (cliente != null)
                {
                    var notificacion = new NotificacionesPersonas
                    {
                        Cliente = cliente,
                        Titulo = "Pago Rechazado",
                        Descripcion = "Se rechazo su comprobante de Pago",
                        FechaHora = DateTime.Now,
                        TomaConocimiento = null
                    };

                    _context.NotificacionesPersonas.Add(notificacion);
                    await _context.SaveChangesAsync();
                }

                return Ok(new
                {
                    ok = true,
                    message = "El pago fue rechazado correctamente."
                });
            }
            catch
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Hubo un error al rechazar el pago."
                });
            }
        }

        // POST: endpoint/pago-tarjeta-historico/aprobar-masivo
        [HttpPost("aprobar-masivo")]
        public async Task<IActionResult> AprobarMasivo([FromBody] List<int> ids)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return BadRequest(new
                    {
                        ok = false,
                        message = "Debe enviar al menos un pago para aprobar."
                    });
                }

                var pagosAprobar = await _context.PagoTarjeta
                    .Include(p => p.Persona)
                    .Where(s => ids.Contains(s.Id))
                    .ToListAsync();

                foreach (var pagoTarjeta in pagosAprobar)
                {
                    pagoTarjeta.EstadoPago = EstadoPago.Aprobado;
                    _context.PagoTarjeta.Update(pagoTarjeta);
                }

                await _context.SaveChangesAsync();

                foreach (var pagoTarjeta in pagosAprobar)
                {
                    if (pagoTarjeta.Persona == null)
                        continue;

                    var cliente = await _context.Clientes
                        .Include(c => c.Persona)
                        .FirstOrDefaultAsync(x => x.Persona.Id == pagoTarjeta.Persona.Id);

                    if (cliente != null)
                    {
                        var notificacion = new NotificacionesPersonas
                        {
                            Cliente = cliente,
                            Titulo = "Pago Aprobado",
                            Descripcion = "Se aprobó su comprobante de Pago",
                            FechaHora = DateTime.Now,
                            TomaConocimiento = null
                        };

                        _context.NotificacionesPersonas.Add(notificacion);
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    ok = true,
                    message = "Los pagos fueron aprobados."
                });
            }
            catch
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Error al aprobar los pagos."
                });
            }
        }

        // POST: endpoint/pago-tarjeta-historico/rechazar-masivo
        [HttpPost("rechazar-masivo")]
        public async Task<IActionResult> RechazarMasivo([FromBody] RechazarComprobanteMasivoDTO dto)
        {
            try
            {
                if (dto == null || dto.Ids == null || !dto.Ids.Any())
                {
                    return BadRequest(new
                    {
                        ok = false,
                        message = "Debe enviar al menos un pago para rechazar."
                    });
                }

                var pagosRechazar = await _context.PagoTarjeta
                    .Include(p => p.Persona)
                    .Where(s => dto.Ids.Contains(s.Id))
                    .ToListAsync();

                foreach (var pagoTarjeta in pagosRechazar)
                {
                    pagoTarjeta.EstadoPago = EstadoPago.Rechazado;
                    pagoTarjeta.Observacion = dto.Observacion;
                    _context.PagoTarjeta.Update(pagoTarjeta);
                }

                await _context.SaveChangesAsync();

                foreach (var pagoTarjeta in pagosRechazar)
                {
                    if (pagoTarjeta.Persona == null)
                        continue;

                    var cliente = await _context.Clientes
                        .Include(c => c.Persona)
                        .FirstOrDefaultAsync(x => x.Persona.Id == pagoTarjeta.Persona.Id);

                    if (cliente != null)
                    {
                        var notificacion = new NotificacionesPersonas
                        {
                            Cliente = cliente,
                            Titulo = "Pago Rechazado",
                            Descripcion = "Se rechazó su comprobante de Pago. Motivo: " + dto.Observacion,
                            FechaHora = DateTime.Now,
                            TomaConocimiento = null
                        };

                        _context.NotificacionesPersonas.Add(notificacion);
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    ok = true,
                    message = "Los pagos han sido rechazados."
                });
            }
            catch
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Hubo un error al rechazar los pagos."
                });
            }
        }

        // GET: endpoint/pago-tarjeta-historico/exportar-excel?ids=1,2,3
        [HttpGet("exportar-excel")]
        public async Task<IActionResult> ExportarExcel([FromQuery] string ids)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ids))
                {
                    return BadRequest(new
                    {
                        ok = false,
                        message = "Debe enviar al menos un ID."
                    });
                }

                var listaIds = ids
                    .Split(',')
                    .Select(int.Parse)
                    .ToList();

                var pagos = await _context.PagoTarjeta
                    .Include(p => p.Persona)
                    .Where(p => listaIds.Contains(p.Id))
                    .OrderBy(p => p.FechaComprobante)
                    .ToListAsync();

                var excelBytes = GenerateXlsxBytes(pagos);

                var excelName = $"Comprobantes_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

                return File(
                    excelBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    excelName
                );
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Hubo un error al generar el archivo Excel: " + e.Message
                });
            }
        }

        private byte[] GenerateXlsxBytes(List<PagoTarjeta> datos)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Pagos");

                var dataToExport = datos.Select(p => new
                {
                    Cliente = $"{p.Persona?.Apellido}, {p.Persona?.Nombres}",
                    NroDocumento = p.Persona?.NroDocumento,
                    FechaVencimiento = p.FechaDePago?.ToString("dd/MM/yyyy") ?? "",
                    FechaComprobante = p.FechaComprobante?.ToString("dd/MM/yyyy HH:mm") ?? "",
                    MontoInformado = p.MontoInformado,
                    Estado = p.EstadoPago.ToString()
                }).ToList();

                worksheet.Cells.LoadFromCollection(dataToExport, true);

                worksheet.Cells["A1"].Value = "Cliente";
                worksheet.Cells["B1"].Value = "NroDocumento.";
                worksheet.Cells["C1"].Value = "Fecha Informada";
                worksheet.Cells["D1"].Value = "Fecha de Carga";
                worksheet.Cells["E1"].Value = "Monto Informado";
                worksheet.Cells["F1"].Value = "Estado";

                worksheet.Column(5).Style.Numberformat.Format = "$ #,##0.00";
                worksheet.Column(6).Style.Numberformat.Format = "$ #,##0.00";

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                return package.GetAsByteArray();
            }
        }
    }
}