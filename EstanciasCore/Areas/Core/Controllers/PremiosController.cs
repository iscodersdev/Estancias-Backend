using Commons.Models;
using DAL.Data;
using DAL.DTOs;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.Controllers
{
    [Area("Core")]
    public class PremiosController : EstanciasCoreController
    {
        public PremiosController(EstanciasContext context) : base(context)
        {
            breadcumb.Add(new Message() { DisplayName = "Datos" });
        }

        public IActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Premios" });
            ViewBag.Breadcrumb = breadcumb;
            return View();
        }

        public async Task<IActionResult> _ListadoPremios(Page<Premios> page)
        {
            var c = _context.Premios.Count();
            if (c < 1) { c = 1; }
            page.SelectPage("/Premios/_ListadoPremios",
                _context.Premios, c);

            return PartialView("_ListadoPremios", page);
        }

        public IActionResult _Create()
        {
            ViewBag.Categorias = _context.Categorias.Select(g => new SelectListItem() { Text = g.Nombre, Value = g.Id.ToString() });
            ViewBag.Marcas = _context.Marcas.Select(g => new SelectListItem() { Text = g.Nombre, Value = g.Id.ToString() });
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Create(Premios premio)
        {
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                try
                {
                    Categorias categoria = _context.Categorias.Where(x=>x.Id==premio.Categoria.Id).FirstOrDefault();
                    premio.Categoria=categoria;
                    Marcas marca = _context.Marcas.Where(x=>x.Id==premio.Marcas.Id).FirstOrDefault();
                    premio.Marcas=marca;
                    premio.StockActual=premio.Stock;
                    premio.Fecha=DateTime.Now;
                    premio.Activo=true;
                    await _context.Premios.AddAsync(premio);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se creó correctamente el Premio " + premio.Nombre + ".");
                    return RedirectToAction("Index", "Premios");
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al crear el Premio. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "Premios");
                }
            }
            else
            {
                return PartialView(premio);
            }
        }


        public async Task<IActionResult> _Update(int Id)
        {

            ViewBag.Categorias = _context.Categorias.Select(g => new SelectListItem() { Text = g.Nombre, Value = g.Id.ToString() });
            ViewBag.Marcas = _context.Marcas.Select(g => new SelectListItem() { Text = g.Nombre, Value = g.Id.ToString() });
            Premios premio = await _context.Premios.FindAsync(Id);
            return PartialView(premio);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Update(Premios premio)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var categoria = _context.Categorias.Where(x => x.Id == premio.Categoria.Id).FirstOrDefault();
                    var marca = _context.Marcas.Where(x => x.Id == premio.Marcas.Id).FirstOrDefault();
                    var premioDB = _context.Premios.Where(x => x.Id == premio.Id).FirstOrDefault();
                    premioDB.Categoria = categoria;
                    premioDB.Marcas = marca;
                    premioDB.Nombre = premio.Nombre;
                    premioDB.Descripcion = premio.Descripcion;
                    premioDB.Stock = premio.Stock;
                    premioDB.Puntos = premio.Puntos;
                    premioDB.TerminosCondiciones = premio.TerminosCondiciones;
                    premioDB.FechaVencimiento = premio.FechaVencimiento;
                    premioDB.DiasDeVencimiento = premio.DiasDeVencimiento;
                    _context.Premios.Update(premioDB);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se editó correctamente el Premio " + premio.Nombre + ".");
                    return RedirectToAction("Index", "Premios");
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al editar el Premio. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "Premios");
                }

            }
            else
            {
                return PartialView(premio);
            }
        }


        public IActionResult Delete(int id)
        {
            try
            {                
                if(_context.HistorialCanje.Any(x => x.Premio.Id == id))
                {
                    AddPageAlerts(PageAlertType.Error, "No se puede canjear un Cupón Canjeado.");
                    return RedirectToAction("Index", "Premios");
                }
                Premios premio = _context.Premios.Where(s => s.Id == id).First();
                List<FotosPremios> fotos = _context.FotosPremios.Where(x => x.Premio.Id == id).ToList();
                _context.FotosPremios.RemoveRange(fotos);
                _context.Premios.Remove(premio);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, "Se eliminó correctamente el Cupón.");
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al eliminar el Cupón.");
                return RedirectToAction("Index", "Premios");
            }
        }

        [HttpGet]
        public async Task<IActionResult> _CambiarImagen(int id)
        {
            var premio = await _context.Premios.FindAsync(id);
            var fotoExistente = _context.FotosPremios.FirstOrDefault(x => x.Premio.Id == id);
            ViewBag.Foto = fotoExistente?.Foto;

            if (premio == null) return NotFound();

            return PartialView(premio);
        }

        [HttpPost]
        public async Task<IActionResult> _CambiarImagen(IFormFile file, int id)
        {
            try
            {
                var premio = await _context.Premios.FindAsync(id);
                if (premio == null)
                {
                    AddPageAlerts(PageAlertType.Error, "El premio no fue encontrado.");
                    return RedirectToAction("Index");
                }

                if (file != null && file.Length > 0)
                {
                    var fotoExistente = _context.FotosPremios.FirstOrDefault(fp => fp.Premio.Id == id);

                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);
                        string base64Foto = Convert.ToBase64String(memoryStream.ToArray());

                        if (fotoExistente != null)
                        {
                            fotoExistente.Foto = base64Foto;
                            fotoExistente.Fecha = DateTime.Now;
                            _context.FotosPremios.Update(fotoExistente);
                        }
                        else
                        {
                            FotosPremios nuevaFoto = new FotosPremios
                            {
                                Premio = premio,
                                Foto = base64Foto,
                                Orden = 1,
                                Fecha = DateTime.Now
                            };
                            _context.FotosPremios.Add(nuevaFoto);
                        }
                        await _context.SaveChangesAsync();
                    }
                }

                AddPageAlerts(PageAlertType.Success, "Se actualizó la imagen correctamente.");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al cargar la imagen: " + ex.Message);
                return RedirectToAction("Index");
            }
        }

        private byte[] ComprimirImagen(IFormFile imagen, long calidad = 50)
        {
            using (var image = Image.FromStream(imagen.OpenReadStream()))
            {
                var qualityEncoder = Encoder.Quality;
                var encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = new EncoderParameter(qualityEncoder, calidad);

                var imageCodecInfo = ImageCodecInfo.GetImageEncoders()
                    .FirstOrDefault(codec => codec.FormatID == ImageFormat.Jpeg.Guid);

                if (imageCodecInfo == null)
                {
                    throw new Exception("Codec no encontrado para la extensión de archivo: " + Path.GetExtension(imagen.FileName));
                }

                using (var memoryStream = new MemoryStream())
                {
                    image.Save(memoryStream, imageCodecInfo, encoderParameters);
                    return memoryStream.ToArray();
                }
            }
        }

    }
}
