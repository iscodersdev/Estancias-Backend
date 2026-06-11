using Commons.Models;
using DAL.Data;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
            ViewBag.Marcas = _context.Marcas
                                .Where(x => x.Activo)
                                .OrderBy(x => x.Orden)
                                .Select(x => new SelectListItem { Text = x.Nombre, Value = x.Id.ToString() })
                                .ToList();
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Create(Sucursales sucursales, List<int> MarcaIds)
        {
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                try
                {
                    await _context.Sucursales.AddAsync(sucursales);
                    await _context.SaveChangesAsync();

                    if (MarcaIds != null && MarcaIds.Any())
                    {
                        foreach (var id in MarcaIds)
                        {
                            var marca = await _context.Marcas.FindAsync(id);
                            if (marca != null)
                            {
                                await _context.SucursalesMarcas.AddAsync(new SucursalesMarcas { Sucursales = sucursales, Marca = marca });
                            }
                        }
                        await _context.SaveChangesAsync();
                    }

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
            Sucursales sucursal = await _context.Sucursales.FirstOrDefaultAsync(x => x.Id == Id);
            if (sucursal == null)
            {
                sucursal = await _context.Sucursales.FindAsync(Id);
            }
            var selectedMarcas = await _context.SucursalesMarcas
                                        .Where(x => x.Sucursales.Id == Id)
                                        .Select(x => x.Marca.Id)
                                        .ToListAsync();
            ViewBag.Marcas = _context.Marcas
                                .Where(x => x.Activo)
                                .OrderBy(x => x.Orden)
                                .Select(x => new SelectListItem 
                                { 
                                    Text = x.Nombre, 
                                    Value = x.Id.ToString(),
                                    Selected = selectedMarcas.Contains(x.Id)
                                })
                                .ToList();
            return PartialView(sucursal);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Update(Sucursales sucursal, List<int> MarcaIds)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.Sucursales.FirstOrDefaultAsync(x => x.Id == sucursal.Id);
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

                    _context.Sucursales.Update(existing);

                    var existingMarcas = await _context.SucursalesMarcas.Where(x => x.Sucursales.Id == sucursal.Id).ToListAsync();
                    _context.SucursalesMarcas.RemoveRange(existingMarcas);

                    if (MarcaIds != null && MarcaIds.Any())
                    {
                        foreach (var id in MarcaIds)
                        {
                            var marca = await _context.Marcas.FindAsync(id);
                            if (marca != null)
                            {
                                await _context.SucursalesMarcas.AddAsync(new SucursalesMarcas { Sucursales = existing, Marca = marca });
                            }
                        }
                    }

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
                var selectedMarcas = MarcaIds ?? new List<int>();
                ViewBag.Marcas = _context.Marcas
                                    .Where(x => x.Activo)
                                    .OrderBy(x => x.Orden)
                                    .Select(x => new SelectListItem 
                                    { 
                                        Text = x.Nombre, 
                                        Value = x.Id.ToString(),
                                        Selected = selectedMarcas.Contains(x.Id)
                                    })
                                    .ToList();
                return PartialView(sucursal);
            }
        }


        public IActionResult Delete(int id)
        {
            try
            {
                var existingMarcas = _context.SucursalesMarcas.Where(x => x.Sucursales.Id == id).ToList();
                if(existingMarcas.Any())
                {
                    _context.SucursalesMarcas.RemoveRange(existingMarcas);
                }

                Sucursales sucursal = _context.Sucursales.Where(s => s.Id == id).First();
                _context.Sucursales.Remove(sucursal);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, "Se eliminó correctamente la Sucursal.");
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al eliminar la Sucursal.");
                return RedirectToAction("Index", "Sucursales");
            }
        }


    }
}