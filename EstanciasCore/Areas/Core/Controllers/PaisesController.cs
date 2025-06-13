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
    public class PaisesController : EstanciasCoreController
    {
        public PaisesController(EstanciasContext context) : base(context)
        {
            breadcumb.Add(new Message() { DisplayName = "Datos" });
        }

        public IActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Paises" });
            ViewBag.Breadcrumb = breadcumb;
            return View();
        }

        public async Task<IActionResult> _ListadoPaises(Page<Paises> page)
        {
            var c = _context.Paises.Count();
            if (c < 1) { c = 1; }
            page.SelectPage("/Clientes/_ListadoPaises",
                _context.Paises, c);

            return PartialView("_ListadoPaises", page);
        }

        public IActionResult _Create()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Create(Paises paises)
        {
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                try
                {
                    await _context.Paises.AddAsync(paises);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se creó correctamente el País " + paises.Nombre + ".");
                    return RedirectToAction("Index", "Paises");
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al crear el País. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "Paises");
                }
            }
            else
            {
                return PartialView(paises);
            }
        }


        public async Task<IActionResult> _Update(int Id)
        {

            Paises paises = await _context.Paises.FindAsync(Id);
            return PartialView(paises);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Update(Paises paises)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Paises.Update(paises);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se editó correctamente el País " + paises.Nombre + ".");
                    return RedirectToAction("Index", "Paises");
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al editar el País. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "Paises");
                }

            }
            else
            {
                return PartialView(paises);
            }
        }


        public IActionResult Delete(int id)
        {
            try
            {
                Paises paises = _context.Paises.Where(s => s.Id == id).First();
                _context.Paises.Remove(paises);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, "Se eliminó correctamente el País.");
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Success, "Hubo un error al eliminar el País.");
                return RedirectToAction("Index", "Paises");
            }
        }

    }
}
