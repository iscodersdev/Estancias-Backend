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
                    _context.Premios.Update(premio);
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
                Premios premio = _context.Premios.Where(s => s.Id == id).First();
                _context.Premios.Remove(premio);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, "Se eliminó correctamente el Premio.");
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Success, "Hubo un error al eliminar el Premio.");
                return RedirectToAction("Index", "Premios");
            }
        }

        [HttpGet]
        public async Task<IActionResult> _CambiarImagen(int id)
        {
            var premio = await _context.Premios.FindAsync(id);
            PremiosImagenDTO premiosImagenDTO = new PremiosImagenDTO();
            List<FotosPremios> fotos = _context.FotosPremios.Where(x=>x.Premio.Id==id).ToList();

            premiosImagenDTO.Id = id;
            premiosImagenDTO.Imagenes = fotos.Select(x => x.Foto).ToList();

            if (premio == null) return NotFound();

            return PartialView(premiosImagenDTO);
        }


        [HttpPost]
        public async Task<IActionResult> _CambiarImagen(int Id, List<IFormFile> Imagenes)
        {
            try
            {
                var premio = await _context.Premios.FindAsync(Id);
                if (premio == null)
                {
                    AddPageAlerts(PageAlertType.Error, "El premio no fue encontrado.");
                    return RedirectToAction("Index");
                }

                // Verificar si se subieron imágenes
                if (Imagenes == null || !Imagenes.Any())
                {
                    AddPageAlerts(PageAlertType.Warning, "No se seleccionaron imágenes para subir.");
                    return RedirectToAction("Index");
                }

                // Eliminar fotos anteriores asociadas al premio (si es necesario)
                var fotosExistentes = _context.FotosPremios.Where(fp => fp.Premio.Id == Id).ToList();
                _context.FotosPremios.RemoveRange(fotosExistentes);

                int cont = 0;
                foreach (var file in Imagenes)
                {
                    if (file.Length > 0)
                    {
                        cont++;
                        FotosPremios fotosPremio = new FotosPremios();
                        fotosPremio.Premio.Id = Id; // Asignar directamente el ID del premio
                        fotosPremio.Orden = cont;
                        fotosPremio.Fecha = DateTime.Now;
                        fotosPremio.Foto = Convert.ToBase64String(ComprimirImagen(file));   
                        _context.FotosPremios.Add(fotosPremio);
                    }
                }

                await _context.SaveChangesAsync();

                AddPageAlerts(PageAlertType.Success, "Se cargaron las imágenes correctamente.");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al cargar las imágenes: " + ex.Message);
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
