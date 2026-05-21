using DAL.Data;
using DAL.DTOs;
using DAL.DTOs.Reportes;
using DAL.Models;
using DAL.Models.Core;
using EstanciasCore.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.Endpoints
{
    [Route("endpoint/pago-tarjeta")]
    [ApiController]
    public class PagoTarjetaEndpointController : ControllerBase
    {
        private readonly EstanciasContext _context;
        private readonly IPushService _wonderPushService;

        public PagoTarjetaEndpointController(
            EstanciasContext context,
            IPushService wonderPushService)
        {
            _context = context;
            _wonderPushService = wonderPushService;
        }

        // GET: endpoint/pago-tarjeta
        [HttpGet]
        public async Task<IActionResult> GetAll(
        [FromQuery] string buscar = null,
        [FromQuery] int? estado = null,
        [FromQuery] DateTime? fecha = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
        {
            if (page < 1)
                page = 1;

            if (pageSize < 1)
                pageSize = 20;

            if (pageSize > 100)
                pageSize = 100;

            var query = _context.PagoTarjeta
                .AsNoTracking()
                .Include(p => p.Persona)
                .AsQueryable();

            // Filtro por búsqueda: nombre, apellido o documento
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
                        ? (p.Persona.Apellido + " " + p.Persona.Nombres)
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

                    FechaDePago = p.FechaDePago.HasValue
                        ? p.FechaDePago.Value.ToString("dd/MM/yyyy")
                        : "",

                    FechaVencimiento = p.FechaVencimiento.HasValue
                        ? p.FechaVencimiento.Value.ToString("dd/MM/yyyy")
                        : "",

                    MontoAdeudado = p.MontoAdeudado.ToString(),

                    MontoInformado = p.MontoInformado.ToString(),

                    FechaPagoProximaCuota = p.FechaPagoProximaCuota.HasValue
                        ? p.FechaPagoProximaCuota.Value.ToString("dd/MM/yyyy")
                        : "",

                    FechaComprobante = p.FechaComprobante.HasValue
                        ? p.FechaComprobante.Value.ToString("dd/MM/yyyy")
                        : "",

                    EstadoPago = p.EstadoPago.ToString(),
                    EstadoPagoId = (int)p.EstadoPago,

                    // CLAVE: esto NO trae el archivo, solo dice si existe
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
                total = total,
                page = page,
                pageSize = pageSize,
                totalPages = (int)Math.Ceiling(total / (double)pageSize),
                data = pagos
            });
        }

        // GET: endpoint/pago-tarjeta/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var pago = await _context.PagoTarjeta
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
                MontoAdeudado = pago.MontoAdeudado,
                EstadoPago = pago.EstadoPago,
                ComprobantePago = pago.ComprobantePago
            };

            return Ok(new
            {
                ok = true,
                data = dto
            });
        }

        // GET: endpoint/pago-tarjeta/{id}/comprobante
        [HttpGet("{id}/comprobante")]
        public async Task<IActionResult> VerComprobante(int id)
        {
            var pago = await _context.PagoTarjeta
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pago == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el pago."
                });
            }

            if (pago.ComprobantePago == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "El pago no tiene comprobante cargado."
                });
            }

            var base64 = Convert.ToBase64String(pago.ComprobantePago);

            return Ok(new
            {
                ok = true,
                data = base64
            });
        }

        // PUT: endpoint/pago-tarjeta/{id}/aprobar
        [HttpPut("{id}/aprobar")]
        public async Task<IActionResult> AprobarComprobante(int id)
        {
            var pago = await _context.PagoTarjeta
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

            pago.EstadoPago = EstadoPago.Aprobado;
            _context.PagoTarjeta.Update(pago);

            var cliente = await _context.Clientes
                .Include(c => c.Persona)
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.Persona.Id == pago.Persona.Id);

            if (cliente != null)
            {
                var notificacionPersona = new NotificacionesPersonas
                {
                    Cliente = cliente,
                    Titulo = "Pago Aprobado",
                    Descripcion = "Se aprobó su comprobante de Pago",
                    FechaHora = DateTime.Now,
                    TomaConocimiento = null
                };

                _context.NotificacionesPersonas.Add(notificacionPersona);
            }

            await _context.SaveChangesAsync();

            await EnviarPushSiCorresponde(cliente, "PA");

            return Ok(new
            {
                ok = true,
                message = "El pago fue aprobado correctamente."
            });
        }

        // PUT: endpoint/pago-tarjeta/rechazar
        [HttpPut("rechazar")]
        public async Task<IActionResult> RechazarComprobante([FromBody] RechazarComprobanteDTO dto)
        {
            if (dto == null)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Datos inválidos."
                });
            }

            var pago = await _context.PagoTarjeta
                .Include(p => p.Persona)
                .FirstOrDefaultAsync(p => p.Id == dto.Id);

            if (pago == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el pago."
                });
            }

            pago.EstadoPago = EstadoPago.Rechazado;
            pago.Observacion = dto.Observacion;

            _context.PagoTarjeta.Update(pago);

            var cliente = await _context.Clientes
                .Include(c => c.Persona)
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.Persona.Id == pago.Persona.Id);

            if (cliente != null)
            {
                var notificacionPersona = new NotificacionesPersonas
                {
                    Cliente = cliente,
                    Titulo = "Pago Rechazado",
                    Descripcion = "Se rechazó su comprobante de Pago",
                    FechaHora = DateTime.Now,
                    TomaConocimiento = null
                };

                _context.NotificacionesPersonas.Add(notificacionPersona);
            }

            await _context.SaveChangesAsync();

            await EnviarPushSiCorresponde(cliente, "PR");

            return Ok(new
            {
                ok = true,
                message = "El pago fue rechazado correctamente."
            });
        }

        // POST: endpoint/pago-tarjeta/aprobar-masivo
        [HttpPost("aprobar-masivo")]
        public async Task<IActionResult> AprobarMasivo([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Debe enviar al menos un pago para aprobar."
                });
            }

            var pagos = await _context.PagoTarjeta
                .Include(p => p.Persona)
                .Where(p => ids.Contains(p.Id))
                .ToListAsync();

            if (!pagos.Any())
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontraron pagos para aprobar."
                });
            }

            var installationIds = new List<string>();

            foreach (var pago in pagos)
            {
                pago.EstadoPago = EstadoPago.Aprobado;
                _context.PagoTarjeta.Update(pago);

                if (pago.Persona == null)
                    continue;

                var cliente = await _context.Clientes
                    .Include(c => c.Persona)
                    .Include(c => c.Usuario)
                    .FirstOrDefaultAsync(c => c.Persona.Id == pago.Persona.Id);

                if (cliente == null)
                    continue;

                var notificacionPersona = new NotificacionesPersonas
                {
                    Cliente = cliente,
                    Titulo = "Pago Aprobado",
                    Descripcion = "Se aprobó su comprobante de Pago",
                    FechaHora = DateTime.Now,
                    TomaConocimiento = null
                };

                _context.NotificacionesPersonas.Add(notificacionPersona);

                if (cliente.Usuario != null && !string.IsNullOrEmpty(cliente.Usuario.DeviceId))
                {
                    installationIds.Add(cliente.Usuario.DeviceId);
                }
            }

            await _context.SaveChangesAsync();

            await EnviarPushMasivoSiCorresponde(installationIds, "PA");

            return Ok(new
            {
                ok = true,
                message = "Los pagos fueron aprobados correctamente."
            });
        }

        // POST: endpoint/pago-tarjeta/rechazar-masivo
        [HttpPost("rechazar-masivo")]
        public async Task<IActionResult> RechazarMasivo([FromBody] RechazarComprobanteMasivoDTO dto)
        {
            if (dto == null || dto.Ids == null || !dto.Ids.Any())
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Debe enviar al menos un pago para rechazar."
                });
            }

            var pagos = await _context.PagoTarjeta
                .Include(p => p.Persona)
                .Where(p => dto.Ids.Contains(p.Id))
                .ToListAsync();

            if (!pagos.Any())
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontraron pagos para rechazar."
                });
            }

            var installationIds = new List<string>();

            foreach (var pago in pagos)
            {
                pago.EstadoPago = EstadoPago.Rechazado;
                pago.Observacion = dto.Observacion;

                _context.PagoTarjeta.Update(pago);

                if (pago.Persona == null)
                    continue;

                var cliente = await _context.Clientes
                    .Include(c => c.Persona)
                    .Include(c => c.Usuario)
                    .FirstOrDefaultAsync(c => c.Persona.Id == pago.Persona.Id);

                if (cliente == null)
                    continue;

                var notificacionPersona = new NotificacionesPersonas
                {
                    Cliente = cliente,
                    Titulo = "Pago Rechazado",
                    Descripcion = "Se rechazó su comprobante de Pago. Motivo: " + dto.Observacion,
                    FechaHora = DateTime.Now,
                    TomaConocimiento = null
                };

                _context.NotificacionesPersonas.Add(notificacionPersona);

                if (cliente.Usuario != null && !string.IsNullOrEmpty(cliente.Usuario.DeviceId))
                {
                    installationIds.Add(cliente.Usuario.DeviceId);
                }
            }

            await _context.SaveChangesAsync();

            await EnviarPushMasivoSiCorresponde(installationIds, "PR");

            return Ok(new
            {
                ok = true,
                message = "Los pagos fueron rechazados correctamente."
            });
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new
            {
                ok = true,
                message = "Pago tarjeta endpoint funcionando"
            });
        }

        // POST: endpoint/pago-tarjeta/exportar-excel
        [HttpPost("exportar-excel")]
        public async Task<IActionResult> ExportarExcel([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Debe enviar al menos un pago para exportar."
                });
            }

            var pagos = await _context.PagoTarjeta
                .Include(p => p.Persona)
                .Where(p => ids.Contains(p.Id))
                .OrderBy(p => p.FechaComprobante)
                .ToListAsync();

            if (!pagos.Any())
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontraron pagos para exportar."
                });
            }

            var excelBytes = GenerateXlsxBytes(pagos);
            var excelName = $"Comprobantes_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

            return File(
                excelBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                excelName
            );
        }

        private async Task EnviarPushSiCorresponde(Clientes cliente, string codigoNotificacion)
        {
            if (cliente == null || cliente.Usuario == null || string.IsNullOrEmpty(cliente.Usuario.DeviceId))
                return;

            var notificacion = await _context.Notificaciones
                .Include(n => n.NotificacionesPlantillas)
                .FirstOrDefaultAsync(n => n.Codigo == codigoNotificacion);

            if (notificacion == null || notificacion.NotificacionesPlantillas == null)
                return;

            var notificacionDto = new NotificacionViewModelDTO
            {
                Titulo = notificacion.NotificacionesPlantillas.Titulo,
                Mensaje = notificacion.NotificacionesPlantillas.Mensaje,
                ImagenUrl = notificacion.NotificacionesPlantillas.ImagenUrl,
                DeepLink = notificacion.NotificacionesPlantillas.DeepLink
            };

            var installationIds = new List<string>
            {
                cliente.Usuario.DeviceId
            };

            await _wonderPushService.EnviarNotificacionAIds(notificacionDto, installationIds);
        }

        private async Task EnviarPushMasivoSiCorresponde(List<string> installationIds, string codigoNotificacion)
        {
            if (installationIds == null || !installationIds.Any())
                return;

            var notificacion = await _context.Notificaciones
                .Include(n => n.NotificacionesPlantillas)
                .FirstOrDefaultAsync(n => n.Codigo == codigoNotificacion);

            if (notificacion == null || notificacion.NotificacionesPlantillas == null)
                return;

            var notificacionDto = new NotificacionViewModelDTO
            {
                Titulo = notificacion.NotificacionesPlantillas.Titulo,
                Mensaje = notificacion.NotificacionesPlantillas.Mensaje,
                ImagenUrl = notificacion.NotificacionesPlantillas.ImagenUrl,
                DeepLink = notificacion.NotificacionesPlantillas.DeepLink
            };

            await _wonderPushService.EnviarNotificacionAIds(notificacionDto, installationIds);
        }

        private byte[] GenerateXlsxBytes(List<PagoTarjeta> datos)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Pagos");

                var dataToExport = datos.Select(p => new
                {
                    Cliente = p.Persona != null
                        ? $"{p.Persona.Apellido}, {p.Persona.Nombres}"
                        : "",

                    NroDocumento = p.Persona != null
                        ? p.Persona.NroDocumento
                        : "",

                    FechaInformada = p.FechaDePago.HasValue
                        ? p.FechaDePago.Value.ToString("dd/MM/yyyy")
                        : "",

                    FechaDeCarga = p.FechaComprobante.HasValue
                        ? p.FechaComprobante.Value.ToString("dd/MM/yyyy HH:mm")
                        : "",

                    MontoInformado = p.MontoInformado,

                    Estado = p.EstadoPago.ToString()
                }).ToList();

                worksheet.Cells.LoadFromCollection(dataToExport, true);

                worksheet.Cells["A1"].Value = "Cliente";
                worksheet.Cells["B1"].Value = "NroDocumento";
                worksheet.Cells["C1"].Value = "Fecha Informada";
                worksheet.Cells["D1"].Value = "Fecha de Carga";
                worksheet.Cells["E1"].Value = "Monto Informado";
                worksheet.Cells["F1"].Value = "Estado";

                worksheet.Column(5).Style.Numberformat.Format = "$ #,##0.00";
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                return package.GetAsByteArray();
            }
        }
    }
}