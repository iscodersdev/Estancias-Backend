using EstanciasCore.Controllers;
using Commons.Models;
using DAL.Data;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.Areas.Core.Controllers
{
    [Area("Core")]
    public class GruposController : EstanciasCoreController
    {
        public GruposController(EstanciasContext context) : base(context)
        {
            _context = context;
            breadcumb.Add(new Message() { DisplayName = "Datos" });
        }
        public ActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Grupos" });
            ViewBag.Breadcrumb = breadcumb;
            return View();
        }
        public IActionResult _ListarGrupos(Page<Grupos> page)
        {
            var c = _context.Grupos.Count();
            if (c < 1) { c = 1; }
            page.SelectPage("/Grupos/_ListarGrupos",
                _context.Grupos, c);

            return PartialView("_ListarGrupos", page);
        }

        public ActionResult _Create()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<ActionResult> _Create([Bind("Nombre")] Grupos grupo)
        {
            try
            {
                ModelState.Remove("Id");
                if (ModelState.IsValid)
                {
                    await _context.Grupos.AddAsync(grupo);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se cargo correctamente el Grupo " + grupo.Nombre + ".");
                    return RedirectToAction("Index", "Grupos");
                }
                else
                {
                    return PartialView(grupo);
                }

            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al cargar el Grupo. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index", "Grupos");
            }
        }
        public IActionResult Delete(int id)
        {
            try
            {
                Grupos grupo = _context.Grupos.Where(s => s.Id == id).First();
                _context.Grupos.Remove(grupo);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, "Se eliminó correctamente el Grupo " + grupo.Nombre + ".");
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Success, "Hubo un error al eliminar el Grupo.");
                return RedirectToAction("Index", "Grupos");
            }
        }

        public ActionResult _Update(int id)
        {
            return PartialView(_context.Grupos.Where(s => s.Id == id).First());
        }
        [HttpPost]
        public async Task<ActionResult> _Update(Grupos grupo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Grupos.Update(grupo);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se modifico corretamente el Grupo.");
                    return RedirectToAction("Index", "Grupos");
                }
                else
                {
                    return PartialView(grupo);
                }
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al modificar el Grupo. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index", "Grupos");
            }
        }
    }
}
