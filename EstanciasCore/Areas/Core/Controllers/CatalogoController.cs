using Commons.Models;
using DAL.Data;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
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
                _context.Catalogo, c);

            return PartialView("_ListadoCatalogo", page);
        }

        public IActionResult _Create()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Create(Catalogo catalogo)
        {
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                try
                {
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
                return PartialView(catalogo);
            }
        }


        public async Task<IActionResult> _Update(int Id)
        {

            Catalogo catalogo = await _context.Catalogo.FindAsync(Id);
            return PartialView(catalogo);
        }

  


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Update(Catalogo catalogo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Catalogo catalogoUpdate = _context.Catalogo.Find(catalogo.Id);
                    catalogoUpdate.Nombre = catalogo.Nombre;
                    catalogoUpdate.Descripcion = catalogo.Descripcion;
                    catalogoUpdate.Link = catalogo.Link;
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
                return PartialView(catalogo);
            }
        }

        public bool ActivarCatalogo(int id)
        {
            try
            {
                List<Catalogo> ListCatalogo = _context.Catalogo.ToList();
                foreach (var item in ListCatalogo)
                {
                    if (item.Id==id)
                    {
                        item.Activo=true;
                    }
                    else
                    {
                        item.Activo=false;
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
