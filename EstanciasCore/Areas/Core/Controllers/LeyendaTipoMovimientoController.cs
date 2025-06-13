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
    public class LeyendaTipoMovimientoController : EstanciasCoreController
    {
        public LeyendaTipoMovimientoController(EstanciasContext context) : base(context)
        {
            breadcumb.Add(new Message() { DisplayName = "Datos" });
        }

        public IActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Leyenda Tipo Movimiento" });
            ViewBag.Breadcrumb = breadcumb;
            return View();
        }

        public async Task<IActionResult> _ListadoLeyendaTipoMovimiento(Page<LeyendaTipoMovimiento> page)
        {
            var c = _context.LeyendaTipoMovimiento.Count();
            if (c < 1) { c = 1; }
            page.SelectPage("/Clientes/_ListadoLeyendaTipoMovimiento",
                _context.LeyendaTipoMovimiento, c);

            return PartialView("_ListadoLeyendaTipoMovimiento", page);
        }

        public IActionResult _Create()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Create(LeyendaTipoMovimiento leyenda)
        {
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                try
                {
                    leyenda.Activo = true;
                    await _context.LeyendaTipoMovimiento.AddAsync(leyenda);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se creó correctamente la Leyenda " + leyenda.TextoLeyenda + ".");
                    return RedirectToAction("Index", "LeyendaTipoMovimiento");
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al crear la Leyenda. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "LeyendaTipoMovimiento");
                }
            }
            else
            {
                return PartialView(leyenda);
            }
        }


        public async Task<IActionResult> _Update(int Id)
        {

            LeyendaTipoMovimiento leyenda = await _context.LeyendaTipoMovimiento.FindAsync(Id);
            return PartialView(leyenda);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Update(LeyendaTipoMovimiento leyenda)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.LeyendaTipoMovimiento.Update(leyenda);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se editó correctamente la leyenda " + leyenda.TextoLeyenda + ".");
                    return RedirectToAction("Index", "LeyendaTipoMovimiento");
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al editar la leyenda. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "LeyendaTipoMovimiento");
                }

            }
            else
            {
                return PartialView(leyenda);
            }
        }


        public IActionResult Delete(int id)
        {
            try
            {
                LeyendaTipoMovimiento leyenda = _context.LeyendaTipoMovimiento.Where(s => s.Id == id).First();
                _context.LeyendaTipoMovimiento.Remove(leyenda);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, "Se eliminó correctamente la leyenda.");
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Success, "Hubo un error al eliminar la leyenda.");
                return RedirectToAction("Index", "LeyendaTipoMovimiento");
            }
        }

        [HttpGet]
        public async Task<IActionResult> _Enabled(int Id)
        {
            LeyendaTipoMovimiento leyenda = _context.LeyendaTipoMovimiento.Where(s => s.Id == Id).First();
            if (ModelState.IsValid)
            {
                try
                {
                    leyenda.Activo = true;
                    _context.LeyendaTipoMovimiento.Update(leyenda);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se habilitó correctamente la leyenda " + leyenda.TextoLeyenda + ".");
                    return RedirectToAction("Index", "LeyendaTipoMovimiento");
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al habilitar la leyenda. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "LeyendaTipoMovimiento");
                }

            }
            else
            {
                return PartialView(leyenda);
            }
        }

        [HttpGet]
        public async Task<IActionResult> _Disabled(int Id)
        {
            LeyendaTipoMovimiento leyenda = _context.LeyendaTipoMovimiento.Where(s => s.Id == Id).First();
            if (ModelState.IsValid)
            {
                try
                {
                    leyenda.Activo = false;
                    _context.LeyendaTipoMovimiento.Update(leyenda);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se deshabilitó correctamente la leyenda " + leyenda.TextoLeyenda + ".");
                    return RedirectToAction("Index", "LeyendaTipoMovimiento");
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al deshabilitar la leyenda. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "LeyendaTipoMovimiento");
                }

            }
            else
            {
                return PartialView(leyenda);
            }
        }
    }
}