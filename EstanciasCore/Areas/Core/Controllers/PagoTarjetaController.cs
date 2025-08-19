using Commons.Models;
using DAL.Data;
using DAL.DTOs;
using DAL.Mobile;
using DAL.Models;
using DAL.Models.Core;
using EstanciasCore.Services;
using Microsoft.AspNetCore.Mvc;
using Renci.SshNet.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace EstanciasCore.Controllers
{
    [Area("Core")]

	public class PagoTarjetaController : EstanciasCoreController
    {

        public PagoTarjetaController(EstanciasContext context) : base(context)
        {
            breadcumb.Add(new Commons.Models.Message() { DisplayName = "Datos" });
		}

        public IActionResult Index()
        {
            breadcumb.Add(new Commons.Models.Message() { DisplayName = "Pago" });
            ViewBag.Breadcrumb = breadcumb;
            return View();
        }

        public async Task<IActionResult> _ListadoPagoTarjeta(Page<PagoTarjeta> page)
        {
            var today = DateTime.Today.AddDays(-7); ;

            var c = _context.PagoTarjeta.Where(x => x.FechaComprobante >= today && x.EstadoPago != EstadoPago.Pendiente).Count();

            if (c < 1) { c = 1; }
            page.SelectPage("/Clientes/_ListadoPagoTarjeta",
                _context.PagoTarjeta.Where(x => x.FechaComprobante >= today && x.EstadoPago != EstadoPago.Pendiente).OrderByDescending(x=>x.FechaComprobante), c);

            return PartialView("_ListadoPagoTarjeta", page);
        }

        public async Task<IActionResult> _ListadoPagoTarjetaNew()
        {
            return PartialView("_ListadoPagoTarjetaNew");
        }

        public async Task<IActionResult> _ListadoPagoTarjetaDataTable()
        {
            var today = DateTime.Today.AddDays(-7); ;

            var query = from p in _context.PagoTarjeta
                        where p.FechaComprobante >= today && p.EstadoPago != EstadoPago.Pendiente
                        orderby p.FechaComprobante descending
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
                            FechaOrden = Convert.ToInt32((p.FechaComprobante ?? DateTime.MinValue).ToString("yyyyMMdd"))
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



        public async Task<bool> AprobarComprobante(int id)
        {

            //common.EnviaNotificationWonderPushId("Pago Aprobado", "Se aprobó su comprobante de Pago", new string[] { "abc10ba8-89fc-465c-a41f-9b5b9b2d466f" });
            //return true;
            try
            {
                PagoTarjeta pagoTarjeta = _context.PagoTarjeta.Where(s => s.Id == id).First();
                pagoTarjeta.EstadoPago = EstadoPago.Aprobado;
                _context.PagoTarjeta.Update(pagoTarjeta);
                _context.SaveChanges();
                Clientes cliente = _context.Clientes.Where(x => x.Persona.Id == pagoTarjeta.Persona.Id).FirstOrDefault();

                NotificacionesPersonas notificacion = new NotificacionesPersonas()
                {
                    Cliente = cliente,
                    Titulo = "Pago Aprobado",
                    Descripcion = "Se aprobo su comprobante de Pago",
                    FechaHora = DateTime.Now,
                    TomaConocimiento = null
                };
                _context.NotificacionesPersonas.Add(notificacion);
                _context.SaveChanges();
                common.EnviaNotificationWonderPushId("Pago Aprobado", "Se aprobó su comprobante de Pago", new string[] { cliente.Usuario.DeviceId });
                return true;
            }
            catch (System.Exception e)
            {
                AddPageAlerts(PageAlertType.Success, "Hubo un error al Aprobar el Pago.");
                return false;
            }
        }


        public bool RechazarComprobante(int id)
        {
            try
            {
                PagoTarjeta pagoTarjeta = _context.PagoTarjeta.Where(s => s.Id == id).First();
                pagoTarjeta.EstadoPago = EstadoPago.Rechazado;
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
                common.EnviaNotificationWonderPushId("Pago Rechazado", "Se rechazo su comprobante de Pago", new string[] { cliente.Usuario.DeviceId });
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

    }
}
