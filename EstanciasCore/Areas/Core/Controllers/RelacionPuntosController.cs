using Commons.Models;
using DAL.Data;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.Controllers
{
    [Area("Core")]
    public class RelacionPuntosController : EstanciasCoreController
    {
        public RelacionPuntosController(EstanciasContext context) : base(context)
        {
            breadcumb.Add(new Message() { DisplayName = "Datos" });
        }

        public IActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Relación Puntos" });
            ViewBag.Breadcrumb = breadcumb;
            return View();
        }

        public async Task<IActionResult> _ListadoRelacionPuntos(Page<RelacionPuntos> page)
        {
            var c = _context.RelacionPuntos.Count();
            if (c < 1) { c = 1; }
            page.SelectPage("/RelacionPuntos/_ListadoRelacionPuntos",
                _context.RelacionPuntos, c);

            return PartialView("_ListadoRelacionPuntos", page);
        }

        public IActionResult _Create()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Create(RelacionPuntos relacion)
        {
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                try
                {
                    await _context.RelacionPuntos.AddAsync(relacion);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se creó correctamente el Registro.");
                    return RedirectToAction("Index", "RelacionPuntos");
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al crear el registro. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "RelacionPuntos");
                }
            }
            else
            {
                return PartialView(relacion);
            }
        }


        public async Task<IActionResult> _Update(int Id)
        {

            RelacionPuntos relacion = await _context.RelacionPuntos.FindAsync(Id);
            return PartialView(relacion);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Update(RelacionPuntos relacion)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.RelacionPuntos.Update(relacion);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se editó correctamente el Registro.");
                    return RedirectToAction("Index", "RelacionPuntos");
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al editar el Registro. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "RelacionPuntos");
                }

            }
            else
            {
                return PartialView(relacion);
            }
        }


        public IActionResult Delete(int id)
        {
            try
            {
                RelacionPuntos relacion = _context.RelacionPuntos.Where(s => s.Id == id).First();
                _context.RelacionPuntos.Remove(relacion);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, "Se eliminó correctamente el Registro.");
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Success, "Hubo un error al eliminar el Registro.");
                return RedirectToAction("Index", "RelacionPuntos");
            }
        }

        public ActionResult _Image(int id)
        {
            RelacionPuntos relacion = _context.RelacionPuntos.Find(id);
            if (relacion != null)
            {
                if (relacion.Imagen != null)
                {
                    ViewBag.Foto = Convert.ToBase64String(relacion.Imagen);
                }
                return PartialView(relacion);
            }
            AddPageAlerts(PageAlertType.Error, "Hubo un error al editar la Imagen. Inténtelo nuevamente más tarde.");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<ActionResult> _Image(RelacionPuntos model, IFormFile FotoRelacion)
        {
            try
            {
                RelacionPuntos relacionEdit = await _context.RelacionPuntos.FindAsync(model.Id);
                if (relacionEdit == null)
                {
                    AddPageAlerts(PageAlertType.Error, "No se encontró el registro para cargar la imagen.");
                    return RedirectToAction(nameof(Index));
                }

                if (FotoRelacion != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await FotoRelacion.CopyToAsync(memoryStream);
                        relacionEdit.Imagen = memoryStream.ToArray();
                    }
                }

                _context.RelacionPuntos.Update(relacionEdit);
                await _context.SaveChangesAsync();
                AddPageAlerts(PageAlertType.Success, "Se cargó correctamente la Imagen.");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al cargar la Imagen. Inténtelo nuevamente más tarde.");
                return RedirectToAction(nameof(Index));
            }
        }

    }
}