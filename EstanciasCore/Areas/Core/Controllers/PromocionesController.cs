using Commons.Models;
using Microsoft.AspNetCore.Mvc;
using DAL.Data;
using DAL.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace EstanciasCore.Controllers
{
    [Area("Core")]
    public class PromocionesController : EstanciasCoreController
    {
        public PromocionesController(EstanciasContext context) : base(context)
        {
            breadcumb.Add(new Message() { DisplayName = "Promociones" });
        }
        public ActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Promociones" });
            return View();
        }
        public ActionResult Create()
        {
            return PartialView();
        }
        public IActionResult ObtenerPromociones(Page<Promociones> page)
        {
            var usuario = _context.Usuarios.FirstOrDefault(x => x.Email == User.Identity.Name);

            if (usuario == null || usuario.Clientes?.Empresa == null)
            {
                var promocionesFiltradas = _context.Promociones
                    .Where(x => x.Empresa == null && (x.Titulo.Contains(page.SearchText) || x.Texto.Contains(page.SearchText)))
                    .OrderBy(x => x.Orden); // Ordenar por Titulo o el campo que desees

                page.SelectPage("/Promociones/ObtenerPromociones", promocionesFiltradas.OrderBy(x => x.Orden));
            }
            else
            {
                var promocionesFiltradas = _context.Promociones
                    .Where(x => x.Empresa.Id == usuario.Clientes.Empresa.Id && (x.Titulo.Contains(page.SearchText) || x.Texto.Contains(page.SearchText)))
                    .OrderBy(x => x.Orden); // Ordenar por Titulo o el campo que desees

                page.SelectPage("/Promociones/ObtenerPromociones", promocionesFiltradas.OrderBy(x => x.Orden));
            }

            return PartialView("_ListadoPromociones", page);
        }


        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> Create(Promociones promociones, int colorId, int PromocionFija, string TieneFechaVencimiento)
        {
            try
            {
                var usuario = _context.Usuarios.FirstOrDefault(x => x.Email == User.Identity.Name);
                promociones.Empresa = usuario.Clientes?.Empresa;
                promociones.QR =false;
                if (TieneFechaVencimiento=="on")
                {
                    promociones.Vencimiento = true;
                }
                if (PromocionFija==1)
                {
                    promociones.PromocionFija = true;
                }
                else
                {
                    promociones.PromocionFija = false;
                }
                _context.Promociones.Add(promociones);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, " Se registro correctamente la Promociones.");
                return RedirectToAction("Index", "Promociones");
            }
            catch(Exception e)
            {
                AddPageAlerts(PageAlertType.Error, " Hubo un error al registrar la Promociones");
                return RedirectToAction("Index", "Promociones");
            }
        }
        public bool Delete(int id)
        {
            try
            {
                List<PromocionesQR> promocionesQR = _context.PromocionesQR.Where(s => s.Promociones.Id == id).ToList();
                if (promocionesQR!=null)
                {
                    foreach (var itemQR in promocionesQR)
                    {                    
                        _context.PromocionesQR.Remove(itemQR);
                    }
                    _context.SaveChanges();
                }
                    
                Promociones promociones = _context.Promociones.Where(s => s.Id == id).First();
                _context.Promociones.Remove(promociones);
                _context.SaveChanges();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
        public ActionResult Update(int id)
        {
            return PartialView(_context.Promociones.Where(s => s.Id == id).First());
        }
        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> Update(Promociones promociones, int colorId, int PromocionFija, string TieneFechaVencimiento)
        {
            Promociones d = _context.Promociones.Where(s => s.Id == promociones.Id).First();
            d.Titulo = promociones.Titulo;
            d.Subtitulo = promociones.Subtitulo == null ? " " : promociones.Subtitulo;
            d.Texto = promociones.Texto==null?" ": promociones.Texto;
            d.TextoBoton = promociones.TextoBoton == null ? " " : promociones.TextoBoton;
            d.Fecha = promociones.Fecha;
            d.Publica = promociones.Publica;
            d.Link = promociones.Link;
            if (TieneFechaVencimiento=="on")
            {
                d.FechaHasta = promociones.FechaHasta;
                d.FechaDesde = promociones.FechaDesde;
                d.Vencimiento = true;
            }
            else
            {

                d.Vencimiento = false;
            }
            if (PromocionFija==1)
            {

                d.PromocionFija = true;
            }
            else
            {
                d.PromocionFija = false;
            }
            _context.SaveChanges();
            return RedirectToAction("Index", "Promociones");
        }
        [HttpGet]
        public async Task<IActionResult> _CambiarImagen(int id)
        {
            var Promocion = await _context.Promociones.FindAsync(id);

            if (Promocion == null) return NotFound();

            return PartialView(Promocion);
        }



        [HttpPost]
        public async Task<IActionResult> _CambiarImagen(IFormFile file, int id)
        {
            var Promocion = await _context.Promociones.FindAsync(id);

            if (Promocion == null) return NotFound();

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                Promocion.Foto = Convert.ToBase64String(memoryStream.ToArray());
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> _EnabledQR(int id)
        {
            try
            {
                var Promocion = await _context.Promociones.FindAsync(id);

                if (Promocion == null) return NotFound();

                Promocion.QR = true;
                _context.Promociones.Update(Promocion);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, "Se Habilitó el QR correctamente.");
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                AddPageAlerts(PageAlertType.Error, "No se pudo Habilitar el QR.");
                return RedirectToAction("Index");
            }
           
        }

        [HttpGet]
        public async Task<IActionResult> _DisabledQR(int id)
        {
            try
            {
                var Promocion = await _context.Promociones.FindAsync(id);

                if (Promocion == null) return NotFound();

                Promocion.QR = false;
                _context.Promociones.Update(Promocion);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, "Se Deshabilitó el QR correctamente.");
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                AddPageAlerts(PageAlertType.Error, "No se pudo Deshabilitar el QR.");
                return RedirectToAction("Index");
            }

        }

        public ActionResult Notificacion(int id)
        {
            try
            {
                var usuario = _context.Usuarios.FirstOrDefault(x => x.Email == User.Identity.Name);
                if(usuario.Clientes!=null && usuario.Clientes.Empresa != null)
                {
                    var ListaPush = _context.Clientes.Where(x => x.Empresa.Id == usuario.Clientes.Empresa.Id);
                    var Promocion = _context.Promociones.Find(id);
                    AddPageAlerts(PageAlertType.Success, ListaPush.Count().ToString() + " Notificaciones Enviadas!");
                    return RedirectToAction("Index", "Promociones");
                }
                AddPageAlerts(PageAlertType.Error, "No se pudieron enviar las notificaciones.");
                return RedirectToAction("Index", "Promociones");
            }
            catch(Exception e)
            {
                AddPageAlerts(PageAlertType.Error, "No se pudieron enviar las notificaciones.");
                return RedirectToAction("Index", "Promociones");
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult ValidarPromocion(string Hash)
        {
            try
            {
                var promocion = _context.PromocionesQR.FirstOrDefault(x => x.Hash == Hash);
                if(promocion != null)
                {
                    if(promocion.Activo == true)
                    {
                        promocion.Activo = false;
                        promocion.FechaUtilizado = DateTime.Now;
                        return View("ValidarPromocion", new { EsActivo = true, Texto = "La promoción se utilizo con éxito." });
                    }
                    else
                    {
                        return View("ValidarPromocion", new { EsActivo = true, Texto = "Esta promocion ya no es válida." });
                    }
                }
                else
                {
                    return View("ValidarPromocion", new { EsActivo = true, Texto = "La promoción no existe" });
                }
               
            }
            catch(Exception e)
            {
                return View("ValidarPromocion", new { EsActivo = true, Texto = "Error al leer la promoción" });
            }
        }

        [HttpPost]
        public IActionResult ModificarOrden(int Id, int Orden)
        {
            try
            {
                Promociones promocion = _context.Promociones.Where(x => x.Id == Id).FirstOrDefault();
                promocion.Orden = Orden;
                _context.Promociones.Update(promocion);
                _context.SaveChanges(); 
                AddPageAlerts(PageAlertType.Success, "Se modificó el orden de la Promoción");
                return RedirectToAction("Index", "Promociones");
            }
            catch (Exception)
            {

                AddPageAlerts(PageAlertType.Error, "No se pudieron modificar la Promoción.");
                return RedirectToAction("Index", "Promociones");
            }
        }
    }    
}