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
    public class SucursalesController : EstanciasCoreController
    {
        public SucursalesController(EstanciasContext context) : base(context)
        {
            breadcumb.Add(new Message() { DisplayName = "Datos" });
        }

        public IActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Sucursales" });
            ViewBag.Breadcrumb = breadcumb;
            return View();
        }

        public async Task<IActionResult> _ListadoSucursales(Page<Sucursales> page)
        {
            var c = _context.Sucursales.Count();
            if (c < 1) { c = 1; }
            page.SelectPage("/Sucursales/_ListadoSucursales",
                _context.Sucursales, c);

            return PartialView("_ListadoSucursales", page);
        }

        public IActionResult _Create()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Create(Sucursales sucursales)
        {
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                try
                {
                    await _context.Sucursales.AddAsync(sucursales);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se creó correctamente la Sucursal " + sucursales.name + ".");
                    return RedirectToAction("Index", "Sucursales");
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al crear la Sucursal. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "Sucursales");
                }
            }
            else
            {
                return PartialView(sucursales);
            }
        }


        public async Task<IActionResult> _Update(int Id)
        {

            Sucursales sucursal = await _context.Sucursales.FindAsync(Id);
            return PartialView(sucursal);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Update(Sucursales sucursal)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Sucursales.Update(sucursal);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se editó correctamente la Sucursal " + sucursal.name + ".");
                    return RedirectToAction("Index", "Sucursales");
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al editar la Sucursal. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "Sucursales");
                }

            }
            else
            {
                return PartialView(sucursal);
            }
        }


        public IActionResult Delete(int id)
        {
            try
            {
                Sucursales sucursal = _context.Sucursales.Where(s => s.Id == id).First();
                _context.Sucursales.Remove(sucursal);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, "Se eliminó correctamente la Sucursal.");
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Success, "Hubo un error al eliminar la Sucursal.");
                return RedirectToAction("Index", "Sucursales");
            }
        }


    }
}