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
    public class MonedasController : EstanciasCoreController
    {
        public MonedasController(EstanciasContext context) : base(context)
        {
            breadcumb.Add(new Message() { DisplayName = "Datos" });
        }

        public IActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Monedas" });
            ViewBag.Breadcrumb = breadcumb;
            return View();
        }

        public async Task<IActionResult> _ListadoMonedas(Page<Monedas> page)
        {
            var c = _context.Monedas.Count();
            if (c < 1) { c = 1; }
            page.SelectPage("/Clientes/_ListadoMonedas",
                _context.Monedas, c);

            return PartialView("_ListadoMonedas", page);
        }

        public IActionResult _Create()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Create(Monedas moneda)
        {
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                try
                {
                    await _context.Monedas.AddAsync(moneda);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se creó correctamente la Moneda " + moneda.Nombre + ".");
                    return RedirectToAction("Index", "Monedas");
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al crear la Moneda. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "Monedas");
                }
            }
            else
            {
                return PartialView(moneda);
            }
        }


        public async Task<IActionResult> _Update(int Id)
        {

            Monedas moneda = await _context.Monedas.FindAsync(Id);
            return PartialView(moneda);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Update(Monedas moneda)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Monedas.Update(moneda);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se editó correctamente la Moneda " + moneda.Nombre + ".");
                    return RedirectToAction("Index", "Monedas");
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al editar la Moneda. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "Monedas");
                }

            }
            else
            {
                return PartialView(moneda);
            }
        }


        public IActionResult Delete(int id)
        {
            try
            {
                Monedas moneda = _context.Monedas.Where(s => s.Id == id).First();
                _context.Monedas.Remove(moneda);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, "Se eliminó correctamente la Monedas.");
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Success, "Hubo un error al eliminar la Monedas.");
                return RedirectToAction("Index", "Monedas");
            }
        }


    }
}