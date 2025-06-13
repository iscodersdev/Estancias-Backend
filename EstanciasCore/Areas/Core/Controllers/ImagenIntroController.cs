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

namespace EstanciasCore.Controllers
{
    [Area("Core")]
    public class ImagenIntroController : EstanciasCoreController
    {
        public ImagenIntroController(EstanciasContext context) : base(context)
        {
            breadcumb.Add(new Message() { DisplayName = "Imagen Intro" });
        }
        public ActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Imagen Intro" });
            return View();
        }
        public ActionResult Create()
        {
            return PartialView();
        }
        public IActionResult ObtenerImagenIntro(Page<ImagenIntro> page)
        {

            var usuario = _context.Usuarios.FirstOrDefault(x => x.Email == User.Identity.Name);
            if(usuario== null || usuario.Clientes?.Empresa == null)
            {
                page.SelectPage("/ImagenIntro/ObtenerImagenIntro", _context.ImagenIntro, x => x.Empresa == null && (x.Titulo.Contains(page.SearchText)));
            }
            else
            {
                page.SelectPage("/ImagenIntro/ObtenerImagenIntro",
                    _context.ImagenIntro,
                    x => x.Empresa.Id == usuario.Clientes.Empresa.Id && (x.Titulo.Contains(page.SearchText))
                    );
            }
            return PartialView("_ListadoImagenIntro", page);
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> Create(ImagenIntro imagenIntro)
        {
            try
            {
                ImagenIntro modificarAnterior = _context.ImagenIntro.Where(s => s.Orden == imagenIntro.Orden).FirstOrDefault();
                if (modificarAnterior!=null)
                {
                    modificarAnterior.Orden = 0;
                    _context.ImagenIntro.Update(modificarAnterior);
                    _context.SaveChanges();
                }
                var usuario = _context.Usuarios.FirstOrDefault(x => x.Email == User.Identity.Name);
                imagenIntro.Empresa = usuario.Clientes?.Empresa;               
                _context.ImagenIntro.Add(imagenIntro);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, " Se registro correctamente la Imagen Intro.");
                return RedirectToAction("Index", "ImagenIntro");
            }
            catch(Exception e)
            {
                AddPageAlerts(PageAlertType.Error, " Hubo un error al registrar Imagen Intro");
                return RedirectToAction("Index", "ImagenIntro");
            }
        }
        public bool Delete(int id)
        {
            try
            {
                ImagenIntro imagenIntro = _context.ImagenIntro.Where(s => s.Id == id).First();
                _context.ImagenIntro.Remove(imagenIntro);
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
            return PartialView(_context.ImagenIntro.Where(s => s.Id == id).First());
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> Update(ImagenIntro imagenIntro)
        {
            ImagenIntro modificarAnterior = _context.ImagenIntro.Where(s => s.Orden == imagenIntro.Orden).FirstOrDefault();
            if (modificarAnterior!=null)
            {
                modificarAnterior.Orden = 0;
                _context.ImagenIntro.Update(modificarAnterior);
                _context.SaveChanges();
            }
            ImagenIntro d = _context.ImagenIntro.Where(s => s.Id == imagenIntro.Id).First();
            d.Titulo = imagenIntro.Titulo;
            d.Orden = imagenIntro.Orden;
            d.Fecha = imagenIntro.Fecha;           
            _context.SaveChanges();
            return RedirectToAction("Index", "ImagenIntro");
        }
        [HttpGet]
        public async Task<IActionResult> _CambiarImagen(int id)
        {
            var imagenIntro = await _context.ImagenIntro.FindAsync(id);

            if (imagenIntro == null) return NotFound();

            return PartialView(imagenIntro);
        }

        [HttpPost]
        public async Task<IActionResult> _CambiarImagen(IFormFile file, int id)
        {
            var imagenIntro = await _context.ImagenIntro.FindAsync(id);

            if (imagenIntro == null) return NotFound();
            imagenIntro.EsVideo = false;
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                imagenIntro.Foto = Convert.ToBase64String(memoryStream.ToArray());
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> _CambiarVideo(int id)
        {
            var imagenIntro = await _context.ImagenIntro.FindAsync(id);

            if (imagenIntro == null) return NotFound();

            return PartialView(imagenIntro);
        }




        [HttpPost]
        public IActionResult _CambiarVideo(ImagenIntro model, IFormFile file)
        {
            try
            {
                var imagenIntro = _context.ImagenIntro.Find(model.Id);
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

                    imagenIntro.Foto =  Url.Content(urlBase + "/uploads/" + uniqueFileName);
                    imagenIntro.EsVideo = true;
                    _context.Update(imagenIntro);
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