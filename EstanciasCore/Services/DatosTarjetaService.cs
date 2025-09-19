using DAL.Data;
using DAL.DTOs.Reportes;
using DAL.DTOs.Servicios;
using DAL.Mobile;
using DAL.Models;
using EstanciasCore.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;  
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
namespace EstanciasCore.Services
{
    public class DatosTarjetaService : IDatosTarjetaService
    {
        private EstanciasContext _context { get; set; }
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider; 
        private readonly ILogger<DatosTarjetaService> _logger; 
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl = "https://sistema.cpecreditos.com.ar/Loan/ServiciosAPI/";

        // Tasa Nominal Anual (TNA) del 98% (Persistir valor en la Base de Datos)
        private const decimal TNA = 0.98m;


        public DatosTarjetaService(IConfiguration configuration, EstanciasContext context, IRazorViewEngine razorViewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider, ILogger<DatosTarjetaService> logger)
        {
            _context=context;
            _razorViewEngine = razorViewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _httpClient = new HttpClient();
        }

        private async Task<string> ObtenerDatos(string usuario, string clave, long documento, long numeroTarjeta, long cantidadMovimientos)
        {
            string soapRequest =
               $@"<?xml version=""1.0"" encoding=""utf-8""?>
                <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                  <soap:Body>
                    <TarjetaRequest xmlns=""http://tempuri.org/"">
                      <TarjetaRequest>
                        <usuario>{usuario}</usuario>
                        <clave>{clave}</clave>
                        <documento>{documento}</documento>
                        <numeroTarjeta>{numeroTarjeta}</numeroTarjeta>
                        <cantidadMovimientos>{cantidadMovimientos}</cantidadMovimientos>
                      </TarjetaRequest>
                    </TarjetaRequest>
                  </soap:Body>
                </soap:Envelope>";

            string url = "http://sistema.cpecreditos.com.ar/Loan/ServiciosWeb/TarjetaWebService.asmx";
            string action = "http://tempuri.org/TarjetaObtenerDatos";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers.Add("SOAPAction", action);
            request.ContentType = "text/xml; charset=utf-8";
            request.Method = "POST";

            using (Stream stream = await request.GetRequestStreamAsync())
            {
                byte[] data = Encoding.UTF8.GetBytes(soapRequest);
                await stream.WriteAsync(data, 0, data.Length);
            }

            string soapResult;
            using (WebResponse response = await request.GetResponseAsync())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    soapResult = await reader.ReadToEndAsync();
                }
            }

