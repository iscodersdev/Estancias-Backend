using EstanciasCore.Controllers;
using Commons.Models;
using DAL.Data;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.Areas.Core.Controllers
{
    [Area("Core")]
    public class MarcasController : EstanciasCoreController
    {
        public MarcasController(EstanciasContext context) : base(context)
        {
            breadcumb.Add(new Message() { DisplayName = "Datos" });
        }

        public ActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Marcas" });
            ViewBag.Breadcrumb = breadcumb;

            var marcas = _context.Marcas.Where(x => x.Activo).OrderBy(x => x.Orden).ToList();

            return View(marcas);
        }

        public ActionResult _Create()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> _Create([Bind("Nombre, Orden, NomAliasbre, CBU, WhatsApp")] Marcas nuevaMarca)
        {
            try
            {
                ModelState.Remove("Id");
                if (ModelState.IsValid)
                {
                    nuevaMarca.Activo = true;
                    await _context.Marcas.AddAsync(nuevaMarca);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se cargó correctamente la Marca " + nuevaMarca.Nombre + ".");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return PartialView(nuevaMarca);
                }
            }
            catch (Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al cargar la Marca. Inténtelo nuevamente más tarde.");
                return RedirectToAction(nameof(Index));
            }
        }

        public ActionResult _Update(int id)
        {
            Marcas marca = _context.Marcas.Find(id);
            if (marca != null)
            {
                return PartialView(marca);
            }
            AddPageAlerts(PageAlertType.Error, "Hubo un error al editar la Marca. Inténtelo nuevamente más tarde.");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> _Update(Marcas editarMarca)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var marcaExistente = await _context.Marcas.FindAsync(editarMarca.Id);
                    if (marcaExistente == null)
                    {
                        AddPageAlerts(PageAlertType.Error, "No se encontró la Marca a modificar.");
                        return RedirectToAction(nameof(Index));
                    }

                    marcaExistente.Nombre = editarMarca.Nombre;
                    marcaExistente.Orden = editarMarca.Orden;
                    marcaExistente.Activo = editarMarca.Activo;
                    marcaExistente.NomAliasbre = editarMarca.NomAliasbre;
                    marcaExistente.CBU = editarMarca.CBU;
                    marcaExistente.WhatsApp = editarMarca.WhatsApp;

                    _context.Marcas.Update(marcaExistente);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se modificó correctamente la Marca.");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return PartialView(editarMarca);
                }
            }
            catch (Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al modificar la Marca. Inténtelo nuevamente más tarde.");
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Delete(int id)
        {
            try
            {
                Marcas marca = _context.Marcas.Find(id);
                if (marca != null)
                {
                    marca.Activo = false;
                    _context.Marcas.Update(marca);
                    _context.SaveChanges();
                    AddPageAlerts(PageAlertType.Success, "Se eliminó correctamente la Marca " + marca.Nombre + ".");
                    return RedirectToAction(nameof(Index));
                }
                AddPageAlerts(PageAlertType.Error, "Hubo un error al eliminar la Marca. Inténtelo nuevamente más tarde.");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al eliminar la Marca. Inténtelo nuevamente más tarde.");
                return RedirectToAction(nameof(Index));
            }
        }

        public ActionResult _Image(int id)
        {
            Marcas marca = _context.Marcas.Find(id);
            if (marca != null)
            {
                if (marca.Imagen != null)
                {
                    ViewBag.Foto = Convert.ToBase64String(marca.Imagen);
                }
                return PartialView(marca);
            }
            AddPageAlerts(PageAlertType.Error, "Hubo un error al editar la Imagen de la Marca. Inténtelo nuevamente más tarde.");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<ActionResult> _Image(Marcas model, IFormFile FotoMarca)
        {
            try
            {
                Marcas marcaEdit = await _context.Marcas.FindAsync(model.Id);
                if (marcaEdit == null)
                {
                    AddPageAlerts(PageAlertType.Error, "No se encontró la Marca para cargar la imagen.");
                    return RedirectToAction(nameof(Index));
                }

                if (FotoMarca != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await FotoMarca.CopyToAsync(memoryStream);
                        marcaEdit.Imagen = memoryStream.ToArray();
                    }
                }

                _context.Marcas.Update(marcaEdit);
                await _context.SaveChangesAsync();
                AddPageAlerts(PageAlertType.Success, "Se cargó correctamente la Imagen de la Marca.");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al cargar la Imagen. Inténtelo nuevamente más tarde.");
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public IActionResult GetImagen(int id)
        {
            var marca = _context.Marcas.Find(id);
            if (marca != null && marca.Imagen != null)
            {
                return File(marca.Imagen, "image/jpeg");
            }
            return NotFound();
        }
    }
}
