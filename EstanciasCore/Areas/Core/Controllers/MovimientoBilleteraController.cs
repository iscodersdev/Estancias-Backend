using Commons.Models;
using DAL.Data;
using DAL.Models.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.Controllers
{
    [Area("Core")]
    public class MovimientoBilleteraController : EstanciasCoreController
    {
        public MovimientoBilleteraController(EstanciasContext context) : base(context)
        {
            _context = context;
            breadcumb.Add(new Message() { DisplayName = "Datos" });
        }
        public ActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Movimiento de Billetera" });
            ViewBag.Breadcrumb = breadcumb;
            var movimientoBilletera = _context.MovimientosBilletera.ToList();
            return View(movimientoBilletera);
        }

        public ActionResult _Create()
        {
            MovimientoBilleteraViewBag();
            return PartialView();
        }

        [HttpPost]
        public async Task<ActionResult> _Create([Bind("Id,TipoMovimiento,Monto,CBU")] MovimientoBilletera movimientoBilletera)
        {
            try
            {
                ModelState.Remove("Id");
                if (ModelState.IsValid)
                {
                    movimientoBilletera.TipoMovimiento = _context.TipoMovimientoBilletera.Find(movimientoBilletera.TipoMovimiento.Id);
                    await _context.MovimientosBilletera.AddAsync(movimientoBilletera);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se cargo correctamente el Movimiento de la Billetera.");
                    return RedirectToAction("Index", "MovimientoBilletera");
                }
                else
                {
                    MovimientoBilleteraViewBag();
                    return PartialView(movimientoBilletera);
                }

            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al cargar el Movimiento de la Billetera. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index", "MovimientoBilletera");
            }
        }
        public IActionResult Delete(int id)
        {
            try
            {
                MovimientoBilletera movimientoBilletera = _context.MovimientosBilletera.Where(s => s.Id == id).First();
                _context.MovimientosBilletera.Remove(movimientoBilletera);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, "Se eliminó correctamente el Movimiento de la Billetera.");
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Success, "Hubo un error al eliminar el Movimiento de la Billetera.");
                return RedirectToAction("Index", "MovimientoBilletera");
            }
        }

        public ActionResult _Update(int id)
        {
            MovimientoBilleteraViewBag();
            return PartialView(_context.MovimientosBilletera.Where(s => s.Id == id).First());
        }
        [HttpPost]
        public async Task<ActionResult> _Update(MovimientoBilletera movimientoBilletera)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    movimientoBilletera.TipoMovimiento = _context.TipoMovimientoBilletera.Find(movimientoBilletera.TipoMovimiento.Id);
                    _context.MovimientosBilletera.Update(movimientoBilletera);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se modifico corretamente el Movimiento de la Billetera.");
                    return RedirectToAction("Index", "MovimientoBilletera");
                }
                else
                {
                    MovimientoBilleteraViewBag();
                    return PartialView(movimientoBilletera);
                }
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al modificar el Movimiento de la Billetera. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index", "MovimientoBilletera");
            }
        }

        private void MovimientoBilleteraViewBag()
        {
            ViewBag.TipoMovimientoBilletera = _context.TipoMovimientoBilletera.Select(g => new SelectListItem() { Text = g.Nombre, Value = g.Id.ToString() });
        }
    }
}
