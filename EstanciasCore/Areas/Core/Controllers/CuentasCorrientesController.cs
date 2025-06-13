using Commons.Models;
using DAL.Data;
using DAL.DTOs;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using EstanciasCore.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.Areas.Core.Controllers
{
    [Area("Core")]
    public class CuentasCorrientesController : EstanciasCoreController
    {
        public CuentasCorrientesController(EstanciasContext context) : base(context)
        {
            breadcumb.Add(new Message() { DisplayName = "Datos" });
            breadcumb.Add(new Message() { DisplayName = "Generales" });
        }

        public ActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Cuentas Corrientes" });
            ViewBag.Breadcrumb = breadcumb;
            return View();
        }

        public IActionResult CuentasCorrientesDataTable()
        {
            List<CuentaCorriente> cuentasCorrientes = _context.CuentaCorriente.ToList();
            var query = from e in cuentasCorrientes
                        select new CuentasCorrientesDTO
                        {
                            Id = e.Id,
                            ClienteNombre = e.Cliente?.Persona.Nombres+ "-" +e.Cliente?.Persona.Apellido,
                            Fecha = e.Fecha.ToShortDateString(),
                            Vencimiento = e.Vencimiento.ToString(),
                            Saldo=e.Saldo
            };
            return DataTable(query.AsQueryable());
        }

        public ActionResult _Create()
        {
            CuentasCorrientesViewBag(null);
            return PartialView();
        }

        [HttpPost]
        public async Task<ActionResult> _Create([Bind("Fecha,Vencimiento,Observaciones,Importe,Saldo,Cliente,Concepto")] CuentaCorriente cuentaCorriente)
        {
            try
            {
                if (cuentaCorriente.Cliente == null || cuentaCorriente.Cliente.Id==0)
                {
                    ModelState.AddModelError("Cliente.Id", "Debe seleccionar un Cliente");
                }
                else
                {
                    cuentaCorriente.Cliente = await _context.Clientes.FindAsync(cuentaCorriente.Cliente.Id);
                }
                if (cuentaCorriente.Concepto == null)
                {
                    ModelState.AddModelError("Concepto", "Debe seleccionar un Concepto");
                }
                ModelState.Remove("Id");
                if (ModelState.IsValid)
                {                    
                    cuentaCorriente.Concepto = await _context.Conceptos.FindAsync(cuentaCorriente.Concepto.Id);
                    await _context.CuentaCorriente.AddAsync(cuentaCorriente);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se cargo correctamente la Cuenta Corriente.");
                    return RedirectToAction("Index", "CuentasCorrientes");
                }
                else
                {
                    if (cuentaCorriente.Cliente == null || cuentaCorriente.Cliente.Id == 0)
                    {
                        cuentaCorriente.Cliente = null;
                    }
                    CuentasCorrientesViewBag(cuentaCorriente.Cliente);
                    return PartialView(cuentaCorriente);
                }
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al cargar la Cuenta Corriente. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index", "CuentasCorrientes");
            }
        }

        public ActionResult _Update(int id)
        {            
            CuentaCorriente cuentaCorriente = _context.CuentaCorriente.Where(s => s.Id == id).First();
            CuentasCorrientesViewBag(cuentaCorriente.Cliente);
            return PartialView(cuentaCorriente);
        }
        [HttpPost]
        public async Task<ActionResult> _Update(CuentaCorriente cuentaCorriente)
        {
            try
            {
                if (cuentaCorriente.Cliente == null)
                {
                    ModelState.AddModelError("Cliente", "Debe seleccionar un Cliente");
                }
                else
                {
                    cuentaCorriente.Cliente = await _context.Clientes.FindAsync(cuentaCorriente.Cliente.Id);
                }
                if (cuentaCorriente.Concepto == null)
                {
                    ModelState.AddModelError("Conceptp", "Debe seleccionar un Concepto");
                }
                if (ModelState.IsValid)
                {
                    cuentaCorriente.Concepto = await _context.Conceptos.FindAsync(cuentaCorriente.Concepto.Id);
                    _context.CuentaCorriente.Update(cuentaCorriente);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se modifico corretamente la Cuenta Corriente.");
                    return RedirectToAction("Index", "CuentasCorrientes");
                }
                else
                {
                    CuentasCorrientesViewBag(cuentaCorriente.Cliente);
                    return PartialView(cuentaCorriente);
                }
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al modificar la Cuenta Corriente. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index", "CuentasCorrientes");
            }
        }

        private void CuentasCorrientesViewBag(Clientes cliente=null)
        {
            ViewBag.Conceptos = _context.Conceptos.Select(g => new SelectListItem() { Text = g.Nombre, Value = g.Id.ToString() });
            if (cliente != null)
            {
                ViewBag.ClienteId = cliente.Id;
                ViewBag.ClienteDescripcion= cliente.Persona.Nombres?.ToUpper() + " " + cliente.Persona.Apellido?.ToUpper();
            }
        }
    }
}
