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
    public class CatalogoController : EstanciasCoreController
    {
        public CatalogoController(EstanciasContext context) : base(context)
        {
            breadcumb.Add(new Message() { DisplayName = "Datos" });
        }

        public IActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Catálogo" });
            ViewBag.Breadcrumb = breadcumb;
            return View();
        }

        public async Task<IActionResult> _ListadoCatalogo(Page<Catalogo> page)
        {
            var c = _context.Catalogo.Count();
            if (c < 1) { c = 1; }
            page.SelectPage("/Catalogo/_ListadoCatalogo",
                _context.Catalogo.Include(x => x.Marca), c);

            return PartialView("_ListadoCatalogo", page);
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
        public async Task<IActionResult> _Create(Catalogo catalogo, int? MarcaId)
        {
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                try
                {
                    if (MarcaId.HasValue && MarcaId.Value > 0)
                    {
                        catalogo.Marca = await _context.Marcas.FindAsync(MarcaId.Value);
                    }
                    await _context.Catalogo.AddAsync(catalogo);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se creó correctamente el Catálogo " + catalogo.Nombre + ".");
                    return RedirectToAction("Index", "Catalogo");
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al crear el Catálogo. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "Catalogo");
                }
            }
            else
            {
                ViewBag.Marcas = _context.Marcas
                                    .Where(x => x.Activo)
                                    .OrderBy(x => x.Orden)
                                    .Select(x => new SelectListItem { Text = x.Nombre, Value = x.Id.ToString() })
                                    .ToList();
                return PartialView(catalogo);
            }
        }


        public async Task<IActionResult> _Update(int Id)
        {
            Catalogo catalogo = await _context.Catalogo.Include(x => x.Marca).FirstOrDefaultAsync(x => x.Id == Id);
            if (catalogo == null)
            {
                catalogo = await _context.Catalogo.FindAsync(Id);
            }
            var currentMarcaId = catalogo?.Marca?.Id;
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
            return PartialView(catalogo);
        }

  


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Update(Catalogo catalogo, int? MarcaId)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Catalogo catalogoUpdate = await _context.Catalogo.Include(x => x.Marca).FirstOrDefaultAsync(x => x.Id == catalogo.Id);
                    if (catalogoUpdate == null)
                    {
                        return NotFound();
                    }
                    catalogoUpdate.Nombre = catalogo.Nombre;
                    catalogoUpdate.Descripcion = catalogo.Descripcion;
                    catalogoUpdate.Link = catalogo.Link;

                    if (MarcaId.HasValue && MarcaId.Value > 0)
                    {
                        catalogoUpdate.Marca = await _context.Marcas.FindAsync(MarcaId.Value);
                    }
                    else
                    {
                        catalogoUpdate.Marca = null;
                    }

                    _context.Catalogo.Update(catalogoUpdate);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se editó correctamente el Catálogo " + catalogo.Nombre + ".");
                    return RedirectToAction("Index", "Catalogo");
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al editar el Catálogo. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "Catalogo");
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
                return PartialView(catalogo);
            }
        }


        public bool ActivarCatalogo(int id)
        {
            try
            {
                var catalogoToActivate = _context.Catalogo.Include(x => x.Marca).FirstOrDefault(x => x.Id == id);
                if (catalogoToActivate == null) return false;

                List<Catalogo> ListCatalogo;
                if (catalogoToActivate.Marca != null)
                {
                    ListCatalogo = _context.Catalogo.Include(x => x.Marca)
                        .Where(x => x.Marca != null && x.Marca.Id == catalogoToActivate.Marca.Id)
                        .ToList();
                }
                else
                {
                    ListCatalogo = _context.Catalogo.Include(x => x.Marca)
                        .Where(x => x.Marca == null)
                        .ToList();
                }

                foreach (var item in ListCatalogo)
                {
                    if (item.Id == id)
                    {
                        item.Activo = true;
                    }
                    else
                    {
                        item.Activo = false;
                    }
                }
                _context.Catalogo.UpdateRange(ListCatalogo);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public IActionResult Delete(int id)
        {
            try
            {
                Catalogo catalogo = _context.Catalogo.Where(s => s.Id == id).First();
                _context.Catalogo.Remove(catalogo);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, "Se eliminó correctamente el Catálogo.");
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Success, "Hubo un error al eliminar el Catálogo.");
                return RedirectToAction("Index", "Catalogo");
            }
        }

    }
}
