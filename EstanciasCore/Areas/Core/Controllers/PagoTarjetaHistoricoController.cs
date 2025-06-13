using Commons.Models;
using DAL.Data;
using DAL.DTOs;
using DAL.Mobile;
using DAL.Models;
using DAL.Models.Core;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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


        public bool RechazarComprobante(int id)
        {
            try
            {
                PagoTarjeta pagoTarjeta = _context.PagoTarjeta.Where(s => s.Id == id).First();
                pagoTarjeta.EstadoPago = EstadoPago.Rechazado;
                _context.PagoTarjeta.Update(pagoTarjeta);
                _context.SaveChanges(); 
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

    }
}
