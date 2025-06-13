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
namespace EstanciasCore.Areas.Reportes.Controllers
{
    [Area("Reportes")]
    public class PagoTarjetaReportesController : EstanciasCoreController
    {

        public PagoTarjetaReportesController(EstanciasContext context) : base(context)
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
        public async Task<IActionResult> FiltrarPagosTarjeta(string fechaDesde, string fechaHasta, string estado, string nroDocumentoNotificacion, string monto, string nombreApellidoNotificacion)
        {
            try
            {
                IQueryable<PagoTarjeta> query = _context.PagoTarjeta
                                .Include(p => p.Persona)
                                .AsQueryable();

                int recordsTotal = await query.CountAsync();
                if (DateTime.TryParse(fechaDesde, out DateTime parsedFechaDesde))
                {
                    query = query.Where(p => p.FechaComprobante.HasValue && p.FechaComprobante.Value.Date >= parsedFechaDesde.Date);
                }
                if (DateTime.TryParse(fechaHasta, out DateTime parsedFechaHasta))
                {
                    query = query.Where(p => p.FechaComprobante.HasValue && p.FechaComprobante.Value.Date <= parsedFechaHasta.Date);
                }

                if (!string.IsNullOrEmpty(estado) && int.TryParse(estado, out int estadoIdInt))
                {
                    if (Enum.IsDefined(typeof(EstadoPago), estadoIdInt)) 
                    {
                        EstadoPago estadoEnum = (EstadoPago)estadoIdInt;
                        query = query.Where(p => p.EstadoPago == estadoEnum);
                    }
                }

                if (!string.IsNullOrEmpty(nroDocumentoNotificacion))
                {
                    query = query.Where(p => (p.Persona.Id==Convert.ToUInt32(nroDocumentoNotificacion)));
                }

                if (!string.IsNullOrEmpty(nombreApellidoNotificacion))
                {
                    query = query.Where(p => (p.Persona.Id==Convert.ToUInt32(nombreApellidoNotificacion)));
                }

                if (!string.IsNullOrEmpty(monto))
                {
                    if (TryParseDecimal(monto, out decimal montoDecimal))
                    {
                        query = query.Where(p => p.MontoAdeudado == montoDecimal);
                    }
                }
               
                ViewBag.listPagos = query.ToList();
               //ViewBag.listPagos = query.Take(15).ToList();

                ViewBag.fechaDesdeFiltro = fechaDesde;
                ViewBag.fechaHastaFiltro = fechaHasta;
                ViewBag.estadoFiltro = estado;
                ViewBag.NroDocumento = nroDocumentoNotificacion;
                ViewBag.montoFiltro = monto;
                ViewBag.nombreApellidoNotificacion = nombreApellidoNotificacion;

                return PartialView("_ListadoPagoTarjeta");
                
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

        public JsonResult NroTarjetaComboJson(string q)
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

        public JsonResult NombreApellidoComboJson(string q)
        {
            var items = _context.Usuarios
                .Where(x => x.Personas.Nombres.Contains(q) || x.Personas.Apellido.Contains(q))
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
        public IActionResult _Exportar(string fechaDesdeFiltro, string fechaHastaFiltro, string estadoFiltro, string nroDocumentoFiltro, string montoFiltro, string nombreApellidoNotificacion)
        {
            ViewBag.fechaDesdeFiltro = fechaDesdeFiltro;
            ViewBag.fechaHastaFiltro = fechaHastaFiltro;
            ViewBag.estadoFiltro = estadoFiltro;
            ViewBag.NroDocumento = nroDocumentoFiltro;
            ViewBag.montoFiltro = montoFiltro;
            ViewBag.nombreApellidoNotificacion = nombreApellidoNotificacion;
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
        public IActionResult ExportarExcel(string fechaDesdeFiltro, string fechaHastaFiltro, string estadoFiltro, string nroDocumentoFiltro, string montoFiltro, string nombreApellidoNotificacion)
        {
            DateTime fechaCorte;
            var memoryStream = new MemoryStream(System.IO.File.ReadAllBytes("wwwroot/Plantillas/PlantillaPagosTarjetas.xlsx"));
            using (var package = new ExcelPackage(memoryStream))
            {
                var workSheet = package.Workbook.Worksheets[1];
                var renglones = FiltrarPagosTarjetaFunction(fechaDesdeFiltro, fechaHastaFiltro, estadoFiltro, nroDocumentoFiltro, montoFiltro, nombreApellidoNotificacion);
                int linea = 1;
                int contador = 1;
                if (linea==1)
                {
                    workSheet.Cells[1, 1].Value = "Cliente";
                    workSheet.Cells[1, 2].Value = "Nro Documento";
                    workSheet.Cells[1, 3].Value = "Fecha Vencimiento";
                    workSheet.Cells[1, 4].Value = "Fecha Comprobante";
                    workSheet.Cells[1, 5].Value = "Monto Proxima Cuota";
                    workSheet.Cells[1, 6].Value = "Estado";
                }
                linea++;
                foreach (var renglon in renglones)
                {
                    contador=1;
                    if (true)
                    {
                        workSheet.Cells[linea, contador].Value = renglon.Persona.Apellido + renglon.Persona.Nombres;                       
                        contador++;
                    }
                    if (true)
                    {
                        workSheet.Cells[linea, contador].Value = renglon.Persona.NroDocumento;
                        contador++;
                    }
                    if (true)
                    {
                        workSheet.Cells[linea, contador].Value = renglon.FechaVencimiento.Value.ToString("dd/MM/yyyy");
                        contador++;
                    }
                    if (true)
                    {
                        workSheet.Cells[linea, contador].Value = renglon.FechaComprobante.Value.ToString("dd/MM/yyyy"); ;
                        contador++;
                    }
                    if (true)
                    {
                        workSheet.Cells[linea, contador].Value = renglon.MontoAdeudado.ToString().Replace(".", ",");
                        contador++;
                    }
                    if (true)
                    {
                        workSheet.Cells[linea, contador].Value = renglon.EstadoPago.ToString();
                        contador++;
                    }
                    linea = linea + 1;
                }
                DateTime fechaActual = DateTime.Now;
                string formatoFechaParaNombreArchivo = fechaActual.ToString("yyyyMMdd_HHmmss");
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Pagos tarjeta_"+formatoFechaParaNombreArchivo+".xlsx");
            

            }
        }

        [HttpGet]
        public IActionResult ExportarCSV(string fechaDesdeFiltro, string fechaHastaFiltro, string estadoFiltro, string nroDocumentoFiltro, string montoFiltro, string nombreApellidoNotificacion)
        {
            // Simulación de tu función de filtrado y la estructura de datos esperada
            // Reemplaza esto con tu lógica real y tus modelos de datos.
            var renglones = FiltrarPagosTarjetaFunction(fechaDesdeFiltro, fechaHastaFiltro, estadoFiltro, nroDocumentoFiltro, montoFiltro, nombreApellidoNotificacion);

            var sb = new StringBuilder();

            // Agregar la cabecera del CSV, delimitada por ;
            sb.AppendLine("Cliente;Nro Documento;Fecha Vencimiento;Fecha Comprobante;Monto Proxima Cuota;Estado");

            // Agregar los datos de cada renglón
            foreach (var renglon in renglones)
            {
                string cliente = $"{renglon.Persona.Apellido} {renglon.Persona.Nombres}";
                string nroDocumento = renglon.Persona.NroDocumento;
                string fechaVencimiento = renglon.FechaVencimiento.HasValue ? renglon.FechaVencimiento.Value.ToString("dd/MM/yyyy") : "";
                string fechaComprobante = renglon.FechaComprobante.HasValue ? renglon.FechaComprobante.Value.ToString("dd/MM/yyyy") : "";
                // Para el monto, si el original es decimal y usa '.', y quieres ',' en el CSV para Excel en español:
                string montoAdeudado = renglon.MontoAdeudado.ToString(System.Globalization.CultureInfo.InvariantCulture).Replace(".", ",");
                string estadoPago = renglon.EstadoPago.ToString();

                // Construir la línea con los campos delimitados por ;
                sb.AppendLine($"{cliente};{nroDocumento};{fechaVencimiento};{fechaComprobante};{montoAdeudado};{estadoPago}");
            }

            // Convertir el StringBuilder a bytes usando UTF-8 (recomendado)
            byte[] byteArray = Encoding.UTF8.GetBytes(sb.ToString());

            // Retornar el archivo CSV
            DateTime fechaActual = DateTime.Now;
            string formatoFechaParaNombreArchivo = fechaActual.ToString("yyyyMMdd_HHmmss");
            return File(byteArray, "text/csv", "PagosTarjeta_"+formatoFechaParaNombreArchivo+".csv");
        }

        public IActionResult ExportarTXT(string fechaDesdeFiltro, string fechaHastaFiltro, string estadoFiltro, string nroDocumentoFiltro, string montoFiltro, string nombreApellidoNotificacion)
        {
            var renglones = FiltrarPagosTarjetaFunction(fechaDesdeFiltro, fechaHastaFiltro, estadoFiltro, nroDocumentoFiltro, montoFiltro, nombreApellidoNotificacion);

            var sb = new StringBuilder();
            char delimitador = '\t'; // Delimitador de tabulación

            // Agregar la cabecera del TXT, delimitada por tabulación
            sb.AppendLine($"Cliente{delimitador}Nro_Documento{delimitador}Fecha_Vencimiento{delimitador}Fecha_Comprobante{delimitador}Monto_Proxima_Cuota{delimitador}Estado");

            // Agregar los datos de cada renglón
            foreach (var renglon in renglones)
            {
                string cliente = $"{renglon.Persona.Apellido} {renglon.Persona.Nombres}";
                string nroDocumento = renglon.Persona.NroDocumento;
                string fechaVencimiento = renglon.FechaVencimiento.HasValue ? renglon.FechaVencimiento.Value.ToString("dd/MM/yyyy") : "";
                string fechaComprobante = renglon.FechaComprobante.HasValue ? renglon.FechaComprobante.Value.ToString("dd/MM/yyyy") : "";
                // Para el monto, si el original es decimal y usa '.', y quieres ',' en el TXT:
                // Si prefieres el punto como separador decimal en el TXT, usa:
                // string montoAdeudado = renglon.MontoAdeudado.ToString(CultureInfo.InvariantCulture);
                string montoAdeudado = renglon.MontoAdeudado.ToString("F2", CultureInfo.GetCultureInfo("es-AR")).Replace(".", ","); // "F2" para dos decimales, cultura para formato local
                string estadoPago = renglon.EstadoPago.ToString();

                // Construir la línea con los campos delimitados por tabulación
                sb.AppendLine($"{cliente}{delimitador}{nroDocumento}{delimitador}{fechaVencimiento}{delimitador}{fechaComprobante}{delimitador}{montoAdeudado}{delimitador}{estadoPago}");
            }

            // Convertir el StringBuilder a bytes usando UTF-8 (recomendado)
            byte[] byteArray = Encoding.UTF8.GetBytes(sb.ToString());

            // Retornar el archivo TXT
            // El tipo MIME para archivos de texto plano genéricos es "text/plain"
            DateTime fechaActual = DateTime.Now;
            string formatoFechaParaNombreArchivo = fechaActual.ToString("yyyyMMdd_HHmmss");
            return File(byteArray, "text/plain", "PagosTarjeta"+formatoFechaParaNombreArchivo+".txt");
        }



        [HttpPost]
        public List<PagoTarjeta> FiltrarPagosTarjetaFunction(string fechaDesde, string fechaHasta, string estado, string nroDocumentoNotificacion, string monto, string nombreApellidoNotificacion)
        {
            try
            {
                IQueryable<PagoTarjeta> query = _context.PagoTarjeta
                                .Include(p => p.Persona)
                                .AsQueryable();

                int recordsTotal = query.Count();
                if (DateTime.TryParse(fechaDesde, out DateTime parsedFechaDesde))
                {
                    query = query.Where(p => p.FechaComprobante.HasValue && p.FechaComprobante.Value.Date >= parsedFechaDesde.Date);
                }
                if (DateTime.TryParse(fechaHasta, out DateTime parsedFechaHasta))
                {
                    query = query.Where(p => p.FechaComprobante.HasValue && p.FechaComprobante.Value.Date <= parsedFechaHasta.Date);
                }

                if (!string.IsNullOrEmpty(estado) && int.TryParse(estado, out int estadoIdInt))
                {
                    if (Enum.IsDefined(typeof(EstadoPago), estadoIdInt))
                    {
                        EstadoPago estadoEnum = (EstadoPago)estadoIdInt;
                        query = query.Where(p => p.EstadoPago == estadoEnum);
                    }
                }

                if (!string.IsNullOrEmpty(nroDocumentoNotificacion))
                {
                    query = query.Where(p => (p.Persona.Id==Convert.ToUInt32(nroDocumentoNotificacion)));
                }

                if (!string.IsNullOrEmpty(nombreApellidoNotificacion))
                {
                    query = query.Where(p => (p.Persona.Id==Convert.ToUInt32(nombreApellidoNotificacion)));
                }

                if (!string.IsNullOrEmpty(monto))
                {
                    if (TryParseDecimal(monto, out decimal montoDecimal))
                    {
                        query = query.Where(p => p.MontoAdeudado == montoDecimal);
                    }
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