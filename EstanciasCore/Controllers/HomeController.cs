using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Commons.Identity.Services;
using DAL.Data;
using DAL.Models;
using System.Linq;
using System;
using System.Globalization;
using EstanciasCore.Interface;

namespace EstanciasCore.Controllers
{
    public class HomeController : EstanciasCoreController
    {
        private readonly SignInManager<Usuario> _signInManager;
        private readonly UserService<Usuario> _userManager;
        private readonly IResumenTarjetaService _resumen;
        private readonly IDatosTarjetaService _datosTarjeta;
        public HomeController(EstanciasContext context, UserService<Usuario> userManager, SignInManager<Usuario> signInManager, IResumenTarjetaService resumen, IDatosTarjetaService datosTarjeta) : base(context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _resumen = resumen;
            _datosTarjeta=datosTarjeta;
        }
        public IActionResult Index()
        {
            //_resumen.GenerarResumenTarjetas();
            DateTime fecha = DateTime.Now;
            var movimientos = _datosTarjeta.ConsultarMovimientos("APPESTANCIAS", "appcpe01", "30463400", 0000010012018003, 100, 1).Result;
            var datosTarjeta = _datosTarjeta.CalcularMontoCuotaDetalles(movimientos, fecha).Result;

            AddPageAlerts(PageAlertType.Success, $"Bienvenido {User.Identity.Name}!");        
            var usuario = _context.Usuarios.FirstOrDefault(x => x.Email == User.Identity.Name);
            ViewBag.title1 = "Socios Con App";
            ViewBag.title4 = "Cantidad Socios Nuevos del Mes";
            
                @ViewBag.Uno = _context.Clientes.Count().ToString();
                @ViewBag.Cuatro = _context.Clientes.Where(x => x.FechaIngreso.Date >= DateTime.Today.AddDays(-30).Date).Count();
           
            return View();

        }
        public IActionResult MailRegistro()
        {
            return View("MailRegistro");
        }
		public IActionResult MailRecuperaPassword()
		{
			return View("MailRecuperaPassword");
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new DAL.Models.ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        

    }
}