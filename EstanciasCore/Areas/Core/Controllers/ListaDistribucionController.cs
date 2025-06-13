using Commons.Models;
using DAL.Data;
using DAL.Models;
using DAL.Models.Core;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.Controllers
{
    [Area("Core")]
    public class ListaDistribucionController : EstanciasCoreController
    {
        public ListaDistribucionController(EstanciasContext context) : base(context)
        {
            breadcumb.Add(new Message() { DisplayName = "Datos" });
        }

        public IActionResult Index(int Id)
        {
            breadcumb.Add(new Message() { DisplayName = "Lista de Distribución" });
            ViewBag.Breadcrumb = breadcumb;
            if (Id!=0)
            {
                ViewBag.ListaId = Id;
            }
            else
            {
                ViewBag.ListaId = null;
            }
            return View();
        }

        public async Task<IActionResult> _ListadoDistribucion(Page<ListaDistribucion> page)
        {
            var c = _context.ListaDistribucion.Count();
            if (c < 1) { c = 1; }
            page.SelectPage("/ListaDistribucion/_ListadoDistribucion",
                _context.ListaDistribucion, c);

            return PartialView("_ListadoDistribucion", page);
        }

        public IActionResult _Create()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Create(ListaDistribucion lista)
        {
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                try
                {
                    await _context.ListaDistribucion.AddAsync(lista);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se creó correctamente la Lista de Distribución " + lista.Nombre + ".");
                    return RedirectToAction("Index", "ListaDistribucion");
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al crear la Lista de Distribución. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "ListaDistribucion");
                }
            }
            else
            {
                return PartialView(lista);
            }
        }


        public async Task<IActionResult> _Update(int Id)
        {

            ListaDistribucion lista = await _context.ListaDistribucion.FindAsync(Id);
            return PartialView(lista);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Update(ListaDistribucion lista)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.ListaDistribucion.Update(lista);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se editó correctamente la Lista de Distribución " + lista.Nombre + ".");
                    return RedirectToAction("Index", "ListaDistribucion");
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al editar la Lista de Distribución. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "ListaDistribucion");
                }
            }
            else
            {
                return PartialView(lista);
            }
        }


        public IActionResult Delete(int id)
        {
            try
            {
                ListaDistribucion lista = _context.ListaDistribucion.Where(s => s.Id == id).First();

                List<DistribucionDestinatarios> destinatarios = _context.DistribucionDestinatarios.Where(s => s.ListaDistribucion.Id == lista.Id).ToList();

                _context.DistribucionDestinatarios.RemoveRange(destinatarios);
                _context.ListaDistribucion.Remove(lista);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, "Se eliminó correctamente la Lista.");
                return RedirectToAction("Index", "ListaDistribucion");
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Success, "Hubo un error al eliminar la Lista.");
                return RedirectToAction("Index", "ListaDistribucion");
            }
        }

        /*------------------------------------------------------------ Destinatarios ---------------------------------------------------------------------------------*/

        public async Task<IActionResult> _ListadoDestinatarios(Page<DistribucionDestinatarios> page, int Id)
        {
            ListaDistribucion lista = _context.ListaDistribucion.Where(x => x.Id==Id).First();
            ViewBag.Lista = lista.Nombre;
            ViewBag.ListaId = lista.Id;

            var c = _context.DistribucionDestinatarios.Where(x=>x.ListaDistribucion.Id==Id).Count();
            if (c < 1) { c = 1; }
            page.SelectPage("/ListaDistribucion/_ListadoDestinatarios",
                _context.DistribucionDestinatarios.Where(x => x.ListaDistribucion.Id==Id), c);

            return PartialView("_ListadoDestinatarios", page);
        }

        public IActionResult _CreateDestinatario(int Id)
        {
            ViewBag.ListaId = Id;
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _CreateDestinatario(DistribucionDestinatarios destinatario, string destinatarioId)
        {
            destinatario.Id=0;
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                try
                {
                    if (destinatarioId==null)
                    {
                        AddPageAlerts(PageAlertType.Error, "Hubo un error al agregar el Destinatario. Intentelo nuevamente mas tarde.");
                        return RedirectToAction("Index", "ListaDistribucion", new { @Id = destinatario.ListaDistribucion.Id });
                    }
                    destinatario.ListaDistribucion = _context.ListaDistribucion.Where(x=>x.Id==destinatario.ListaDistribucion.Id).FirstOrDefault();
                    destinatario.Destinatario = _context.Usuarios.Where(x=>x.Id==destinatarioId).FirstOrDefault();
                    await _context.DistribucionDestinatarios.AddAsync(destinatario);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se agregó correctamente el Destinatario " + destinatario.Destinatario.UserName + ".");
                    return RedirectToAction("Index", "ListaDistribucion", new { @Id = destinatario.ListaDistribucion.Id });
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al agregar el Destinatario. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "ListaDistribucion", new { @Id = destinatario.ListaDistribucion.Id });
                }
            }
            else
            {
                return PartialView(destinatario);
            }
        }

        public IActionResult DeleteDestinatarios(int id)
        {
            DistribucionDestinatarios destinatario = _context.DistribucionDestinatarios.Where(s => s.Id == id).First();
            var ListaId = destinatario.ListaDistribucion.Id;
            try
            {
                _context.DistribucionDestinatarios.Remove(destinatario);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, "Se eliminó correctamente el Destinatario.");
                return RedirectToAction("Index", "ListaDistribucion", new { @Id = ListaId });
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Success, "Hubo un error al eliminar el Destinatario.");
                return RedirectToAction("Index", "ListaDistribucion", new { @Id = ListaId });
            }
        }

        public JsonResult DestinatariosComboJson(string q)
        {
            var items = _context.Usuarios
                .Where(x => x.Personas.NroDocumento.Contains(q))
                .Select(x => new
                {
                    Text = $"{x.Personas.Apellido}, {x.Personas.Nombres}",
                    Value = x.Id,
                    Subtext = $"{x.UserName}",
                    Icon = "fa fa-user"
                }).Take(10).ToArray();

            return Json(items);
        }

    }
}
