using Commons.Models;
using DAL.Data;
using DAL.Mobile;
using DAL.Models;
using DAL.Models.Core;
using EstanciasCore.Controllers; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;
using OfficeOpenXml;
using System.ComponentModel;
using DAL.DTOs.Reportes;
using System.Xml.Linq;
using System.Text;
using SixLabors.ImageSharp;
using static QRCoder.PayloadGenerator;
namespace EstanciasCore.Areas.Reportes.Controllers
{
    [Area("Reportes")]
    public class ClientesReportesController : EstanciasCoreController
    {

        public ClientesReportesController(EstanciasContext context) : base(context)
        {
            breadcumb.Add(new Commons.Models.Message() { DisplayName = "Datos" });
        }

        public IActionResult Index()
        {
            breadcumb.Add(new Commons.Models.Message() { DisplayName = "Pago Tarjeta Reportes" }); 
            ViewBag.Breadcrumb = breadcumb;
            return View();
        }

        [HttpPost] 
        public async Task<IActionResult> _ListadoClientes(string fechaIngresoDesdeFiltro, string fechaIngresoHastaFiltro)
        {
            try
            {
                IQueryable<Clientes> query = _context.Clientes
                                .Include(p => p.Persona)
                                .AsQueryable();

                int recordsTotal = await query.CountAsync();
                if (DateTime.TryParse(fechaIngresoDesdeFiltro, out DateTime parsedfechaIngresoDesdeFiltro))
                {
                    query = query.Where(p => p.FechaIngreso!=null && p.FechaIngreso.Date >= parsedfechaIngresoDesdeFiltro.Date);
                }

                if (DateTime.TryParse(fechaIngresoHastaFiltro, out DateTime parsedfechaIngresoHastaFiltro))
                {
                    query = query.Where(p => p.FechaIngreso!=null && p.FechaIngreso.Date <= parsedfechaIngresoHastaFiltro.Date);
                }
                ViewBag.listClientes = query.ToList();
                //ViewBag.listClientes = query.Take(15).ToList();

                ViewBag.fechaIngresoDesdeFiltro = fechaIngresoDesdeFiltro;
                ViewBag.fechaIngresoHastaFiltro = fechaIngresoHastaFiltro;

                return PartialView();
                
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<object>(),
                    error = "Se produjo un error al procesar la solicitud. " + ex.Message
                });
            }
        }

        public JsonResult DestinatariosComboJson(string q)
        {
            var items = _context.Usuarios
                .Where(x => x.Personas.NroDocumento.Contains(q) || x.Personas.NroTarjeta.Contains(q))
                .Select(x => new
                {
                    Text = $"{x.Personas.Apellido}, {x.Personas.Nombres}",
                    Value = x.Personas.Id,
                    Subtext = $"{x.UserName}",
                    Icon = "fa fa-user"
                }).Take(10).ToArray();

            return Json(items);
        }


        private bool TryParseDecimal(string montoStr, out decimal result)
        {
            result = 0;
            if (string.IsNullOrEmpty(montoStr)) return false;

            // Intentar con la cultura actual que podría usar coma
            if (decimal.TryParse(montoStr, NumberStyles.Any, CultureInfo.CurrentCulture, out result)) return true;
            // Intentar con InvariantCulture (que usa punto)
            if (decimal.TryParse(montoStr, NumberStyles.Any, CultureInfo.InvariantCulture, out result)) return true;

            return false;
        }


        [HttpGet]
        public IActionResult _Filtros()
        {
            return PartialView();
        }
        [HttpGet]
        public IActionResult _Exportar(string fechaDesdeFiltro, string fechaHastaFiltro, string estadoFiltro, string nroDocumentoFiltro, string montoFiltro)
        {
            ViewBag.fechaDesdeFiltro = fechaDesdeFiltro;
            ViewBag.fechaHastaFiltro = fechaHastaFiltro;
            ViewBag.estadoFiltro = estadoFiltro;
            ViewBag.NroDocumento = nroDocumentoFiltro;
            ViewBag.montoFiltro = montoFiltro;
            return PartialView();
        }

        public async Task<IActionResult> _ListadoPagoTarjeta(Page<PagoTarjeta> page)
        {
            var today = DateTime.Today.AddDays(-7);
            var query = _context.PagoTarjeta
                .Where(x => x.FechaComprobante >= today && x.EstadoPago != EstadoPago.Pendiente)
                .OrderByDescending(x => x.FechaComprobante);

            int totalCount = await query.CountAsync();
            if (totalCount < 1) { totalCount = 1; } 
            return Content("Esta acción (_ListadoPagoTarjeta) puede necesitar revisión o ya no ser necesaria con DataTables server-side.");
        }




        [HttpGet]
        public IActionResult ExportarExcel(string fechaIngresoDesdeFiltro, string fechaIngresoHastaFiltro)
        {
            DateTime fechaCorte;
            var memoryStream = new MemoryStream(System.IO.File.ReadAllBytes("wwwroot/Plantillas/PlantillaPagosTarjetas.xlsx"));
            using (var package = new ExcelPackage(memoryStream))
            {
                var workSheet = package.Workbook.Worksheets[1];                
                var renglones = FiltrarClientesFunction(fechaIngresoDesdeFiltro, fechaIngresoHastaFiltro);
                int linea = 1;
                int contador = 1;
                if (linea==1)
                {
                    workSheet.Cells[1, 1].Value = "Mail";
                    workSheet.Cells[1, 2].Value = "Nro Documento";
                    workSheet.Cells[1, 3].Value = "Apellido y Nombre";
                    workSheet.Cells[1, 4].Value = "Nro Tarjeta";
                    workSheet.Cells[1, 5].Value = "Fecha de Ingreso";
                }
                linea++;
                foreach (var renglon in renglones)
                {
                    contador=1;
                    if (true)
                    {
                        workSheet.Cells[linea, contador].Value = renglon.Usuario!=null ? renglon.Usuario.UserName : "Sin Datos";
                        contador++;
                    }
                    if (true)
                    {
                        workSheet.Cells[linea, contador].Value = renglon.Persona!=null ? renglon.Persona.NroDocumento : "Sin Datos";
                        contador++;
                    }
                    if (true)
                    {
                        workSheet.Cells[linea, contador].Value = renglon.Persona!=null ? renglon.Persona.GetNombreCompleto() : "Sin Datos";           
                        contador++;
                    }
                    if (true)
                    {
                        workSheet.Cells[linea, contador].Value = renglon.Persona!=null ? renglon.Persona.NroTarjeta : "Sin Datos";
                        contador++;
                    }
                    if (true)
                    {
                        workSheet.Cells[linea, contador].Value = renglon.FechaIngreso.ToString("dd/MM/yyyy"); ;
                        contador++;
                    }
                    linea = linea + 1;
                }
                DateTime fechaActual = DateTime.Now;
                string formatoFechaParaNombreArchivo = fechaActual.ToString("yyyyMMdd_HHmmss");
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Clientes_"+formatoFechaParaNombreArchivo+".xlsx");


            }
        }

        [HttpGet]
        public IActionResult ExportarCSV(string fechaIngresoDesdeFiltro, string fechaIngresoHastaFiltro)
        {
            var renglones = FiltrarClientesFunction(fechaIngresoDesdeFiltro, fechaIngresoHastaFiltro);
            var sb = new StringBuilder();

            // Cabeceras (igual que en Excel)
            // Nota: Algunos programas CSV pueden tener problemas con espacios en las cabeceras si no se encierran entre comillas.
            // Por simplicidad, las mantendré como en el Excel. Si es necesario, puedes reemplazar espacios por guiones bajos o encerrar.
            sb.AppendLine("Mail;Nro Documento;Apellido y Nombre;Nro Tarjeta;Fecha de Ingreso");

            // Agregar los datos de cada renglón
            foreach (var renglon in renglones)
            {
                string mail = renglon.Usuario != null ? renglon.Usuario.UserName : "Sin Datos";
                string nroDocumento = renglon.Persona != null ? renglon.Persona.NroDocumento : "Sin Datos";
                string apellidoYNombre = renglon.Persona != null ? renglon.Persona.GetNombreCompleto() : "Sin Datos";
                string nroTarjeta = renglon.Persona != null ? renglon.Persona.NroTarjeta : "Sin Datos";
                string fechaIngresoStr = renglon.FechaIngreso.ToString("dd/MM/yyyy");
                // Si FechaIngreso pudiera ser nullable:
                // string fechaIngresoStr = renglon.FechaIngreso.HasValue ? renglon.FechaIngreso.Value.ToString("dd/MM/yyyy") : "Sin Datos";


                // Para asegurar que los campos que podrían contener el delimitador (;) o comillas dobles
                // sean correctamente escapados en CSV, podrías usar una función auxiliar.
                // Por simplicidad, aquí se asume que los datos no contienen el delimitador.
                sb.AppendLine($"{EscapeCsvField(mail)};{EscapeCsvField(nroDocumento)};{EscapeCsvField(apellidoYNombre)};{EscapeCsvField(nroTarjeta)};{EscapeCsvField(fechaIngresoStr)}");
            }

            byte[] byteArray = Encoding.UTF8.GetBytes(sb.ToString());
            DateTime fechaActual = DateTime.Now;
            string formatoFechaParaNombreArchivo = fechaActual.ToString("yyyyMMdd_HHmmss");
            return File(byteArray, "text/plain", "Clientes_"+formatoFechaParaNombreArchivo+".csv");
        }

        private string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field)) return "";
            // Si el campo contiene comillas, punto y coma o saltos de línea, debe ir entre comillas dobles.
            // Las comillas dobles dentro del campo se escapan con otras comillas dobles.
            if (field.Contains("\"") || field.Contains(";") || field.Contains("\n") || field.Contains("\r"))
            {
                return $"\"{field.Replace("\"", "\"\"")}\"";
            }
            return field;
        }


        [HttpGet] // Asegúrate que este atributo esté si es un endpoint accesible vía GET
        public IActionResult ExportarTXT(string fechaIngresoDesdeFiltro, string fechaIngresoHastaFiltro)
        {
            var renglones = FiltrarClientesFunction(fechaIngresoDesdeFiltro, fechaIngresoHastaFiltro);
            var sb = new StringBuilder();
            char delimitador = '\t'; // Delimitador de tabulación

            // Agregar la cabecera del TXT (igual que en Excel, pero separada por tabs)
            sb.AppendLine($"Mail{delimitador}Nro Documento{delimitador}Apellido y Nombre{delimitador}Nro Tarjeta{delimitador}Fecha de Ingreso");

            // Agregar los datos de cada renglón
            foreach (var renglon in renglones)
            {
                string mail = renglon.Usuario != null ? renglon.Usuario.UserName : "Sin Datos";
                string nroDocumento = renglon.Persona != null ? renglon.Persona.NroDocumento : "Sin Datos";
                string apellidoYNombre = renglon.Persona != null ? renglon.Persona.GetNombreCompleto() : "Sin Datos";
                string nroTarjeta = renglon.Persona != null ? renglon.Persona.NroTarjeta : "Sin Datos";
                string fechaIngresoStr = renglon.FechaIngreso.ToString("dd/MM/yyyy");
                // Si FechaIngreso pudiera ser nullable:
                // string fechaIngresoStr = renglon.FechaIngreso.HasValue ? renglon.FechaIngreso.Value.ToString("dd/MM/yyyy") : "Sin Datos";

                // Construir la línea con los campos delimitados por tabulación
                sb.AppendLine($"{mail}{delimitador}{nroDocumento}{delimitador}{apellidoYNombre}{delimitador}{nroTarjeta}{delimitador}{fechaIngresoStr}");
            }
            byte[] byteArray = Encoding.UTF8.GetBytes(sb.ToString());
            DateTime fechaActual = DateTime.Now;
            string formatoFechaParaNombreArchivo = fechaActual.ToString("yyyyMMdd_HHmmss");
            return File(byteArray, "text/plain", "Clientes_"+formatoFechaParaNombreArchivo+".txt");
        }



        [HttpPost]
        public List<DAL.Models.Clientes> FiltrarClientesFunction(string fechaIngresoDesdeFiltro, string fechaIngresoHastaFiltro)
        {
            try
            {
                IQueryable<Clientes> query = _context.Clientes
                                .Include(p => p.Persona)
                                .AsQueryable();

                int recordsTotal = query.Count();
                if (DateTime.TryParse(fechaIngresoDesdeFiltro, out DateTime parsedfechaIngresoDesdeFiltro))
                {
                    query = query.Where(p => p.FechaIngreso!=null && p.FechaIngreso.Date >= parsedfechaIngresoDesdeFiltro.Date);
                }

                if (DateTime.TryParse(fechaIngresoHastaFiltro, out DateTime parsedfechaIngresoHastaFiltro))
                {
                    query = query.Where(p => p.FechaIngreso!=null && p.FechaIngreso.Date <= parsedfechaIngresoHastaFiltro.Date);
                }

                //return query.Take(15).ToList();
                return query.ToList();

            }
            catch (Exception ex)
            {
                return null;
            }
        }


    }

}