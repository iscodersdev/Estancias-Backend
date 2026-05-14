using Commons.Models;
using Microsoft.AspNetCore.Mvc;
using DAL.Data;
using DAL.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using Microsoft.AspNetCore.Hosting;

namespace EstanciasCore.Controllers
{
    [Area("Core")]
    public class UsuarioCategoriasController : EstanciasCoreController
    {
        private IHostingEnvironment _env;
        public UsuarioCategoriasController(EstanciasContext context, IHostingEnvironment env) : base(context)
        {
            _env = env; 
            breadcumb.Add(new Message() { DisplayName = "Categorías de Usuarios" });
        }
        public ActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Categorías de Usuarios" });
            return View();
        }
        public ActionResult Create()
        {
            return PartialView();
        }
        public IActionResult ObtenerUsuariosCategorias(Page<UsuariosCategorias> page)
        {
            page.SelectPage("/UsuarioCategorias/ObtenerUsuariosCategorias", _context.UsuariosCategorias, x => string.IsNullOrEmpty(page.SearchText) || x.Nombre.Contains(page.SearchText));
            return PartialView("_Listado", page);
        }

        [HttpPost]
        public async Task<ActionResult> Create(UsuariosCategorias categoria)
        {
            try
            {
                _context.UsuariosCategorias.Add(categoria);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, " Se registró correctamente la Categoría.");
                return RedirectToAction("Index", "UsuarioCategorias");
            }
            catch(Exception e)
            {
                AddPageAlerts(PageAlertType.Error, " Hubo un error al registrar la Categoría.");
                return RedirectToAction("Index", "UsuarioCategorias");
            }
        }
        
        [HttpGet]
        public bool Delete(int id)
        {
            try
            {
				UsuariosCategorias categoria = _context.UsuariosCategorias.Where(s => s.Id == id).First();
                _context.UsuariosCategorias.Remove(categoria);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public ActionResult Update(int id)
        {
            return PartialView(_context.UsuariosCategorias.Where(s => s.Id == id).First());
        }

        [HttpPost]
        public async Task<ActionResult> Update(UsuariosCategorias categoria)
        {
            try
            {
                UsuariosCategorias d = _context.UsuariosCategorias.Where(s => s.Id == categoria.Id).First();
                d.Nombre = categoria.Nombre;
                d.Color = categoria.Color;
                d.CodigoColor = categoria.CodigoColor;
                d.Orden = categoria.Orden;
                d.Activo = categoria.Activo;
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, " Se actualizó correctamente la Categoría.");
                return RedirectToAction("Index", "UsuarioCategorias");
            }
            catch(Exception e)
            {
                AddPageAlerts(PageAlertType.Error, " Hubo un error al actualizar la Categoría.");
                return RedirectToAction("Index", "UsuarioCategorias");
            }
        }

        [HttpPost]
        public IActionResult ModificarOrden(int Id, int Orden)
        {
            try
            {
                UsuariosCategorias categoria = _context.UsuariosCategorias.Where(x => x.Id == Id).FirstOrDefault();
                categoria.Orden = Orden;
                _context.UsuariosCategorias.Update(categoria);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, "Se modificó el orden de la Categoría");
                return RedirectToAction("Index", "UsuarioCategorias");
            }
            catch (Exception)
            {
                AddPageAlerts(PageAlertType.Error, "No se pudo modificar el orden.");
                return RedirectToAction("Index", "UsuarioCategorias");
            }
        }
    }    
}