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
    public class ProvinciasController : EstanciasCoreController
    {
        public ProvinciasController(EstanciasContext context) : base(context)
        {
            breadcumb.Add(new Message() { DisplayName = "Datos" });
        }

        public IActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Provincias" });
            ViewBag.Breadcrumb = breadcumb;
            return View();
        }

        public async Task<IActionResult> _ListadoProvincias(Page<Provincia> page)
        {
            var c = _context.Provincia.Count();
            if (c < 1) { c = 1; }
            page.SelectPage("/Clientes/_ListadoProvincias",
                _context.Provincia, c);

            return PartialView("_ListadoProvincias", page);
        }

        public IActionResult _Create()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Create(Provincia provincia)
        {
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                try
                {
                    await _context.Provincia.AddAsync(provincia);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se creó correctamente la Provincia " + provincia.Descripcion + ".");
                    return RedirectToAction("Index", "Provincias");
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al crear la Provincia. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "Provincias");
                }
            }
            else
            {
                return PartialView(provincia);
            }
        }


        public async Task<IActionResult> _Update(int Id)
        {

            Provincia provincia = await _context.Provincia.FindAsync(Id);
            return PartialView(provincia);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Update(Provincia provincia)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Provincia.Update(provincia);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se editó correctamente la Provincia " + provincia.Descripcion + ".");
                    return RedirectToAction("Index", "Provincias");
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al editar la Provincia. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "Provincias");
                }

            }
            else
            {
                return PartialView(provincia);
            }
        }


        public IActionResult Delete(int id)
        {
            try
            {
                Provincia provincia = _context.Provincia.Where(s => s.Id == id).First();
                _context.Provincia.Remove(provincia);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, "Se eliminó correctamente la Provincia.");
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Success, "Hubo un error al eliminar la Provincia.");
                return RedirectToAction("Index", "Provincias");
            }
        }

    }
}
