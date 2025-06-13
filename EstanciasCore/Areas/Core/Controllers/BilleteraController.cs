using Commons.Models;
using DAL.Data;
using DAL.Models.Core;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.Controllers
{
    [Area("Core")]
    public class BilleteraController : EstanciasCoreController
    {
        public BilleteraController(EstanciasContext context) : base(context)
        {
            _context = context;
            breadcumb.Add(new Message() { DisplayName = "Datos" });
        }
        public ActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Billetera" });
            ViewBag.Breadcrumb = breadcumb;
            var billetera = _context.Billeteras.ToList();
            return View(billetera);
        }

        public ActionResult _Create()
        {
            return PartialView();
        }

        public JsonResult BilleteraComboJson(string q)
        {
            var items = _context.Clientes
                .Where(x => x.Persona.Nombres.Contains(q) || x.Persona.Apellido.Contains(q) || x.Persona.NroDocumento.Contains(q))
                .Select(x => new
                {
                    Text = $"{x.Persona.Apellido}, {x.Persona.Nombres}",
                    Value = x.Id,
                    Subtext = $"{x.Usuario.Mail}",
                    Icon = "fa fa-user"
                }).Take(10).ToArray();

            return Json(items);
        }

        [HttpPost]
        public async Task<ActionResult> _Create([Bind("Saldo, AliasCVU, CVU")] Billetera billetera, string Cliente)
        {
            try
            {
                if (Cliente == null)
                {
                    ModelState.AddModelError("Cliente", "Debe seleccionar un Cliente");
                }
                ModelState.Remove("Id");
                if (ModelState.IsValid)
                {
                    billetera.Cliente = _context.Clientes.Find(Convert.ToInt32(Cliente));
                    await _context.Billeteras.AddAsync(billetera);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se cargo correctamente la billetera del Usuario" + billetera.Cliente.Usuario.UserName  + ".");
                    return RedirectToAction("Index", "Billetera");
                }
                else
                {
                    return PartialView(billetera);
                }

            }
            catch (System.Exception e)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al cargar la billetera. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index", "Billetera");
            }
        }
        public IActionResult Delete(int id)
        {
            try
            {
                Billetera billetera = _context.Billeteras.Where(s => s.Id == id).First();
                _context.Billeteras.Remove(billetera);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, "Se eliminó correctamente la billetera del Usuario " + billetera.Cliente.Usuario.Email + ".");
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Success, "Hubo un error al eliminar el billetera.");
                return RedirectToAction("Index", "Billetera");
            }
        }

        public ActionResult _Update(int id)
        {
            return PartialView(_context.Billeteras.Where(s => s.Id == id).First());
        }
        [HttpPost]
        public async Task<ActionResult> _Update(Billetera billetera, string Cliente)
        {
            try
            {
                if (Cliente == null)
                {
                    ModelState.AddModelError("Cliente", "Debe seleccionar un Cliente");
                }
                if (ModelState.IsValid)
                {
                    billetera.Cliente = _context.Clientes.Find(Convert.ToInt32(Cliente));
                    _context.Billeteras.Update(billetera);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se modifico corretamente la billetera del Usuario " + billetera.Cliente.Usuario.Email + ".");
                    return RedirectToAction("Index", "Billetera");
                }
                else
                {
                    return PartialView(billetera);
                }
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al modificar el Banco. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index", "Billetera");
            }
        }
    }
}
