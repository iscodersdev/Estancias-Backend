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
using Microsoft.AspNetCore.Hosting;

namespace EstanciasCore.Controllers
{
    [Area("Core")]
    public class BannersController : EstanciasCoreController
    {
        private IHostingEnvironment _env;
        public BannersController(EstanciasContext context, IHostingEnvironment env) : base(context)
        {
            _env = env; 
            breadcumb.Add(new Message() { DisplayName = "Banners" });
        }
        public ActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Banners" });
            return View();
        }
        public ActionResult Create()
        {
            return PartialView();
        }
        public IActionResult ObtenerBanners(Page<Banners> page)
        {

            var usuario = _context.Usuarios.FirstOrDefault(x => x.Email == User.Identity.Name);
            if(usuario== null || usuario.Clientes?.Empresa == null)
            {
                page.SelectPage("/Banners/ObtenerBanners", _context.Banners, x => x.Empresa == null && (x.Titulo.Contains(page.SearchText) || x.Texto.Contains(page.SearchText)));
            }
            else
            {
                page.SelectPage("/Banners/ObtenerBanners",
                    _context.Banners,
                    x => x.Empresa.Id == usuario.Clientes.Empresa.Id && (x.Titulo.Contains(page.SearchText) || x.Texto.Contains(page.SearchText))
                    );
            }
            return PartialView("_ListadoBanners", page);
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> Create(Banners banner, int colorId, int BannersFijo, string TieneFechaVencimiento)
        {
            try
            {
                var usuario = _context.Usuarios.FirstOrDefault(x => x.Email == User.Identity.Name);
				banner.Empresa = usuario.Clientes?.Empresa;
                banner.Vencimiento = false;
                if (BannersFijo==1)
                {
					banner.BannerFijo = true;
                }
                else
                {
					banner.BannerFijo = false;
                }
                if (TieneFechaVencimiento=="on")
                {
                    banner.Vencimiento = true;
                }
                banner.EsVideo=false;
                _context.Banners.Add(banner);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, " Se registro correctamente el Banner.");
                return RedirectToAction("Index", "Banners");
            }
            catch(Exception e)
            {
                AddPageAlerts(PageAlertType.Error, " Hubo un error al registrar el Banner");
                return RedirectToAction("Index", "Banners");
            }
        }
        public bool Delete(int id)
        {
            try
            {
				Banners banner = _context.Banners.Where(s => s.Id == id).First();
                _context.Banners.Remove(banner);
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
            return PartialView(_context.Banners.Where(s => s.Id == id).First());
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> Update(Banners banner, int colorId, int BannerFijo, string TieneFechaVencimiento)
        {
			Banners d = _context.Banners.Where(s => s.Id == banner.Id).First();
            d.Titulo = banner.Titulo;
            d.Subtitulo = banner.Subtitulo == null ? " " : banner.Subtitulo;
            d.Texto = banner.Texto==null?" ": banner.Texto;
            d.TextoBoton = banner.TextoBoton == null ? " " : banner.TextoBoton;
            d.Fecha = banner.Fecha;
            d.Publico = banner.Publico;
            d.Link = banner.Link;
            d.FechaDesde = banner.FechaDesde;
            if (BannerFijo==1)
            {

                d.BannerFijo = true;
            }
            else
            {
                d.BannerFijo = false;
            }

            if (TieneFechaVencimiento=="on")
            {
                d.FechaHasta = banner.FechaHasta;
                d.Vencimiento = true;
            }
            else
            {
                d.FechaHasta = null;
                d.Vencimiento = false;
            }
            _context.SaveChanges();
            return RedirectToAction("Index", "Banners");
        }
        [HttpGet]
        public async Task<IActionResult> _CambiarImagen(int id)
        {
            var banner = await _context.Banners.FindAsync(id);

            if (banner == null) return NotFound();

            return PartialView(banner);
        }

        [HttpPost]
        public async Task<IActionResult> _CambiarImagen(IFormFile file, int id)
        {
            var banner = await _context.Banners.FindAsync(id);
            if (banner.EsVideo && banner.Video!=null)
            {
                Uri uri = new Uri(banner.Video);
                string nombreArchivo = Path.GetFileName(uri.LocalPath);
                try
                {
                    string uploadsPath = Path.Combine(_env.WebRootPath, "uploads");
                    string rutaArchivo = Path.Combine(uploadsPath, nombreArchivo);
                    System.IO.File.Delete(rutaArchivo);
                }
                catch (Exception ex)
                {
                }
            }


            if (banner == null) return NotFound();

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                banner.EsVideo = false;
                banner.Video = null;
                banner.Foto = Convert.ToBase64String(memoryStream.ToArray());
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        public ActionResult Notificacion(int id)
        {
            try
            {
                var usuario = _context.Usuarios.FirstOrDefault(x => x.Email == User.Identity.Name);
                if(usuario.Clientes!=null && usuario.Clientes.Empresa != null)
                {
                    var ListaPush = _context.Clientes.Where(x => x.Empresa.Id == usuario.Clientes.Empresa.Id);
                    var Promocion = _context.Banners.Find(id);
                    AddPageAlerts(PageAlertType.Success, ListaPush.Count().ToString() + " Notificaciones Enviadas!");
                    return RedirectToAction("Index", "Banners");
                }
                AddPageAlerts(PageAlertType.Error, "No se pudieron enviar las notificaciones.");
                return RedirectToAction("Index", "Banners");
            }
            catch(Exception e)
            {
                AddPageAlerts(PageAlertType.Error, "No se pudieron enviar las notificaciones.");
                return RedirectToAction("Index", "Banners");
            }
        }

        [HttpPost]
        public IActionResult ModificarOrden(int Id, int Orden)
        {
            try
            {
                Banners banner = _context.Banners.Where(x => x.Id == Id).FirstOrDefault();
                banner.Orden = Orden;
                _context.Banners.Update(banner);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, "Se modificó el orden del Banner");
                return RedirectToAction("Index", "Banners");
            }
            catch (Exception)
            {

                AddPageAlerts(PageAlertType.Error, "No se pudieron modificar el Banner.");
                return RedirectToAction("Index", "Banners");
            }
        }


        [HttpGet]
        public async Task<IActionResult> _CambiarVideo(int id)
        {
            var banner = await _context.Banners.FindAsync(id);

            if (banner == null) return NotFound();

            return PartialView(banner);
        }

        [HttpPost]
        public IActionResult _CambiarVideo(Banners model, IFormFile file)
        {
            try
            {
                var banner = _context.Banners.Find(model.Id);
                if (banner.EsVideo && banner.Video!=null)
                {
                    Uri uri = new Uri(banner.Video);
                    string nombreArchivo = Path.GetFileName(uri.LocalPath);
                    try
                    {
                        string uploadsPath = Path.Combine(_env.WebRootPath, "uploads");
                        string rutaArchivo = Path.Combine(uploadsPath, nombreArchivo);
                        System.IO.File.Delete(rutaArchivo);
                    }
                    catch (Exception ex)
                    {
                    }
                }

                if (file != null && file.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    Directory.CreateDirectory(uploadsFolder);
                    string cadenaSinEspacios = file.FileName.Replace(" ", "_");
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + cadenaSinEspacios;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    var urlBase = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

                    banner.Video =  Url.Content(urlBase + "/uploads/" + uniqueFileName);
                    banner.Foto = null;
                    banner.EsVideo = true;
                    _context.Update(banner);
                    _context.SaveChanges();


                }
                AddPageAlerts(PageAlertType.Success, " Se cargo correctamente el video.");
                return RedirectToAction("Index");
            }
            catch (Exception)
            {

                AddPageAlerts(PageAlertType.Error, " Hubo un error al cargar el video");
                return RedirectToAction("Index");
            }
        }
    }    
}