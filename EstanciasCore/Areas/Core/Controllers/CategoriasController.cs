using Commons.Models;
using DAL.Data;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.Controllers
{
    [Area("Core")]
    public class CategoriasController : EstanciasCoreController
    {
        public CategoriasController(EstanciasContext context) : base(context)
        {
            breadcumb.Add(new Message() { DisplayName = "Datos" });
        }

        public IActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Categorías" });
            ViewBag.Breadcrumb = breadcumb;
            return View();
        }

        public async Task<IActionResult> _ListadoCategorias(Page<Categorias> page)
        {
            var c = _context.Categorias.Count();
            if (c < 1) { c = 1; }
            page.SelectPage("/Categorias/_ListadoCategorias",
                _context.Categorias, c);

            return PartialView("_ListadoCategorias", page);
        }

        public IActionResult _Create()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Create(Categorias categorias)
        {
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                try
                {
                    await _context.Categorias.AddAsync(categorias);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se creó correctamente la Categoría " + categorias.Nombre + ".");
                    return RedirectToAction("Index", "Categorias");
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al crear la Categoría. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "Categorias");
                }
            }
            else
            {
                return PartialView(categorias);
            }
        }


        public async Task<IActionResult> _Update(int Id)
        {

            Categorias categorias = await _context.Categorias.FindAsync(Id);
            return PartialView(categorias);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Update(Categorias categorias)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Categorias.Update(categorias);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se editó correctamente la Categoría " + categorias.Nombre + ".");
                    return RedirectToAction("Index", "Categorias");
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al editar la Categoría. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "Categorias");
                }

            }
            else
            {
                return PartialView(categorias);
            }
        }


        public IActionResult Delete(int id)
        {
            try
            {
                Categorias categorias = _context.Categorias.Where(s => s.Id == id).First();
                _context.Categorias.Remove(categorias);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, "Se eliminó correctamente la Categoría.");
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Success, "Hubo un error al eliminar la Categoría.");
                return RedirectToAction("Index", "Categoría");
            }
        }

    }
}
