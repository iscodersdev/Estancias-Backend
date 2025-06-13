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
    public class TipoMovimientoBilleteraController : EstanciasCoreController
    {
        public TipoMovimientoBilleteraController(EstanciasContext context) : base(context)
        {
            _context = context;
            breadcumb.Add(new Message() { DisplayName = "Datos" });
        }
        public ActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Tipo Movimiento de Billetera" });
            ViewBag.Breadcrumb = breadcumb;
            var tipoMovimientoBilletera = _context.TipoMovimientoBilletera.ToList();
            return View(tipoMovimientoBilletera);
        }

        public ActionResult _Create()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<ActionResult> _Create([Bind("Id,Nombre,Credito,Debito")] TipoMovimientoBilletera tipoMovimientoBilletera, string Credito, string Debito, int optionsRadios)
        {
            try
            {
                ModelState.Remove("Id");
                ModelState.Remove("Credito");
                ModelState.Remove("Debito");
                if (ModelState.IsValid)
                {
                    if (optionsRadios == 0)
                    {
                        tipoMovimientoBilletera.Credito = true;
                    }
                    else
                    {
                        tipoMovimientoBilletera.Debito = true;
                    }
                    await _context.TipoMovimientoBilletera.AddAsync(tipoMovimientoBilletera);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se cargo correctamente el Tipo de Movimiento de Billetera" + tipoMovimientoBilletera.Nombre + ".");
                    return RedirectToAction("Index", "TipoMovimientoBilletera");
                }
                else
                {
                    return PartialView(tipoMovimientoBilletera);
                }

            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al cargar el Tipo de Movimiento de Billetera. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index", "TipoMovimientoBilletera");
            }
        }
        public IActionResult Delete(int id)
        {
            try
            {
                TipoMovimientoBilletera tipoMovimientoBilletera = _context.TipoMovimientoBilletera.Where(s => s.Id == id).First();
                _context.TipoMovimientoBilletera.Remove(tipoMovimientoBilletera);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, "Se eliminó correctamente el Tipo de Movimiento de Billetera " + tipoMovimientoBilletera.Nombre + ".");
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Success, "Hubo un error al eliminar el Tipo de Movimiento de Billetera.");
                return RedirectToAction("Index", "TipoMovimientoBilletera");
            }
        }

        public ActionResult _Update(int id)
        {
            return PartialView(_context.TipoMovimientoBilletera.Where(s => s.Id == id).First());
        }
        [HttpPost]
        public async Task<ActionResult> _Update(TipoMovimientoBilletera tipoMovimientoBilletera, string Credito, string Debito, int optionsRadios)
        {
            try
            {
                ModelState.Remove("Credito");
                ModelState.Remove("Debito");
                if (ModelState.IsValid)
                {
                    if (optionsRadios == 0)
                    {
                        tipoMovimientoBilletera.Credito = true;
                        tipoMovimientoBilletera.Debito = false;
                    }
                    else
                    {
                        tipoMovimientoBilletera.Credito = false;
                        tipoMovimientoBilletera.Debito = true;
                    }
                    _context.TipoMovimientoBilletera.Update(tipoMovimientoBilletera);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se modifico corretamente el Tipo de Movimiento de Billetera.");
                    return RedirectToAction("Index", "TipoMovimientoBilletera");
                }
                else
                {
                    return PartialView(tipoMovimientoBilletera);
                }
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al modificar el Tipo de Movimiento de Billetera. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index", "TipoMovimientoBilletera");
            }
        }
    }
}
