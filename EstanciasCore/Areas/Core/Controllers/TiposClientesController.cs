using EstanciasCore.Controllers;
using Commons.Models;
using DAL.Data;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.Areas.Core.Controllers
{
    [Area("Core")]
    public class TiposClientesController : EstanciasCoreController
    {
        public TiposClientesController(EstanciasContext context) : base(context)
        {
            breadcumb.Add(new Message() { DisplayName = "Datos" });
        }
        public ActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Tipos de Clientes" });
            ViewBag.Breadcrumb = breadcumb;

            var tiposClientes = _context.TiposClientes.ToList();

            return View(tiposClientes);
        }
        public ActionResult _Create()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<ActionResult> _Create([Bind("Nombre, CantidadActividadesSemanales")] TiposClientes tipoCliente)
        {
            try
            {
                ModelState.Remove("Id");
                if (ModelState.IsValid)
                {
                    await _context.TiposClientes.AddAsync(tipoCliente);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se cargo correctamente el Tipo de Cliente " + tipoCliente.Nombre + ".");
                    return RedirectToAction("Index", "TiposClientes");
                }
                else
                {
                    return PartialView(tipoCliente);
                }

            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al cargar el Tipo de Cliente. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index", "TiposClientes");
            }
        }
        public IActionResult Delete(int id)
        {
            try
            {
                TiposClientes tipoCliente = _context.TiposClientes.Where(s => s.Id == id).First();
                _context.TiposClientes.Remove(tipoCliente);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, "Se eliminó correctamente el Tipo de Cliente " + tipoCliente.Nombre + ".");
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Success, "Hubo un error al eliminar el Tipo de Cliente.");
                return RedirectToAction("Index", "TiposClientes");
            }
        }

        public ActionResult _Update(int id)
        {
            return PartialView(_context.TiposClientes.Where(s => s.Id == id).First());
        }
        [HttpPost]
        public async Task<ActionResult> _Update(TiposClientes tipoCliente)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.TiposClientes.Update(tipoCliente);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se modifico corretamente el Tipo de Cliente.");
                    return RedirectToAction("Index", "TiposClientes");
                }
                else
                {
                    return PartialView(tipoCliente);
                }
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al modificar el Tipo de Cliente. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index", "TiposClientes");
            }
        }
    }
}