            return soapResult;
        }

        public async Task<CombinedData> ConsultarMovimientos(string usuario, string clave, string documento, long numeroTarjeta, long cantidadMovimientos, int tipomovimientotarjeta)
        {
            var cliente = new TarjetaObtenerDatosClient();

            string soapResponse = await ObtenerDatos(usuario, clave, Convert.ToInt32(documento), numeroTarjeta, cantidadMovimientos);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(soapResponse);

            XmlNodeList DatosNodes = xmlDoc.GetElementsByTagName("TarjetaObtenerDatosResult");
            ListaMovimientoTarjetaDTO datos = new ListaMovimientoTarjetaDTO();

            XDocument doc = XDocument.Parse(soapResponse);
            XNamespace ns = "http://tempuri.org/";

            #region Guardar Archivo XML (opcional)
            ////------------------------------------//
            //// --- Para guardar el archivo de forma segura ---

            //// 1. Obtiene la ruta a la carpeta del Escritorio del usuario actual.
            //string rutaEscritorio = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            //// 2. Define el nombre del archivo.
            //string nombreArchivo = "respuesta_tarjeta.xml";

            //// 3. Combina la ruta del escritorio y el nombre del archivo. 
            ////    Path.Combine se asegura de que la ruta sea correcta.
            //string rutaCompleta = Path.Combine(rutaEscritorio, nombreArchivo);

            //// 4. Escribe el contenido en el archivo en el Escritorio.
            // File.WriteAllText(rutaCompleta, soapResponse);
            ////------------------------------------//
            #endregion

            Detail detalles = new Detail();
            var resultadosConsulta = doc.Descendants(ns + "resultadoServicioWeb")
                                .Select(m => new Detail()
                                {
                                    Resultado = m.Element(ns + "resultado").Value,
                                    Mensaje = m.Element(ns + "mensaje").Value,
                                }).FirstOrDefault();

            CombinedData combinedResults = new CombinedData()
            {
                Detalle = new Detail()
                {
                    Resultado = "ERROR",
                    Mensaje = "Error al traer los movimientos."
                }
            };

            if (resultadosConsulta.Resultado=="EXITO")
            {
                detalles = doc.Descendants(ns + "TarjetaObtenerDatosResult")
                                               .Select(m => new Detail()
                                               {
                                                   Documento = m.Element(ns + "documento").Value,
                                                   Nombre = m.Element(ns + "nombre").Value,
                                                   Direccion = m.Element(ns + "direccion").Value,
                                                   MontoAdeudado = m.Element(ns + "montoAdeudado").Value,
                                                   MontoDisponible = m.Element(ns + "MontoDisponible").Value,
                                                   ProximaFechaPago = m.Element(ns + "proximaFechaPago").Value != "0:00:00" ? DateTime.ParseExact(m.Element(ns + "proximaFechaPago").Value, "dd/MM/yyyy", null) : DateTime.Parse("11/11/1111"),
                                                   TotalProximaCuota = m.Element(ns + "totalProximaCuota").Value,
                                                   FechaPagoProximaCuota = m.Element(ns + "fechaPagoProximaCuota").Value != "0:00:00" ? DateTime.ParseExact(m.Element(ns + "fechaPagoProximaCuota").Value, "dd/MM/yyyy", null) : DateTime.Parse("11/11/1111"),
                                               }).FirstOrDefault();

                detalles.Resultado = resultadosConsulta.Resultado;

                XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
                var movements = doc.Descendants(ns + "Movimiento")
                     .Where(m => m.Attribute(xsi + "nil") == null || m.Attribute(xsi + "nil").Value != "true")
                     .Select(m => new MovementDetail
                     {
                         Fecha = DateTime.ParseExact(m.Element(ns + "fecha")?.Value, "dd/MM/yyyy", null),
                         Descripcion = m.Element(ns + "descripcion")?.Value,
                         Monto = m.Element(ns + "monto")?.Value,
                         Recargo = m.Element(ns + "recargo")?.Value
                     });


                var detallesSolicitud = doc.Descendants(ns + "DetalleSolicitud")
                                       .Select(d => new SolicitudDetail()
                                       {
                                           NumeroSolicitud = d.Element(ns + "numeroSolicitud").Value,
                                           NombreComercio = d.Element(ns + "nombreComercio").Value,
                                           DetallesCuota = d.Descendants(ns + "DetalleCuota").Select(e => new DetalleCuota()
                                           {
                                               Fecha = DateTime.ParseExact(e.Element(ns + "fechaCuota").Value, "yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"),
                                               NumeroCuota = e.Element(ns + "numeroCuota").Value,
                                               Monto = e.Element(ns + "montoCuota").Value
                                           }).ToList()
                                       });

                combinedResults = new CombinedData()
                {
                    Detalle = detalles,
                    Movimientos = movements.ToList(),
                    DetallesSolicitud = detallesSolicitud.ToList(),
                };
            }
            return combinedResults;
        }

        public async Task<decimal> CalcularMontoCuota(CombinedData datosMovimientos, DateTime fechaActualCuotas)
        {
            List<MovimientoTarjetaDTO> comprasAgrupadas = new List<MovimientoTarjetaDTO>();
            CultureInfo.CurrentCulture = new CultureInfo("es-AR");

            comprasAgrupadas = datosMovimientos.Movimientos.Where(x => x.Descripcion=="PAGOS DE CUOTA REGULAR")
                        .GroupBy(m => new { m.Descripcion, m.Fecha })
                        .Select(g => new MovimientoTarjetaDTO
                        {
                            Monto =  (g.Sum(m => Convert.ToDecimal(m.Monto.Replace(",", ".")) + Convert.ToDecimal(m.Recargo.Replace(",", "."))).ToString().Replace(".", ","))==null ? g.Sum(m => Convert.ToDecimal(m.Monto.Replace(",", "."))).ToString().Replace(".", ",") : (g.Sum(m => Convert.ToDecimal(m.Monto.Replace(",", ".")) + Convert.ToDecimal(m.Recargo.Replace(",", "."))).ToString().Replace(".", ",")),
                            TipoMovimiento = g.Key.Descripcion,
                            Fecha = g.Key.Fecha.Date.ToString("dd/MM/yyyy")
                        })
                        .ToList();

            comprasAgrupadas.AddRange(datosMovimientos.Movimientos.Where(x => x.Descripcion!="PAGOS DE CUOTA REGULAR")
            .Select(g => new MovimientoTarjetaDTO
            {
                Monto = g.Monto.Replace(",", ".").ToString().Replace(".", ","),
                TipoMovimiento = g.Descripcion,
                Fecha = g.Fecha.Date.ToString("dd/MM/yyyy")
            }).ToList());

            //Montos a Pagar con Deuda pero sin Punitorios
            var totalDetallesCuota = datosMovimientos.DetallesSolicitud
            .Where(result => result?.DetallesCuota != null)
            .SelectMany(result => result.DetallesCuota)
            .Where(detalle => (common.ConvertirFecha(detalle.Fecha) <= fechaActualCuotas))
            .Select(e => new { monto = e.Monto }).ToList();

            var totalMontoCuota = totalDetallesCuota.Sum(e => Convert.ToDecimal(e.monto.Replace(".", ",")));
            return totalMontoCuota;
        }

        public async Task<List<ResultadoCuotas>> CalcularMontoCuotaDetalles(CombinedData datosMovimientos, DateTime fechaActualCuotas)
        {
            List<MovimientoTarjetaDTO> comprasAgrupadas = new List<MovimientoTarjetaDTO>();
            CultureInfo.CurrentCulture = new CultureInfo("es-AR");

            comprasAgrupadas = datosMovimientos.Movimientos.Where(x => x.Descripcion=="PAGOS DE CUOTA REGULAR")
                        .GroupBy(m => new { m.Descripcion, m.Fecha })
                        .Select(g => new MovimientoTarjetaDTO
                        {
                            Monto =  (g.Sum(m => Convert.ToDecimal(m.Monto.Replace(",", ".")) + Convert.ToDecimal(m.Recargo.Replace(",", "."))).ToString().Replace(".", ","))==null ? g.Sum(m => Convert.ToDecimal(m.Monto.Replace(",", "."))).ToString().Replace(".", ",") : (g.Sum(m => Convert.ToDecimal(m.Monto.Replace(",", ".")) + Convert.ToDecimal(m.Recargo.Replace(",", "."))).ToString().Replace(".", ",")),
                            TipoMovimiento = g.Key.Descripcion,
                            Fecha = g.Key.Fecha.Date.ToString("dd/MM/yyyy")
                        })
                        .ToList();

            comprasAgrupadas.AddRange(datosMovimientos.Movimientos.Where(x => x.Descripcion!="PAGOS DE CUOTA REGULAR")
            .Select(g => new MovimientoTarjetaDTO
            {
                Monto = g.Monto.Replace(",", ".").ToString().Replace(".", ","),
                TipoMovimiento = g.Descripcion,
                Fecha = g.Fecha.Date.ToString("dd/MM/yyyy")
            }).ToList());

            //Montos a Pagar con Deuda pero sin Punitorios
            //var totalDetallesCuota = datosMovimientos.DetallesSolicitud
            //.Where(result => result?.DetallesCuota != null)
            //.SelectMany(result => result.DetallesCuota)
            //.Where(detalle => (common.ConvertirFecha(detalle.Fecha) <= fechaActualCuotas))
            //.ToList();

            List<ResultadoCuotas> totalDetallesCuota = datosMovimientos.DetallesSolicitud
              .Where(result => result?.DetallesCuota != null)
              .SelectMany(result => result.DetallesCuota
                .Where(detalle => common.ConvertirFecha(detalle.Fecha) <= fechaActualCuotas)
                .Select(detalle => new ResultadoCuotas
                {
                    Descripcion = result.NombreComercio,
                    Codigo = result.NumeroSolicitud,
                    Fecha = detalle.Fecha,
                    NumeroCuota = detalle.NumeroCuota,
                    Monto = detalle.Monto
                })).OrderBy(e=>e.Fecha).OrderBy(d=>d.Codigo)
              .ToList();

            return totalDetallesCuota;
        }

        public class ResultadoCuotas
        {
            // Esta clase representa el objeto plano que necesitas
            public string Codigo { get; set; }
            public string Descripcion { get; set; }
            public string Fecha { get; set; }
            public string NumeroCuota { get; set; }
            public string Monto { get; set; }
        }

        public async Task<decimal> CalcularMontoProximaCuota(CombinedData datosMovimientos, DateTime fechaActualCuotasProximo)
        {
            DateTime fechaInicio = new DateTime(fechaActualCuotasProximo.Year, fechaActualCuotasProximo.Month, 1);
            List<MovimientoTarjetaDTO> comprasAgrupadas = new List<MovimientoTarjetaDTO>();
            CultureInfo.CurrentCulture = new CultureInfo("es-AR");

            var totalDetallesCuotaProximoMes = datosMovimientos.DetallesSolicitud
                   .Where(result => result?.DetallesCuota != null)
                   .SelectMany(result => result.DetallesCuota)
                   .Where(detalle => (common.ConvertirFecha(detalle.Fecha) >= fechaInicio && common.ConvertirFecha(detalle.Fecha)<=fechaActualCuotasProximo))
                   .Select(e => new { monto = e.Monto }).ToList();

            var totalMontoProximaCuota = totalDetallesCuotaProximoMes.Sum(e => Convert.ToDecimal(e.monto.Replace(".", ",")));
            return totalMontoProximaCuota;
        }


        public async Task<List<MovimientoTarjetaDTO>> ObtieneUltimosMovimientos(CombinedData datosMovimientos, int top)
        {
            List<MovimientoTarjetaDTO> comprasAgrupadas = new List<MovimientoTarjetaDTO>();
            CultureInfo.CurrentCulture = new CultureInfo("es-AR");

            comprasAgrupadas = datosMovimientos.Movimientos.Where(x => x.Descripcion=="PAGOS DE CUOTA REGULAR")
                        .GroupBy(m => new { m.Descripcion, m.Fecha })
                        .Select(g => new MovimientoTarjetaDTO
                        {
                            //Monto =  (g.Sum(m => Convert.ToDecimal(m.Monto.Replace(",", ".")) + Convert.ToDecimal(m.Recargo.Replace(",", "."))).ToString().Replace(".", ","))==null ? g.Sum(m => Convert.ToDecimal(m.Monto.Replace(",", "."))).ToString().Replace(".", ",") : (g.Sum(m => Convert.ToDecimal(m.Monto.Replace(",", ".")) + Convert.ToDecimal(m.Recargo.Replace(",", "."))).ToString().Replace(".", ",")),
                            Monto =  ((g.Sum(m => Convert.ToDecimal(m.Monto)))+(g.Sum(m => Convert.ToDecimal(m.Recargo)))).ToString(),
                            TipoMovimiento = g.Key.Descripcion,
                            Fecha = g.Key.Fecha.Date.ToString("dd/MM/yyyy")
                        })
                        .ToList();

            comprasAgrupadas.AddRange(datosMovimientos.Movimientos.Where(x => x.Descripcion!="PAGOS DE CUOTA REGULAR")
            .Select(g => new MovimientoTarjetaDTO
            {
                Monto = g.Monto.Replace(",", ".").ToString().Replace(".", ","),
                TipoMovimiento = g.Descripcion,
                Fecha = g.Fecha.Date.ToString("dd/MM/yyyy")
            }).ToList());

            List<MovimientoTarjetaDTO> MovientosOrdenadosPorFecha = comprasAgrupadas.OrderByDescending(x => common.ConvertirFecha(x.Fecha)).Take(Convert.ToInt32(20)).ToList();
            return MovientosOrdenadosPorFecha;
        }

        public async Task<decimal> CalcularPunitorios(List<SolicitudDetail> cuotas)
        {
            int anioActual = DateTime.Now.Year; // -> 2025
            int mesActual = DateTime.Now.Month; // -> 8

            // El servicio calcula automáticamente fa fecha de corte para los Punitorios.
            DateTime fechaCorteMora = ObtenerFechaDeCalculoCorrecta();

            // 1. Calcular la tasa diaria
            int diasDelAnio = DateTime.IsLeapYear(fechaCorteMora.Year) ? 366 : 365;
            decimal tasaDiaria = (TNA / diasDelAnio);

            List<DetalleCuota> cuotasEnMora = cuotas
                .SelectMany(result => result.DetallesCuota)
                .Where(c => common.ConvertirFecha(c.Fecha) <= fechaCorteMora.Date)
                .ToList();

            if (!cuotasEnMora.Any())
            {
                //No se encontraron cuotas en mora para la fecha de corte especificada.
                return 0;
            }

            decimal totalPunitorios = 0;

            // 3. Calcular el punitorio para cada cuota vencida
            foreach (var cuota in cuotasEnMora)
            {
                // Se calculan los días de mora desde el vencimiento hasta la fecha de cálculo
                int diasEnMora = (int)(fechaCorteMora.Date -  common.ConvertirFecha(cuota.Fecha)).TotalDays;

                // Se multiplica el monto por la tasa diaria y por los días en mora
                decimal monto = Convert.ToDecimal(cuota.Monto.Replace(".", ","));
                decimal punitorioCuota = monto * tasaDiaria * diasEnMora;

                totalPunitorios += punitorioCuota;
            }
            return totalPunitorios;
        }

        public async Task<decimal> CalcularPunitoriosResumen(List<SolicitudDetail> cuotas)
        {
            int anioActual = DateTime.Now.Year; // -> 2025
            int mesActual = DateTime.Now.Month; // -> 8

            const int diaDeCalculo = 26;
            DateTime fechaDeCalculo = new DateTime(anioActual, mesActual, diaDeCalculo);

            int diasDelAnio = DateTime.IsLeapYear(fechaDeCalculo.Year) ? 366 : 365;
            decimal tasaDiaria = (TNA / diasDelAnio);
            DateTime fechaCorteMora = fechaDeCalculo.AddMonths(-1);

            List<DetalleCuota> cuotasEnMora = cuotas
                .SelectMany(result => result.DetallesCuota)
                .Where(c => common.ConvertirFecha(c.Fecha) <= fechaCorteMora.Date)
                .ToList();

            if (!cuotasEnMora.Any())
            {
                return 0;
            }

            decimal totalPunitorios = 0;

            foreach (var cuota in cuotasEnMora)
            {
                int diasEnMora = (int)(fechaDeCalculo.Date - common.ConvertirFecha(cuota.Fecha)).TotalDays;
                decimal monto = Convert.ToDecimal(cuota.Monto.Replace(".", ","));
                decimal punitorioCuota = monto * tasaDiaria * diasEnMora;

                totalPunitorios += punitorioCuota;
            }
            return totalPunitorios;
        }
         
        public async Task<TempalteResumenDTO> PrepararDatosDTO(CombinedData datosMovimientos, Periodo periodo, UsuarioParaProcesarDTO usuario)
        {
            TempalteResumenDTO tempalteResumenDTO = new TempalteResumenDTO();
            List<DetallesCuotasResumenDTO>  detallesCuotasResumenDTO = new List<DetallesCuotasResumenDTO>();
            ResumenTarjeta resumenTarjeta = _context.ResumenTarjeta.Where(x=>x.Usuario.Personas.NroDocumento==datosMovimientos.Detalle.Documento).OrderByDescending(r => r.Id).FirstOrDefault();
            tempalteResumenDTO.SaldoAnterior = resumenTarjeta!=null?(resumenTarjeta.Monto+resumenTarjeta.MontoAdeudado):0;
            tempalteResumenDTO.SaldoActual = 0; 
            tempalteResumenDTO.SaldoTotal = 0; 
            tempalteResumenDTO.Pagos = 0; 
            tempalteResumenDTO.Intereses = 0; 
            tempalteResumenDTO.Impuestos = 0; 
            tempalteResumenDTO.Nombre = datosMovimientos.Detalle.Nombre; 
            tempalteResumenDTO.NroDocumento =  datosMovimientos.Detalle.Documento; 
            tempalteResumenDTO.Mail =  usuario.UserName; 
            tempalteResumenDTO.NroSocio = usuario.Id; 
            tempalteResumenDTO.NroTarjeta = usuario.NroTarjeta; 
            tempalteResumenDTO.Domicilio =  datosMovimientos.Detalle.Direccion; 
            tempalteResumenDTO.PeriodoDesde = periodo.FechaDesde.ToString("dd/MMM/yyyy"); 
            tempalteResumenDTO.PeriodoHasta = periodo.FechaHasta.ToString("dd/MMM/yyyy");
            tempalteResumenDTO.Vencimiento = periodo.FechaVencimiento.ToString("dd/MMM/yyyy");



            var totalDetallesCuota = datosMovimientos.DetallesSolicitud
                .Where(result => result?.DetallesCuota != null)
                .SelectMany(result => result.DetallesCuota,
                    (result, detalle) => new DetallesCuotasResumenDTO
                    {
                        NroSolicitud = result.NumeroSolicitud,
                        Fecha = detalle.Fecha,
                        Concepto = result.NombreComercio,
                        NroCuota = detalle.NumeroCuota,
                        Monto = Convert.ToDecimal(detalle.Monto.Replace(".", ",")),
                    })
                .Where(detalle => (common.ConvertirFecha(detalle.Fecha) >= periodo.FechaDesde) && (common.ConvertirFecha(detalle.Fecha) <= periodo.FechaHasta));

            foreach (var item in totalDetallesCuota)
            {
                var cuota = datosMovimientos.DetallesSolicitud.Where(x => x.NumeroSolicitud==item.NroSolicitud).Select(e => e.DetallesCuota.Max(c => c.NumeroCuota)).FirstOrDefault();
                DetallesCuotasResumenDTO detalle = item;
                detalle.TotalDeCuotas = cuota;
                detallesCuotasResumenDTO.Add(detalle);
            }
            tempalteResumenDTO.DetallesCuotas = detallesCuotasResumenDTO;

            var cuotasVencidas = datosMovimientos.DetallesSolicitud
                    .Where(result => result?.DetallesCuota != null)
                    .SelectMany(result => result.DetallesCuota)
                    .Where(detalle => (common.ConvertirFecha(detalle.Fecha) < periodo.FechaDesde))
                    .Select(e => new { monto = Convert.ToDecimal(e.Monto.Replace(".", ",")) }).ToList();

            CultureInfo.CurrentCulture = new CultureInfo("es-AR");
            decimal montoPunitoriosTotal = await CalcularPunitoriosResumen(datosMovimientos.DetallesSolicitud);

            decimal saldo = totalDetallesCuota.Sum(e => e.Monto);
            tempalteResumenDTO.SaldoAnterior = montoPunitoriosTotal;
            tempalteResumenDTO.SaldoActual = saldo;
            tempalteResumenDTO.SaldoTotal = saldo+ montoPunitoriosTotal;
            return tempalteResumenDTO;
        }

        public async Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model)
        {
            var actionContext = GetActionContext();
            var view = FindView(actionContext, viewName);

            using (var output = new StringWriter())
            {
                var viewContext = new ViewContext(
                    actionContext,
                    view,
                    new ViewDataDictionary<TModel>(
                        metadataProvider: new EmptyModelMetadataProvider(),
                        modelState: new ModelStateDictionary())
                    {
                        Model = model
                    },
                    new TempDataDictionary(
                        actionContext.HttpContext,
                        _tempDataProvider),
                    output,
                    new HtmlHelperOptions());

                await view.RenderAsync(viewContext);

                return output.ToString();
            }
        }


        #region Extra 
            private IView FindView(ActionContext actionContext, string viewName)
            {
                var getViewResult = _razorViewEngine.GetView(executingFilePath: null, viewPath: viewName, isMainPage: true);
                if (getViewResult.Success)
                {
                    return getViewResult.View;
                }

                var findViewResult = _razorViewEngine.FindView(actionContext, viewName, isMainPage: true);
                if (findViewResult.Success)
                {
                    return findViewResult.View;
                }

                throw new ArgumentNullException($"No se pudo encontrar la vista {viewName}. Se buscó en las siguientes ubicaciones: {string.Join(", ", findViewResult.SearchedLocations)}");
            }

            private ActionContext GetActionContext()
            {
                var httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };
                return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            }

            /// <summary>
            /// Calcula el último día de un mes y año específicos.
            /// </summary>
            /// <param name="anio">El año.</param>
            /// <param name="mes">El mes.</param>
            /// <returns>El último día hábil del mes.</returns>
            private DateTime ObtenerUltimoDia(int anio, int mes)
            {
                DateTime ultimoDia = new DateTime(anio, mes, DateTime.DaysInMonth(anio, mes));
                return ultimoDia;
            }

            public DateTime ObtenerFechaDeCalculoCorrecta()
            {
                DateTime hoy = DateTime.Now;

                if (hoy.Day <= 15)
                {
                    // Si es antes del día 15, la fecha de cálculo es el 15 del mes actual.
                    return new DateTime(hoy.Year, hoy.Month, 15);
                }
                else
                {
                    // Si es día 15 o posterior, la fecha de cálculo es el último día del mes actual.
                    return ObtenerUltimoDia(hoy.Year, hoy.Month);
                }
            }
        #endregion


    }


}