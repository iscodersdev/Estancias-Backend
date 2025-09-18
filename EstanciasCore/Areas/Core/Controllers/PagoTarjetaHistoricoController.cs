﻿using Commons.Models;
using DAL.Data;
using DAL.DTOs;
using DAL.Mobile;
using DAL.Models;
using DAL.Models.Core;
using EstanciasCore.Services;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.Controllers
{
    [Area("Core")]
    public class PagoTarjetaHistoricoController : EstanciasCoreController
    {
        public PagoTarjetaHistoricoController(EstanciasContext context) : base(context)
        {
            breadcumb.Add(new Message() { DisplayName = "Datos" });
        }

        public IActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Pagos Históricos" });
            ViewBag.Breadcrumb = breadcumb;
            return View();
        }

        public async Task<IActionResult> _ListadoPagoTarjetaNew()
        {
            //var today = DateTime.Today.AddDays(-7); ;

            //var c = _context.PagoTarjeta.Where(x => x.FechaComprobante < today).Count();

            //if (c < 1) { c = 1; }
            //page.SelectPage("/Clientes/_ListadoPagoTarjeta",
            //    _context.PagoTarjeta.Where(x => x.FechaComprobante < today).OrderByDescending(x=>x.FechaComprobante), c);

            return PartialView("_ListadoPagoTarjetaNew");
        }
        public async Task<IActionResult> _ListadoPagoTarjetaDataTable()
        {
            var today = DateTime.Today.AddDays(-7); ;

            var query = from p in _context.PagoTarjeta where p.FechaComprobante < today orderby p.FechaComprobante descending
                        select new DAL.DTOs.PagoTarjetaDataTableDTO
                        {
                            Id = p.Id,
                            Persona = (p.Persona!=null) ? p.Persona.Apellido+" "+p.Persona.Nombres : "---",
                            NroDocumento =(p.Persona!=null) ? p.Persona.NroDocumento : "---",
                            Usuario = (p.Persona!=null) ? p.Persona.Email : "---",
                            NroTarjeta =(p.Persona!=null) ? p.Persona.NroTarjeta : "---",
                            MontoAdeudado = p.MontoAdeudado.ToString().Replace(".", ","),
                            MontoInformado = p.MontoInformado.ToString().Replace(".", ","),
                            FechaVencimiento = (p.FechaVencimiento ?? DateTime.MinValue).ToString("dd/MM/yyyy"),
                            FechaComprobante = (p.FechaComprobante ?? DateTime.MinValue).ToString("dd/MM/yyyy"),
                            EstadoPago = p.EstadoPago.ToString(),
                            EstadoPagoId = ((int)p.EstadoPago),
                            ComprobantePago = p.ComprobantePago !=null ? true : false,
                            FechaOrden = Convert.ToInt32((p.FechaComprobante ?? DateTime.MinValue).ToString("yyyyMMdd")),
                            Observacion = (p.Observacion!=null) ? p.Observacion : "",
                        };
            return DataTable<PagoTarjetaDataTableDTO>(query.AsQueryable<PagoTarjetaDataTableDTO>());

            //return PartialView(page);
        }

        public IActionResult _Create()
        {
            return PartialView();
        }     


        public async Task<IActionResult> _VerComprobante(int Id)
        {

            PagoTarjeta pagoTarjeta = await _context.PagoTarjeta.FindAsync(Id);
            string comprobante = Convert.ToBase64String(pagoTarjeta.ComprobantePago);
            return PartialView("_VerComprobante", comprobante);
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> _Update(Paises paises)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Paises.Update(paises);
        //            await _context.SaveChangesAsync();
        //            AddPageAlerts(PageAlertType.Success, "Se editó correctamente el País " + paises.Nombre + ".");
        //            return RedirectToAction("Index", "Paises");
        //        }
        //        catch (Exception e)
        //        {
        //            AddPageAlerts(PageAlertType.Error, "Hubo un error al editar el País. Intentelo nuevamente mas tarde.");
        //            return RedirectToAction("Index", "Paises");
        //        }

        //    }
        //    else
        //    {
        //        return PartialView(paises);
        //    }
        //}


        public bool AprobarComprobante(int id)
        {
            try
            {

                PagoTarjeta pagoTarjeta = _context.PagoTarjeta.Where(s => s.Id == id).First();
                pagoTarjeta.EstadoPago = EstadoPago.Aprobado;
                _context.PagoTarjeta.Update(pagoTarjeta);
                _context.SaveChanges();
                Clientes cliente = _context.Clientes.Where(x => x.Persona.Id == pagoTarjeta.Persona.Id).FirstOrDefault();

                NotificacionesPersonas notificacion = new NotificacionesPersonas(){
                    Cliente = cliente,
                    Titulo = "Pago Aprobado",
                    Descripcion = "Se aprobo su comprobante de Pago",
                    FechaHora = DateTime.Now,
                    TomaConocimiento = null
                };
                _context.NotificacionesPersonas.Add(notificacion);
                _context.SaveChanges();
                return true;
            }
            catch (System.Exception e)
            {
                AddPageAlerts(PageAlertType.Success, "Hubo un error al Aprobar el Pago.");
                return false;
            }
        }


        public bool RechazarComprobante([FromBody] RechazarComprobanteDTO dto)
        {
            try
            {
                PagoTarjeta pagoTarjeta = _context.PagoTarjeta.Where(s => s.Id == dto.Id).First();
                pagoTarjeta.EstadoPago = EstadoPago.Rechazado;
                pagoTarjeta.Observacion = dto.Observacion;
                _context.PagoTarjeta.Update(pagoTarjeta);
                _context.SaveChanges();
                Clientes cliente = _context.Clientes.Where(x => x.Persona.Id == pagoTarjeta.Persona.Id).FirstOrDefault();

                NotificacionesPersonas notificacion = new NotificacionesPersonas()
                {
                    Cliente = cliente,
                    Titulo = "Pago Rechazado",
                    Descripcion = "Se rechazo su comprobante de Pago",
                    FechaHora = DateTime.Now,
                    TomaConocimiento = null
                };
                _context.NotificacionesPersonas.Add(notificacion);
                _context.SaveChanges();
                //common.EnviaNotificationWonderPushId("Pago Rechazado", "Se rechazo su comprobante de Pago", new string[] { cliente.Usuario.DeviceId });
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        [HttpPost]
        public JsonResult AprobarMasivo([FromBody] List<int> ids)
        {
            try
            {
                var pagosAprobar = _context.PagoTarjeta.Where(s => ids.Contains(s.Id)).ToList();
                foreach (var pagoTarjeta in pagosAprobar)
                {
                    pagoTarjeta.EstadoPago = EstadoPago.Aprobado;
                    _context.PagoTarjeta.Update(pagoTarjeta);
                }
                _context.SaveChanges();

                foreach (var pagoTarjeta in pagosAprobar)
                {
                    Clientes cliente = _context.Clientes.Where(x => x.Persona.Id == pagoTarjeta.Persona.Id).FirstOrDefault();
                    if (cliente != null)
                    {
                        NotificacionesPersonas notificacion = new NotificacionesPersonas()
                        {
                            Cliente = cliente,
                            Titulo = "Pago Aprobado",
                            Descripcion = "Se aprobó su comprobante de Pago",
                            FechaHora = DateTime.Now,
                            TomaConocimiento = null
                        };
                        _context.NotificacionesPersonas.Add(notificacion);
                        // Si tienes WonderPush, puedes enviarla aquí
                        // common.EnviaNotificationWonderPushId("Pago Aprobado", "Se aprobó su comprobante de Pago", new string[] { cliente.Usuario.DeviceId });
                    }
                }
                _context.SaveChanges();

                return Json(new { success = true, message = "Los pagos fueron aprobados." });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = "Error al aprobar los pagos." });
            }
        }

        [HttpPost]
        public JsonResult RechazarMasivo([FromBody] RechazarComprobanteMasivoDTO dto)
        {
            try
            {
                var pagosRechazar = _context.PagoTarjeta.Where(s => dto.Ids.Contains(s.Id)).ToList();
                foreach (var pagoTarjeta in pagosRechazar)
                {
                    pagoTarjeta.EstadoPago = EstadoPago.Rechazado;
                    pagoTarjeta.Observacion = dto.Observacion;
                    _context.PagoTarjeta.Update(pagoTarjeta);
                }
                _context.SaveChanges();

                foreach (var pagoTarjeta in pagosRechazar)
                {
                    Clientes cliente = _context.Clientes.Where(x => x.Persona.Id == pagoTarjeta.Persona.Id).FirstOrDefault();
                    if (cliente != null)
                    {
                        NotificacionesPersonas notificacion = new NotificacionesPersonas()
                        {
                            Cliente = cliente,
                            Titulo = "Pago Rechazado",
                            Descripcion = "Se rechazó su comprobante de Pago. Motivo: " + dto.Observacion,
                            FechaHora = DateTime.Now,
                            TomaConocimiento = null
                        };
                        _context.NotificacionesPersonas.Add(notificacion);
                        // common.EnviaNotificationWonderPushId("Pago Rechazado", "Se rechazó su comprobante de Pago. Motivo: " + dto.Observacion, new string[] { cliente.Usuario.DeviceId });
                    }
                }
                _context.SaveChanges(); 
                return Json(new { success = true, message = "Los pagos han sido rechazados." });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = "Hubo un error al rechazar los pagos." });
            }
        }

        [HttpGet]
        public IActionResult ExportarExcel(string ids)
        {
            try
            {
                var listaIds = ids.Split(',').Select(int.Parse).ToList();
                var query = _context.PagoTarjeta.Where(p => listaIds.Contains(p.Id));
                query = ApplyOrder(query, "FechaComprobante", "asc");
                var pagos = query.ToList();
                byte[] excelBytes = _generateXlsxBytes(pagos);

                string excelName = $"Comprobantes_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
            catch (Exception e)
            {
                return BadRequest("Hubo un error al generar el archivo Excel: " + e.Message);
            }
        }

        private IQueryable<PagoTarjeta> ApplyOrder(IQueryable<PagoTarjeta> query, string sortColumn, string sortDirection)
        {
            bool ascending = sortDirection == "asc";
            switch (sortColumn)
            {
                case "FechaComprobante":
                    return ascending ? query.OrderBy(p => p.FechaComprobante) : query.OrderByDescending(p => p.FechaComprobante);
                case "Persona.Apellido":
                    return ascending ? query.OrderBy(p => p.Persona.Apellido) : query.OrderByDescending(p => p.Persona.Apellido);
                default:
                    return query.OrderByDescending(p => p.FechaComprobante);
            }
        }

        private byte[] _generateXlsxBytes(List<PagoTarjeta> datos)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Pagos");
                var dataToExport = datos.Select(p => new
                {
                    Cliente = $"{p.Persona?.Apellido}, {p.Persona?.Nombres}",
                    NroDocumento = p.Persona?.NroDocumento,
                    FechaVencimiento = p.FechaVencimiento?.ToString("dd/MM/yyyy") ?? "",
                    FechaComprobante = p.FechaComprobante?.ToString("dd/MM/yyyy") ?? "",
                    MontoAdeudado = p.MontoAdeudado,
                    MontoInformado = p.MontoInformado,
                    Estado = p.EstadoPago.ToString()
                }).ToList();

                worksheet.Cells.LoadFromCollection(dataToExport, true);
                worksheet.Column(5).Style.Numberformat.Format = "$ #,##0.00";
                worksheet.Column(6).Style.Numberformat.Format = "$ #,##0.00";
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                return package.GetAsByteArray();
            }
        }
    }
}
