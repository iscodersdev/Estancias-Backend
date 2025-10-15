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
using DAL.DTOs.Servicios;
using iText.Html2pdf;
using System.IO;
using OfficeOpenXml.FormulaParsing.Utilities;
using System.Threading.Tasks;

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
            _resumen.GenerarResumenTarjetas();
            AddPageAlerts(PageAlertType.Success, $"Bienvenido {User.Identity.Name}!");        
            var usuario = _context.Usuarios.FirstOrDefault(x => x.Email == User.Identity.Name);
            ViewBag.title1 = "Socios Con App";
            ViewBag.title4 = "Cantidad Socios Nuevos del Mes";
            
            @ViewBag.Uno = _context.Clientes.Count().ToString();
            @ViewBag.Cuatro = _context.Clientes.Where(x => x.FechaIngreso.Date >= DateTime.Today.AddDays(-30).Date).Count();
           
            return View();
        }

        public async Task<IActionResult> DescargarResumen(string dni)
        {
            Usuario usuarioLocal = _context.Usuarios.Where(x => x.Personas.NroDocumento == dni).FirstOrDefault();
            DateTime fecha = DateTime.Now;
            var movimientos = _datosTarjeta.ConsultarMovimientos("APPESTANCIAS", "appcpe01", dni, Convert.ToInt32(usuarioLocal.Personas.NroTarjeta), 100, 1).Result;
            var datosResumen = _datosTarjeta.CuotasDetallesResumen(movimientos, fecha).Result;
            Periodo periodo = _context.Periodo.Where(x => x.FechaDesde <= fecha && x.FechaHasta >= fecha).FirstOrDefault();

            UsuarioParaProcesarDTO usuarioDTO = new UsuarioParaProcesarDTO()
            {
                NroDocumento = usuarioLocal.Personas.NroDocumento,
                NombreCompleto = usuarioLocal.Personas.GetNombreCompleto(),
                Id = usuarioLocal.Id,
                UserName = User.Identity.Name,
                NroTarjeta = usuarioLocal.Personas.NroTarjeta
            };

            var datosParaResumenDTO = _datosTarjeta.PrepararDatosResumen(movimientos, datosResumen, periodo, usuarioDTO).Result;

            var html = await _datosTarjeta.RenderViewToStringAsync("ResumenBancarioTemplate", datosParaResumenDTO);

            byte[] pdfBytesPDF;
            using (var memoryStream = new MemoryStream())
            {
                HtmlConverter.ConvertToPdf(html, memoryStream);
                pdfBytesPDF = memoryStream.ToArray();
            }

            return File(pdfBytesPDF, "application/pdf", "ResumenBancario.pdf");
        }

        public async Task<IActionResult> DescargarResumenHtml(string dni)
        {
            Usuario usuarioLocal = _context.Usuarios.Where(x => x.Personas.NroDocumento == dni).FirstOrDefault();
            //DateTime fecha = DateTime.Now;


            DateTime fechaMesActualCuotas = new DateTime(2025, 10, 01);
            int diasEnMes = DateTime.DaysInMonth(fechaMesActualCuotas.Year, fechaMesActualCuotas.Month);

            //Fecha para Punitorios
            if (fechaMesActualCuotas.Day > 15)
            {
                DateTime fechaPunitorios = new DateTime(fechaMesActualCuotas.Year, fechaMesActualCuotas.Month, diasEnMes);
            }
            else
            {
                DateTime fechaPunitorios = new DateTime(fechaMesActualCuotas.Year, fechaMesActualCuotas.Month, 15);
            }

            DateTime fechaActualCuotas = new DateTime(fechaMesActualCuotas.Year, fechaMesActualCuotas.Month, diasEnMes);
            DateTime fechaActualCuotasProximo = fechaActualCuotas.AddMonths(1);


            var movimientos = _datosTarjeta.ConsultarMovimientos("APPESTANCIAS", "appcpe01", dni, Convert.ToInt64(usuarioLocal.Personas.NroTarjeta), 100, 1).Result;

            var datosResumen = _datosTarjeta.CuotasDetallesResumen(movimientos, fechaActualCuotas).Result;

            var datosResumenConPunitorios = _datosTarjeta.CalcularPunitoriosResumen(datosResumen).Result;

            Periodo periodo = _context.Periodo.Where(x => x.FechaVencimiento.Date==new DateTime(2025, 10, 15).Date).FirstOrDefault();

            UsuarioParaProcesarDTO usuarioDTO = new UsuarioParaProcesarDTO()
            {
                NroDocumento = usuarioLocal.Personas.NroDocumento,
                NombreCompleto = usuarioLocal.Personas.GetNombreCompleto(),
                Id = usuarioLocal.Id,
                UserName = usuarioLocal.UserName,
                NroTarjeta = usuarioLocal.Personas.NroTarjeta
            };

            var datosParaResumenDTO = _datosTarjeta.PrepararDatosResumen(movimientos, datosResumenConPunitorios, periodo, usuarioDTO).Result;

            var html = await _datosTarjeta.RenderViewToStringAsync("ResumenBancarioTemplate", datosParaResumenDTO);

            return View("ResumenBancarioTemplate", datosParaResumenDTO);
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