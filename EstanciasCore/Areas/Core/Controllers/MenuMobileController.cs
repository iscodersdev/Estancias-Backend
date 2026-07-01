using EstanciasCore.Controllers;
using Commons.Models;
using DAL.Data;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL.Models.Core;

namespace EstanciasCore.Areas.Core.Controllers
{
    [Area("Core")]
    public class MenuMobileController : EstanciasCoreController
    {
        public MenuMobileController(EstanciasContext context) : base(context)
        {
            breadcumb.Add(new Message() { DisplayName = "Datos" });
        }

        public ActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Menu Mobile" });
            ViewBag.Breadcrumb = breadcumb;

            var opciones = _context.MenuMobile.OrderBy(x => x.Nombre).ToList();

            return View(opciones);
        }

        public ActionResult _Create()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> _Create([Bind("Nombre, Descripcion")] MenuMobile nuevaOpcion)
        {
            try
            {
                ModelState.Remove("Id");
                if (ModelState.IsValid)
                {
                    nuevaOpcion.Activo = true;
                    await _context.MenuMobile.AddAsync(nuevaOpcion);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se cargó correctamente la opción " + nuevaOpcion.Nombre + ".");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return PartialView(nuevaOpcion);
                }
            }
            catch (Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al cargar la opción. Inténtelo nuevamente más tarde.");
                return RedirectToAction(nameof(Index));
            }
        }

        public ActionResult _Update(int id)
        {
            MenuMobile opcion = _context.MenuMobile.Find(id);
            if (opcion != null)
            {
                return PartialView(opcion);
            }
            AddPageAlerts(PageAlertType.Error, "Hubo un error al editar la opción. Inténtelo nuevamente más tarde.");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> _Update(MenuMobile editarOpcion)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var opcionExistente = await _context.MenuMobile.FindAsync(editarOpcion.Id);
                    if (opcionExistente == null)
                    {
                        AddPageAlerts(PageAlertType.Error, "No se encontró la opción a modificar.");
                        return RedirectToAction(nameof(Index));
                    }

                    opcionExistente.Nombre = editarOpcion.Nombre;
                    opcionExistente.Descripcion = editarOpcion.Descripcion;
                    opcionExistente.Activo = editarOpcion.Activo;
                    opcionExistente.Codigo = editarOpcion.Codigo;

                    _context.MenuMobile.Update(opcionExistente);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se modificó correctamente la opción.");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return PartialView(editarOpcion);
                }
            }
            catch (Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al modificar la opción. Inténtelo nuevamente más tarde.");
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Delete(int id)
        {
            try
            {
                MenuMobile opcion = _context.MenuMobile.Find(id);
                if (opcion != null)
                {
                    opcion.Activo = !opcion.Activo;
                    _context.MenuMobile.Update(opcion);
                    _context.SaveChanges();
                    AddPageAlerts(PageAlertType.Success, "Se " + (opcion.Activo ? "habilitó" : "deshabilitó") + " correctamente la opción " + opcion.Nombre + ".");
                    return RedirectToAction(nameof(Index));
                }
                AddPageAlerts(PageAlertType.Error, "Hubo un error al cambiar el estado de la opción. Inténtelo nuevamente más tarde.");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al cambiar el estado de la opción. Inténtelo nuevamente más tarde.");
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Usuarios(int id)
        {
            breadcumb.Add(new Message() { DisplayName = "Menu Mobile" });
            breadcumb.Add(new Message() { DisplayName = "Usuarios" });
            ViewBag.Breadcrumb = breadcumb;
            
            var menuMobile = _context.MenuMobile.Find(id);
            if (menuMobile == null)
            {
                AddPageAlerts(PageAlertType.Error, "Opción no encontrada.");
                return RedirectToAction(nameof(Index));
            }
            
            var asignados = _context.MenuMobileUsuariosHabilitados
                .Include(x => x.Usuario)
                .ThenInclude(u => u.Personas)
                .Where(x => x.MenuMobile.Id == id)
                .ToList();

            ViewBag.MenuMobile = menuMobile;

            return View(asignados);
        }

        [HttpPost]
        public async Task<IActionResult> AddUsuarioByDni(int MenuMobileId, string Dni)
        {
            try
            {
                var usuario = _context.Usuarios.Include(u => u.Personas).FirstOrDefault(u => u.Personas.NroDocumento == Dni);
                
                if (usuario == null)
                {
                    AddPageAlerts(PageAlertType.Error, "No se encontró ningún usuario activo con ese DNI.");
                    return RedirectToAction(nameof(Usuarios), new { id = MenuMobileId });
                }

                if (!_context.MenuMobileUsuariosHabilitados.Any(x => x.MenuMobile.Id == MenuMobileId && x.Usuario.Id == usuario.Id))
                {
                    var asignacion = new MenuMobileUsuariosHabilitados
                    {
                        MenuMobile = await _context.MenuMobile.FindAsync(MenuMobileId),
                        Usuario = await _context.Usuarios.FindAsync(usuario.Id)
                    };
                    await _context.MenuMobileUsuariosHabilitados.AddAsync(asignacion);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Usuario asignado correctamente.");
                }
                else
                {
                    AddPageAlerts(PageAlertType.Error, "El usuario ya se encuentra asignado.");
                }
            }
            catch (Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Ocurrió un error al asignar el usuario.");
            }
            
            return RedirectToAction(nameof(Usuarios), new { id = MenuMobileId });
        }

        public async Task<IActionResult> RemoveUsuario(int id)
        {
            var asignacion = await _context.MenuMobileUsuariosHabilitados.FindAsync(id);
            if (asignacion != null)
            {
                int menuMobileId = asignacion.MenuMobile.Id;
                _context.MenuMobileUsuariosHabilitados.Remove(asignacion);
                await _context.SaveChangesAsync();
                AddPageAlerts(PageAlertType.Success, "Usuario desasignado correctamente.");
                return RedirectToAction(nameof(Usuarios), new { id = menuMobileId });
            }
            
            AddPageAlerts(PageAlertType.Error, "No se encontró la asignación.");
            return RedirectToAction(nameof(Index));
        }
    }
}
