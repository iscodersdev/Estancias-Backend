using Commons.Models;
using DAL.Data;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using EstanciasCore.Controllers;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.Areas.Core.Controllers
{
    [Area("Core")]
    public class ConceptosController : EstanciasCoreController
    {
        public ConceptosController(EstanciasContext context) : base(context)
        {
            breadcumb.Add(new Message() { DisplayName = "Datos" });
            breadcumb.Add(new Message() { DisplayName = "Generales" });
        }
        public ActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Conceptos" });
            ViewBag.Breadcrumb = breadcumb;

            var conceptos = _context.Conceptos.ToList();

            return View(conceptos);
        }
        public ActionResult _Create()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<ActionResult> _Create([Bind("Nombre, Signo")] Conceptos concepto)
        {
            try
            {
                ModelState.Remove("Id");
                if (ModelState.IsValid)
                {
                    await _context.Conceptos.AddAsync(concepto);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se cargo correctamente el Concepto " + concepto.Nombre + ".");
                    return RedirectToAction("Index", "Conceptos");
                }
                else
                {
                    return PartialView(concepto);
                }

            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al cargar el Concepto. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index", "Conceptos");
            }
        }
        public IActionResult Delete(int id)
        {
            try
            {
                Conceptos concepto = _context.Conceptos.Where(s => s.Id == id).First();
                _context.Conceptos.Remove(concepto);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, "Se eliminó correctamente el Concepto " + concepto.Nombre + ".");
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Success, "Hubo un error al eliminar el Concepto.");
                return RedirectToAction("Index", "Conceptos");
            }
        }

        public ActionResult _Update(int id)
        {
            return PartialView(_context.Conceptos.Where(s => s.Id == id).First());
        }
        [HttpPost]
        public async Task<ActionResult> _Update(Conceptos concepto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Conceptos.Update(concepto);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se modifico corretamente el Concepto.");
                    return RedirectToAction("Index", "Conceptos");
                }
                else
                {
                    return PartialView(concepto);
                }
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al modificar el Concepto. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index", "Conceptos");
            }
        }
    }
}
