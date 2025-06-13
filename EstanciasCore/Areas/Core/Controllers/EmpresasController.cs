using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Commons.Models;
using DAL.Data;
using DAL.DTOs;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EstanciasCore.Controllers
{
    [Area("Core")]
    public class EmpresasController : EstanciasCoreController
    {
        public EmpresasController(EstanciasContext context) : base(context)
        {
            _context = context;
            breadcumb.Add(new Message() { DisplayName = "Datos" });
        }
        public ActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Empresas" });
            ViewBag.Breadcrumb = breadcumb;
            return View();
        }
        private IActionResult _ListarEmpresas(Page<Empresas> page)
        {
            var c = _context.Empresas.Count();
            if (c < 1) { c = 1; }
            page.SelectPage("/Empresas/_ListarEmpresas",
                _context.Empresas, c);

            return PartialView("_ListarEmpresas", page);
        }

        public IActionResult EmpresaDataTable()
        {
            List<Empresas> empresas = _context.Empresas.ToList();
            var query = from e in empresas
                        select new EmpresaDTO
                        {
                            Id = e.Id,
                            CUIT = e.CUIT.ToString(),
                            RazonSocial = e.RazonSocial,
                            Grupo = e.Grupo?.Nombre
                        };
            return DataTable<EmpresaDTO>(query.AsQueryable<EmpresaDTO>());
        }

        public ActionResult _Create()
        {
            EmpresasViewBag();
            return PartialView();
        }

        [HttpPost]
        public async Task<ActionResult> _Create([Bind("CUIT, RazonSocial, Domicilio, Telefono, Mail, Grupo, ColorFontCarnet, ColorCarnet, Instagram, Twitter, Facebook, WhatsApp, ColorFondo, ColorBotones, ColorLogin")] Empresas empresa)
        {
            try
            {
                if (empresa.Grupo == null)
                {
                    ModelState.AddModelError("Grupo", "Debe seleccionar un Grupo");
                }
                ModelState.Remove("Id");
                if (ModelState.IsValid)
                {
                    empresa.Grupo =await _context.Grupos.FindAsync(empresa.Grupo.Id);
                    await _context.Empresas.AddAsync(empresa);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se cargo correctamente la Empresa.");
                    return RedirectToAction("Index", "Empresas");
                }
                else
                {
                    EmpresasViewBag();
                    return PartialView(empresa);
                }

            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al cargar la Empresa. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index", "Empresas");
            }
        }



        public ActionResult _Image(int Id)
        {
            ViewBag.EmpresaId = Id;
            Empresas empresa = _context.Empresas.Where(s => s.Id == Id).First();

            if (empresa.FondoMobile != null)
                ViewBag.FondoMobile = Convert.ToBase64String(empresa.FondoMobile);

            if (empresa.GIFLogoMutual != null)
                ViewBag.GIFLogoMutual = Convert.ToBase64String(empresa.GIFLogoMutual);

            if (empresa.LogoMutual != null)
                ViewBag.LogoMutual = Convert.ToBase64String(empresa.LogoMutual);
            return PartialView();
        }

        [HttpPost]
        public async Task<ActionResult> _Image(int Id, IFormFile FondoMobile, IFormFile GIFLogoMutual, IFormFile LogoMutual,IFormFile ImagenLogin)
        {
            try
            {
                Empresas empresa = await _context.Empresas.FindAsync(Id);                
                if (FondoMobile != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await FondoMobile.CopyToAsync(memoryStream);
                        empresa.FondoMobile = memoryStream.ToArray();
                    }
                }

                if (GIFLogoMutual != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await GIFLogoMutual.CopyToAsync(memoryStream);
                        empresa.GIFLogoMutual = memoryStream.ToArray();
                    }
                }            

                if (LogoMutual != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await LogoMutual.CopyToAsync(memoryStream);
                        empresa.LogoMutual = memoryStream.ToArray();
                    }
                }
                if (ImagenLogin != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await ImagenLogin.CopyToAsync(memoryStream);
                        empresa.ImagenLogin = memoryStream.ToArray();
                    }
                }

                _context.Empresas.Update(empresa);
                await _context.SaveChangesAsync();                
                AddPageAlerts(PageAlertType.Success, "Se cargo correctamente las Imagenes de la Empresa.");
                return RedirectToAction("Index", "Empresas");

            }
            catch (System.Exception e)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al cargar las Imagenes de la Empresa. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index", "Empresas");
            }
        }



        public IActionResult Delete(int id)
        {
            try
            {
                Empresas empresa = _context.Empresas.Where(s => s.Id == id).First();
                _context.Empresas.Remove(empresa);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, "Se eliminó correctamente la Empresa.");
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Success, "Hubo un error al eliminar la Empresa.");
                return RedirectToAction("Index", "Empresas");
            }
        }

        public ActionResult _Update(int id)
        {
            EmpresasViewBag();
            Empresas empresa = _context.Empresas.Where(s => s.Id == id).First();
            return PartialView(empresa);
        }
        [HttpPost]
        public async Task<ActionResult> _Update(Empresas empresa)
        {
            try
            {
                if (empresa.Grupo == null)
                {
                    ModelState.AddModelError("Grupo", "Debe seleccionar un Grupo");
                }
                if (ModelState.IsValid)
                {
                    
                    Empresas empresaUpdate =  await _context.Empresas.FindAsync(empresa.Id);
                    empresaUpdate.CUIT = empresa.CUIT;
                    empresaUpdate.RazonSocial = empresa.RazonSocial;
                    empresaUpdate.Abreviatura = empresa.Abreviatura;
                    empresaUpdate.Domicilio = empresa.Domicilio;
                    empresaUpdate.Telefono = empresa.Telefono;
                    empresaUpdate.Mail = empresa.Mail;
                    empresaUpdate.ColorFontCarnet = empresa.ColorFontCarnet;
                    empresaUpdate.ColorCarnet = empresa.ColorCarnet;
                    empresaUpdate.ColorFondo = empresa.ColorFondo;
                    empresaUpdate.ColorBotones = empresa.ColorBotones;
                    empresaUpdate.Instagram = empresa.Instagram;
                    empresaUpdate.Twitter = empresa.Twitter;
                    empresaUpdate.Facebook = empresa.Facebook;
                    empresaUpdate.WhatsApp = empresa.WhatsApp;
                    empresaUpdate.ColorLogin = empresa.ColorLogin;
                    empresa.Grupo = await _context.Grupos.FindAsync(empresa.Grupo.Id);
                    _context.Empresas.Update(empresa);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se modifico corretamente la Empresa.");
                    return RedirectToAction("Index", "Empresas");
                }
                else
                {
                    EmpresasViewBag();
                    return PartialView(empresa);
                }
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al modificar la Empresa. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index", "Empresas");
            }
        } 
        
        
        public ActionResult _UpdateCBU(int id)
        {
            DatosEstructura datos = _context.DatosEstructura.FirstOrDefault();
            return PartialView(datos);
        }
        [HttpPost]
        public ActionResult _UpdateCBU(DatosEstructura datosEstructura)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    DatosEstructura datosEstructuraUpdate =  _context.DatosEstructura.FirstOrDefault();
                    datosEstructuraUpdate.Telefono = datosEstructura.Telefono;
                    datosEstructuraUpdate.CUIT = datosEstructura.CUIT;
                    datosEstructuraUpdate.CBU = datosEstructura.CBU;
                    datosEstructuraUpdate.Alias = datosEstructura.Alias;
                    _context.DatosEstructura.Update(datosEstructuraUpdate);
                    _context.SaveChanges();
                    AddPageAlerts(PageAlertType.Success, "Se modifico corretamente los datos.");
                    return RedirectToAction("Index", "Empresas");
                }
                else
                {
                    EmpresasViewBag();
                    return PartialView(datosEstructura);
                }
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al modificar los datos. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index", "Empresas");
            }
        }

        private void EmpresasViewBag()
        {
            ViewBag.Grupos = _context.Grupos.Select(g => new SelectListItem() { Text = g.Nombre, Value = g.Id.ToString() });
        }
    }
}
