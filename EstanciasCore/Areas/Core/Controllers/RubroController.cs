using EstanciasCore.Controllers;
using Commons.Models;
using DAL.Data;
using DAL.Models.Core;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.Areas.Core.Controllers
{
    [Area("Core")]
    public class RubroController : EstanciasCoreController
    {
        public RubroController(EstanciasContext context) : base(context)
        {
            breadcumb.Add(new Message() { DisplayName = "Datos" });
        }
        public ActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Rubros" });
            ViewBag.Breadcrumb = breadcumb;

            var rubros = _context.Rubros.Where(x=>x.Activo).ToList();

            return View(rubros);
        }
        public ActionResult _Create()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> _Create([Bind("Nombre, Descripcion")] Rubro nuevoRubro)
        {
            try
            {
                ModelState.Remove("Id");
                if (ModelState.IsValid)
                {
                    nuevoRubro.Activo = true;
                    await _context.Rubros.AddAsync(nuevoRubro);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se cargo correctamente el Rubro " + nuevoRubro.Nombre + ".");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return PartialView(nuevoRubro);
                }

            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al cargar el Rubro. Intentelo nuevamente mas tarde.");
                return RedirectToAction(nameof(Index));
            }
        }
        public IActionResult Delete(int id)
        {
            try
            {
                Rubro rubro = _context.Rubros.Find(id);
                if (rubro != null)
                {
                    rubro.Activo = false;
                    _context.Rubros.Update(rubro);
                    _context.SaveChanges();
                    AddPageAlerts(PageAlertType.Success, "Se eliminó correctamente el Rubro " + rubro.Nombre + ".");
                    return RedirectToAction(nameof(Index));
                }
                AddPageAlerts(PageAlertType.Error, "Hubo un error al eliminar el Rubro. Intentelo nuevamente mas tarde.");
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al eliminar el Rubro. Intentelo nuevamente mas tarde.");
                return RedirectToAction(nameof(Index));
            }
        }

        public ActionResult _Update(int id)
        {
            Rubro rubro = _context.Rubros.Find(id);
            if (rubro != null)
            {
                return PartialView(rubro);
            }
            AddPageAlerts(PageAlertType.Error, "Hubo un error al editar el Rubro. Intentelo nuevamente mas tarde.");
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> _Update(Rubro editarRubro)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Rubros.Update(editarRubro);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se modifico corretamente el Rubro.");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return PartialView(editarRubro);
                }
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al modificar el Rubro. Intentelo nuevamente mas tarde.");
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
