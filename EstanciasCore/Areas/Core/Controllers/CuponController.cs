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
    public class CuponController : EstanciasCoreController
    {
        public CuponController(EstanciasContext context) : base(context)
        {
            breadcumb.Add(new Message() { DisplayName = "Validación de Cupones" });
        }

        public IActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Validar Cupón" });
            ViewBag.Breadcrumb = breadcumb;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Buscar(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                AddPageAlerts(PageAlertType.Error, "Debe ingresar un código de cupón o DNI.");
                return RedirectToAction("Index");
            }

            codigo = codigo.Trim().ToUpper();

            try
            {
                var cupones = await _context.HistorialCanje
                    .Include(x => x.Premio)
                    .Include(x => x.Cliente)
                    .Include(x => x.Cliente.Persona)
                    .Where(x => (x.CodigoCupon == codigo || x.Cliente.Persona.NroDocumento == codigo) && x.Activo)
                    .OrderBy(x => x.FechaVencimientoCupon)
                    .ToListAsync();

                if (cupones.Any())
                {
                    var cupones_disponibles = cupones.Where(x => x.FechaVencimientoCupon.Date >= DateTime.Now.Date).ToList();

                    if (!cupones_disponibles.Any())
                    {
                        AddPageAlerts(PageAlertType.Warning, "Se encontraron cupones, pero se encuentran vencidos.");
                        return RedirectToAction("Index");
                    }

                    breadcumb.Add(new Message() { DisplayName = "Validar Cupón" });
                    ViewBag.Breadcrumb = breadcumb;
                    
                    if (cupones_disponibles.Count == 1 && cupones_disponibles.First().CodigoCupon == codigo)
                    {
                        AddPageAlerts(PageAlertType.Info, $"Se encontró el cupón {codigo}.");
                    }
                    else
                    {
                        AddPageAlerts(PageAlertType.Info, $"Se encontraron {cupones_disponibles.Count} cupon(es) disponible(s).");
                    }
                    
                    return View("Index", cupones_disponibles);
                }

                AddPageAlerts(PageAlertType.Error, "No existe el cupón ingresado o el cliente no tiene cupones disponibles.");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                AddPageAlerts(PageAlertType.Error, "Ocurrió un error al buscar.");
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Validar(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                AddPageAlerts(PageAlertType.Error, "Debe ingresar un código de cupón.");
                return RedirectToAction("Index");
            }

            codigo = codigo.Trim().ToUpper();

            try
            {
                var cupon = await _context.HistorialCanje
                    .Include(x => x.Premio)
                    .Include(x => x.Cliente)
                    .FirstOrDefaultAsync(x => x.CodigoCupon == codigo);

                if (cupon != null)
                {
                    if (!cupon.Activo)
                    {
                        AddPageAlerts(PageAlertType.Warning, "El cupón ya fue utilizado o ha sido anulado.");
                        return RedirectToAction("Index");
                    }

                    if (cupon.FechaVencimientoCupon.Date < DateTime.Now.Date)
                    {
                        AddPageAlerts(PageAlertType.Error, "El cupón ingresado se encuentra vencido.");
                        return RedirectToAction("Index");
                    }

                    // If everything is OK, we validate it
                    cupon.Activo = false;
                    cupon.Fecha = DateTime.Now;
                    
                    _context.HistorialCanje.Update(cupon);
                    await _context.SaveChangesAsync();

                    AddPageAlerts(PageAlertType.Success, $"Cupón validado con éxito. Premio: {cupon.Premio?.Nombre}");
                    return RedirectToAction("Index");
                }

                AddPageAlerts(PageAlertType.Error, "No existe el cupón ingresado.");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                AddPageAlerts(PageAlertType.Error, "Ocurrió un error al intentar validar el cupón.");
                return RedirectToAction("Index");
            }
        }
    }
}
