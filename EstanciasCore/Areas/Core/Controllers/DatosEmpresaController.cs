using Commons.Models;
using DAL.Data;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.Controllers
{
    [Area("Core")]
    public class DatosEmpresaController : EstanciasCoreController
    {
        public DatosEmpresaController(EstanciasContext context) : base(context)
        {
            breadcumb.Add(new Message() { DisplayName = "Datos Empresa" });
        }

        public IActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Datos Empresa" });
            ViewBag.Breadcrumb = breadcumb;
            return View();
        }

        public async Task<IActionResult> _listadoDatosEmpresa(Page<DatosEstructura> page)
        {
            var c = await _context.DatosEstructura.CountAsync();
            if (c < 1) { c = 1; }
            page.SelectPage("/DatosEmpresa/_ListadoDatosEmpresa",
                _context.DatosEstructura, c);

            return PartialView("_ListadoDatosEmpresa", page);
        }

        

        public async Task<IActionResult> _Update(int Id)
        {
            DatosEstructura datosEmpresa = await _context.DatosEstructura.FindAsync(Id);
            DatosEmpresa datos = new DatosEmpresa()
            {
                Alias = datosEmpresa.Alias,
                CBU = datosEmpresa.CBU,
                Id = datosEmpresa.Id,
                Whatsapp = datosEmpresa.Telefono

            };
            return PartialView(datos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Update(DatosEmpresa datos)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    DatosEstructura datosEmpresa = _context.DatosEstructura.Where(x => x.Id == datos.Id).FirstOrDefault();
                    datosEmpresa.Alias = datos.Alias;
                    datosEmpresa.Telefono = datos.Whatsapp;
                    datosEmpresa.CBU = datos.CBU;
                    _context.DatosEstructura.Update(datosEmpresa);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se editó correctamente los datos de empresa");
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al editar el dato de empresa. Inténtelo nuevamente más tarde.");
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return PartialView(datos);
            }
        }

       
    }
}
        