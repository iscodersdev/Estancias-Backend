using System;
using System.Collections.Generic;
using System.Linq;
using Commons.Controllers;
using Commons.Identity.Services;
using Commons.Models;
using DAL.Data;
using DAL.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EstanciasCore.Controllers
{
    public class EstanciasCoreController : BaseController
    {
        public EstanciasContext _context;
        public List<Message> breadcumb = new List<Message>();
        private IHostingEnvironment _env;
        protected UserManager<Usuario> _UserManager;

        public EstanciasCoreController(EstanciasContext context)
        {
            _context = context;
            breadcumb.Add(new Message() { DisplayName = "Inicio", URLPath = "/Home/Index", FontAwesomeIcon = "archway" });
        }
        public EstanciasCoreController(EstanciasContext context, IHostingEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public EstanciasCoreController(EstanciasContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _UserManager = userManager;
        }
        public IActionResult cierraSesion(SignInService<Usuario> signInService)
        {
            signInService.SignOutAsync().Wait();
            return RedirectToAction("Index", "Home");
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
        }
        protected ActionResult DesloguearUsuario()
        {
            return PartialView("SesionVencida");
        }

        public Empresas GetEmpresa()
        {
            var usuario = _context.Usuarios.Where(u => u.NormalizedUserName == User.Identity.Name).FirstOrDefault();
            if(usuario==null) throw new Exception("No se encontro el usuario.");

            Empresas empresa = _context.Empresas.Find(usuario.Clientes.Empresa.Id);

            if (empresa == null)
            {
                throw new Exception("Empresa inexistente.");
            }

            return empresa;
        }


    }
}