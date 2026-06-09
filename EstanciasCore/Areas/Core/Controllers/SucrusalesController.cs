using Commons.Models;
using DAL.Data;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
                _context.Sucursales.Include(x => x.Marca), c);

            return PartialView("_ListadoSucursales", page);
        }

        public IActionResult _Create()
        {
            ViewBag.Marcas = _context.Marcas
                                .Where(x => x.Activo)
                                .OrderBy(x => x.Orden)
                                .Select(x => new SelectListItem { Text = x.Nombre, Value = x.Id.ToString() })
                                .ToList();
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Create(Sucursales sucursales, int? MarcaId)
        {
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                try
                {
                    if (MarcaId.HasValue && MarcaId.Value > 0)
                    {
                        sucursales.Marca = await _context.Marcas.FindAsync(MarcaId.Value);
                    }
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
                ViewBag.Marcas = _context.Marcas
                                    .Where(x => x.Activo)
                                    .OrderBy(x => x.Orden)
                                    .Select(x => new SelectListItem { Text = x.Nombre, Value = x.Id.ToString() })
                                    .ToList();
                return PartialView(sucursales);
            }
        }


        public async Task<IActionResult> _Update(int Id)
        {
            Sucursales sucursal = await _context.Sucursales.Include(x => x.Marca).FirstOrDefaultAsync(x => x.Id == Id);
            if (sucursal == null)
            {
                sucursal = await _context.Sucursales.FindAsync(Id);
            }
            var currentMarcaId = sucursal?.Marca?.Id;
            ViewBag.Marcas = _context.Marcas
                                .Where(x => x.Activo)
                                .OrderBy(x => x.Orden)
                                .Select(x => new SelectListItem 
                                { 
                                    Text = x.Nombre, 
                                    Value = x.Id.ToString(),
                                    Selected = currentMarcaId.HasValue && x.Id == currentMarcaId.Value
                                })
                                .ToList();
            return PartialView(sucursal);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Update(Sucursales sucursal, int? MarcaId)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.Sucursales.Include(x => x.Marca).FirstOrDefaultAsync(x => x.Id == sucursal.Id);
                    if (existing == null)
                    {
                        return NotFound();
                    }
                    existing.name = sucursal.name;
                    existing.address = sucursal.address;
                    existing.phone = sucursal.phone;
                    existing.latitude = sucursal.latitude;
                    existing.longitude = sucursal.longitude;
                    existing.group = sucursal.group;

                    if (MarcaId.HasValue && MarcaId.Value > 0)
                    {
                        existing.Marca = await _context.Marcas.FindAsync(MarcaId.Value);
                    }
                    else
                    {
                        existing.Marca = null;
                    }

                    _context.Sucursales.Update(existing);
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
                var currentMarcaId = MarcaId;
                ViewBag.Marcas = _context.Marcas
                                    .Where(x => x.Activo)
                                    .OrderBy(x => x.Orden)
                                    .Select(x => new SelectListItem 
                                    { 
                                        Text = x.Nombre, 
                                        Value = x.Id.ToString(),
                                        Selected = currentMarcaId.HasValue && x.Id == currentMarcaId.Value
                                    })
                                    .ToList();
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