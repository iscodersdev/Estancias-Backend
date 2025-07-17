using DAL.Data;
using DAL.DTOs.Reportes;
using DAL.DTOs.Servicios;
using DAL.Mobile;
using DAL.Models;
using DAL.Models.Core;
using EstanciasCore.Controllers;
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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Serilog.Core;
using System;  
using System.Collections.Generic;
using System.Diagnostics;
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
        private IConfiguration _Configuration { get; }
        private EstanciasContext _context { get; set; }
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider; 
        private readonly ILogger<DatosTarjetaService> _logger; 
        private readonly HttpClient _httpClient;
        // IMPORTANTE: Reemplaza esta URL por la URL base real de tu API.
        private readonly string _apiBaseUrl = "https://loandivinf-serviciosapi.loancloudweb.com/";


        public DatosTarjetaService(IConfiguration configuration, EstanciasContext context, IRazorViewEngine razorViewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider, ILogger<DatosTarjetaService> logger)
        {
            _Configuration = configuration;
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

            //var movi = ObtenerCreditosAsync(usuario, clave, documento);

            var cliente = new TarjetaObtenerDatosClient();

            string soapResponse = await ObtenerDatos(usuario, clave, Convert.ToInt32(documento), numeroTarjeta, cantidadMovimientos);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(soapResponse);

            XmlNodeList DatosNodes = xmlDoc.GetElementsByTagName("TarjetaObtenerDatosResult");
            ListaMovimientoTarjetaDTO datos = new ListaMovimientoTarjetaDTO();

            XDocument doc = XDocument.Parse(soapResponse);
            XNamespace ns = "http://tempuri.org/";

            #region Guardar Archivo XML (opcional)
            //------------------------------------//
            // --- Para guardar el archivo de forma segura ---

            // 1. Obtiene la ruta a la carpeta del Escritorio del usuario actual.
            //string rutaEscritorio = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            // 2. Define el nombre del archivo.
            //string nombreArchivo = "respuesta_tarjeta.xml";

            // 3. Combina la ruta del escritorio y el nombre del archivo. 
            //    Path.Combine se asegura de que la ruta sea correcta.
            //string rutaCompleta = Path.Combine(rutaEscritorio, nombreArchivo);

            // 4. Escribe el contenido en el archivo en el Escritorio.
            // File.WriteAllText(rutaCompleta, soapResponse);
            //------------------------------------//
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

        public async Task<DatosParaResumenDTO> PrepararDatosDTO(int periodoId, string usuarioId)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == usuarioId);
            var periodoActual = await _context.Periodo.FindAsync(periodoId);
            if (usuario == null || periodoActual == null) return null;

            var movimientosDelPeriodo = await _context.MovimientoTarjeta
                .Where(m => m.Usuario.Id == usuarioId && m.Periodo.Id == periodoId)
                .OrderBy(m => m.Fecha).ToListAsync();

            // Lógica de ejemplo para calcular saldos. Debes adaptarla a tu negocio.
            decimal saldoAnterior = _context.MovimientoTarjeta.Where(m => m.Usuario.Id == usuarioId && m.Periodo.FechaDesde < periodoActual.FechaDesde && m.Pagado==false).Sum(x => x.Monto);
            decimal pagos = 0; // TODO: Reemplazar con tu lógica para obtener pagos del mes.
            decimal intereses = 0; // TODO: Reemplazar con tu lógica para obtener intereses.
            decimal impuestos = 0; // TODO: Reemplazar con tu lógica para obtener impuestos.

            decimal totalConsumos = movimientosDelPeriodo.Where(m => m.Monto > 0).Sum(m => m.Monto);
            decimal saldoActual = saldoAnterior - pagos + totalConsumos + intereses + impuestos;
            decimal pagoMinimo = saldoActual * 0.10m; // TODO: Reemplazar con tu lógica de cálculo de pago mínimo.

            return new DatosParaResumenDTO
            {
                Usuario = usuario,
                Periodo = periodoActual,
                Movimientos = movimientosDelPeriodo,
                SaldoAnterior = saldoAnterior,
                Pagos = pagos,
                Intereses = intereses,
                Impuestos = impuestos,
                SaldoActual = saldoActual,
                PagoMinimo = pagoMinimo
            };
        }


        public async Task ActualizarMovimientosAsyncModificado(Usuario usuario)
        {
            bool hayCambiosParaGuardar = false;
            bool nuevasEntidadesParaGuardar = false;
            int contNew = 0;
            int contUpdate = 0;
            long tiempo = 0;
            try
            {
                if (usuario == null) return;
                var cronometro = Stopwatch.StartNew();

                DatosEstructura empresa = await _context.DatosEstructura.FirstOrDefaultAsync();
                CombinedData data = await ConsultarMovimientos(empresa.UsernameWS, empresa.PasswordWS, usuario.Personas.NroDocumento, Convert.ToInt64(usuario.Personas.NroTarjeta), 10000, 0);
                List<Periodo> listaDePeriodos = await _context.Periodo.ToListAsync();

                if (data?.DetallesSolicitud == null) return;

                DateTime FechaCorte1 = new DateTime(2025, 05, 26);
                DateTime FechaCorte = new DateTime(2025, 06, 25);

                string CapitalAdeudado = data.Detalle.MontoAdeudado;
                string TotalProximaCuota = data.Detalle.TotalProximaCuota;
                string MontoDisponible = data.Detalle.MontoDisponible;

                decimal Movimientos = data.Movimientos.Where(x => x.Fecha<=FechaCorte).Sum(x => decimal.Parse(x.Monto, CultureInfo.InvariantCulture));
                decimal MovimientosRecargo = data.Movimientos.Where(x => x.Fecha<=FechaCorte).Sum(x => decimal.Parse(x.Recargo, CultureInfo.InvariantCulture));
                decimal MovientoTotal = Movimientos+MovimientosRecargo;


                //var MovimientosDetalles = data.DetallesSolicitud.Where(x => x.DetallesCuota.Any(e=> common.ConvertirFecha(e. Fecha)<=FechaCorte)).Sum(x => Convert.ToDecimal(x.DetallesCuota.Select(i=>i.Monto)));

                decimal movimientosDetallesMesActual = data.DetallesSolicitud
                 // 1. Aplana todas las listas de 'DetallesCuota' en una sola gran lista de cuotas
                 .SelectMany(detalle => detalle.DetallesCuota)
                 // 2. Filtra esa lista única de cuotas por la fecha de corte
                 .Where(cuota => common.ConvertirFecha(cuota.Fecha) >= FechaCorte1 && common.ConvertirFecha(cuota.Fecha) < FechaCorte)
                 // 3. Suma directamente el monto de las cuotas filtradas
                 .Sum(cuota => decimal.Parse(cuota.Monto, CultureInfo.InvariantCulture));


                decimal movimientosDetallesAnteriores = data.DetallesSolicitud
                // 1. Aplana todas las listas de 'DetallesCuota' en una sola gran lista de cuotas
                .SelectMany(detalle => detalle.DetallesCuota)
                // 2. Filtra esa lista única de cuotas por la fecha de corte
                .Where(cuota => common.ConvertirFecha(cuota.Fecha) <= FechaCorte1.AddDays(-1))
                // 3. Suma directamente el monto de las cuotas filtradas
                .Sum(cuota => decimal.Parse(cuota.Monto, CultureInfo.InvariantCulture));

                var movimientosDetalles2 = data.DetallesSolicitud
               // 1. Aplana todas las listas de 'DetallesCuota' en una sola gran lista de cuotas
               .SelectMany(detalle => detalle.DetallesCuota)
               // 2. Filtra esa lista única de cuotas por la fecha de corte
               .Where(cuota => common.ConvertirFecha(cuota.Fecha) <= FechaCorte)
               // 3. Suma directamente el monto de las cuotas filtradas
               .ToList();

                foreach (var item in movimientosDetalles2.OrderBy(x=> common.ConvertirFecha(x.Fecha)))
                {
                    decimal monto = decimal.Parse(item.Monto, CultureInfo.InvariantCulture);
                    DateTime fecha = common.ConvertirFecha(item.Fecha);
                    Debug.WriteLine($"{fecha} - $ {monto}");
                }








                // --- MARCAR MOVIMIENTOS COMO PAGADOS ---
                // 1. Crear un HashSet con las claves únicas (Solicitud+Cuota) que vienen del servicio.
                var cuotasExternas = data.DetallesSolicitud.SelectMany(detalle => detalle.DetallesCuota.Select(cuota => new Tuple<string, string>(detalle.NumeroSolicitud, cuota.NumeroCuota))).ToHashSet();

                // 2. Obtener movimientos no están pagados.
                var movimientosLocalesSinPagar = await _context.MovimientoTarjeta.Where(m => m.Usuario.Id == usuario.Id && !m.Pagado).ToListAsync();

                // 3. MARCAR COMO PAGADOS: Si un movimiento local no está en la nueva lista del servicio, se marca como Pagado.
                foreach (var movimientoLocal in movimientosLocalesSinPagar)
                {
                    var tuplaLocal = new Tuple<string, string>(movimientoLocal.NroSolicitud, movimientoLocal.NroCuota);
                    if (!cuotasExternas.Contains(tuplaLocal))
                    {
                        movimientoLocal.Pagado = true;
                        hayCambiosParaGuardar = true;
                        contUpdate++;
                    }
                }

                // 4. AÑADIR NUEVOS MOVIMIENTOS:
                // Creamos un HashSet con las claves de los movimientos que ya tenemos en la BD para evitar duplicados.
                var cuotasLocalesExistentes = movimientosLocalesSinPagar.Select(m => new Tuple<string, string>(m.NroSolicitud, m.NroCuota)).ToHashSet();

                foreach (SolicitudDetail detalle in data.DetallesSolicitud)
                {
                    string totalCuotasDeLaSolicitud = detalle.DetallesCuota.Max(x => x.NumeroCuota);

                    foreach (var cuota in detalle.DetallesCuota)
                    {
                        var tuplaExterna = new Tuple<string, string>(detalle.NumeroSolicitud, cuota.NumeroCuota);

                        // Si la cuota específica ya existe, saltamos a la siguiente.
                        if (cuotasLocalesExistentes.Contains(tuplaExterna)) continue;

                        // Si llegamos aquí, es una cuota nueva que debemos guardar.
                        hayCambiosParaGuardar = true;

                        DateTime fechaMovimiento = common.ConvertirFecha(cuota.Fecha);
                        var periodo = listaDePeriodos.FirstOrDefault(p => fechaMovimiento >= p.FechaDesde && fechaMovimiento <= p.FechaHasta);

                        if (periodo == null)
                        {
                            periodo = CrearNuevoPeriodo(fechaMovimiento);
                            if (periodo!=null)
                            {
                                _context.Periodo.Add(periodo);
                                listaDePeriodos.Add(periodo);
                            }

                        }

                        _context.MovimientoTarjeta.Add(new MovimientoTarjeta
                        {
                            NroSolicitud = detalle.NumeroSolicitud,
                            NombreComercio = detalle.NombreComercio,
                            NroCuota = cuota.NumeroCuota,
                            Monto = Convert.ToDecimal(cuota.Monto, CultureInfo.InvariantCulture),
                            Fecha = fechaMovimiento,
                            Usuario = usuario,
                            Periodo = periodo,
                            CantidadCuotas = totalCuotasDeLaSolicitud,
                            Pagado = false
                        });
                        contNew++;
                    }
                }

                // 5. GUARDAR TODOS LOS CAMBIOS UNA SOLA VEZ
                if (hayCambiosParaGuardar)
                {
                    await _context.SaveChangesAsync();
                }
                cronometro.Stop();
                await _context.LogProcedimientos.AddAsync(new LogProcedimientos
                {
                    Fecha = DateTime.Now,
                    Nombre = "ActualizarMovimientosIndividual",
                    Codigo = "SynchronizeMovementIndividual",
                    Mesaje = $"Se actualizaron {contUpdate} registros y se crearon {contNew} nuevos movimientos.",
                    StatusCode = "200",
                    RegistrosActualizados = contUpdate,
                    RegistrosNuevos = contNew,
                    Tiempo = cronometro.ElapsedMilliseconds // Aquí puedes calcular el tiempo si lo deseas
                });
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                await _context.LogProcedimientos.AddAsync(new LogProcedimientos
                {
                    Fecha = DateTime.Now,
                    Nombre = "ActualizarMovimientosIndividual",
                    Codigo = "SynchronizeMovementIndividual",
                    Mesaje = $"Error al Sincronizar - "+ e.Message,
                    StatusCode = "500",
                    RegistrosActualizados = contUpdate,
                    RegistrosNuevos = contNew,
                    Tiempo = 0 // Aquí puedes calcular el tiempo si lo deseas
                });
                await _context.SaveChangesAsync();
            }
        }

        public async Task ActualizarMovimientosAsync(Usuario usuario)
        {
            bool hayCambiosParaGuardar = false;
            bool nuevasEntidadesParaGuardar = false;
            int contNew = 0;
            int contUpdate = 0;
            long tiempo = 0;
            try
            {
                if (usuario == null) return;
                var cronometro = Stopwatch.StartNew();

                DatosEstructura empresa = await _context.DatosEstructura.FirstOrDefaultAsync();
                CombinedData data = await ConsultarMovimientos(empresa.UsernameWS, empresa.PasswordWS, usuario.Personas.NroDocumento, Convert.ToInt64(usuario.Personas.NroTarjeta), 100, 0);
                List<Periodo> listaDePeriodos = await _context.Periodo.ToListAsync();

                if (data?.DetallesSolicitud == null) return;

                // --- MARCAR MOVIMIENTOS COMO PAGADOS ---
                // 1. Crear un HashSet con las claves únicas (Solicitud+Cuota) que vienen del servicio.
                var cuotasExternas = data.DetallesSolicitud.SelectMany(detalle => detalle.DetallesCuota.Select(cuota => new Tuple<string, string>(detalle.NumeroSolicitud, cuota.NumeroCuota))).ToHashSet();

                // 2. Obtener movimientos no están pagados.
                var movimientosLocalesSinPagar = await _context.MovimientoTarjeta.Where(m => m.Usuario.Id == usuario.Id && !m.Pagado).ToListAsync();

                // 3. MARCAR COMO PAGADOS: Si un movimiento local no está en la nueva lista del servicio, se marca como Pagado.
                foreach (var movimientoLocal in movimientosLocalesSinPagar)
                {
                    var tuplaLocal = new Tuple<string, string>(movimientoLocal.NroSolicitud, movimientoLocal.NroCuota);
                    if (!cuotasExternas.Contains(tuplaLocal))
                    {
                        movimientoLocal.Pagado = true;
                        hayCambiosParaGuardar = true;
                        contUpdate++;
                    }
                }

                // 4. AÑADIR NUEVOS MOVIMIENTOS:
                // Creamos un HashSet con las claves de los movimientos que ya tenemos en la BD para evitar duplicados.
                var cuotasLocalesExistentes = movimientosLocalesSinPagar.Select(m => new Tuple<string, string>(m.NroSolicitud, m.NroCuota)).ToHashSet();

                foreach (SolicitudDetail detalle in data.DetallesSolicitud)
                {
                    string totalCuotasDeLaSolicitud = detalle.DetallesCuota.Max(x => x.NumeroCuota);

                    foreach (var cuota in detalle.DetallesCuota)
                    {
                        var tuplaExterna = new Tuple<string, string>(detalle.NumeroSolicitud, cuota.NumeroCuota);

                        // Si la cuota específica ya existe, saltamos a la siguiente.
                        if (cuotasLocalesExistentes.Contains(tuplaExterna)) continue;

                        // Si llegamos aquí, es una cuota nueva que debemos guardar.
                        hayCambiosParaGuardar = true;

                        DateTime fechaMovimiento = common.ConvertirFecha(cuota.Fecha);
                        var periodo = listaDePeriodos.FirstOrDefault(p => fechaMovimiento >= p.FechaDesde && fechaMovimiento <= p.FechaHasta);

                        if (periodo == null)
                        {
                            periodo = CrearNuevoPeriodo(fechaMovimiento);
                            if (periodo!=null)
                            {
                                _context.Periodo.Add(periodo);
                                listaDePeriodos.Add(periodo);
                            }

                        }

                        _context.MovimientoTarjeta.Add(new MovimientoTarjeta
                        {
                            NroSolicitud = detalle.NumeroSolicitud,
                            NombreComercio = detalle.NombreComercio,
                            NroCuota = cuota.NumeroCuota,
                            Monto = Convert.ToDecimal(cuota.Monto, CultureInfo.InvariantCulture),
                            Fecha = fechaMovimiento,
                            Usuario = usuario,
                            Periodo = periodo,
                            CantidadCuotas = totalCuotasDeLaSolicitud,
                            Pagado = false
                        });
                        contNew++;
                    }
                }

                // 5. GUARDAR TODOS LOS CAMBIOS UNA SOLA VEZ
                if (hayCambiosParaGuardar)
                {
                    await _context.SaveChangesAsync();
                }
                cronometro.Stop();
                await _context.LogProcedimientos.AddAsync(new LogProcedimientos
                {
                    Fecha = DateTime.Now,
                    Nombre = "ActualizarMovimientosIndividual",
                    Codigo = "SynchronizeMovementIndividual",
                    Mesaje = $"Se actualizaron {contUpdate} registros y se crearon {contNew} nuevos movimientos.",
                    StatusCode = "200",
                    RegistrosActualizados = contUpdate,
                    RegistrosNuevos = contNew,
                    Tiempo = cronometro.ElapsedMilliseconds // Aquí puedes calcular el tiempo si lo deseas
                });
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                await _context.LogProcedimientos.AddAsync(new LogProcedimientos
                {
                    Fecha = DateTime.Now,
                    Nombre = "ActualizarMovimientosIndividual",
                    Codigo = "SynchronizeMovementIndividual",
                    Mesaje = $"Error al Sincronizar - "+ e.Message,
                    StatusCode = "500",
                    RegistrosActualizados = contUpdate,
                    RegistrosNuevos = contNew,
                    Tiempo = 0 // Aquí puedes calcular el tiempo si lo deseas
                });
                await _context.SaveChangesAsync();
            }            
        }

        public async Task<JsonResult> ActualizarMovimientosAsync()
        {
            int contNew = 0;
            int contUpdate = 0;
            long tiempo = 0;
            try
            {
                var cronometro = Stopwatch.StartNew();
                IQueryable<Usuario> listUsuario = _context.Usuarios;

                bool hayCambiosParaGuardar = false;
                bool nuevasEntidadesParaGuardar = false;
                foreach (var usuario in listUsuario)
                {
                    try
                    {
                        DatosEstructura empresa = await _context.DatosEstructura.FirstOrDefaultAsync();

                        if (usuario.Personas==null) continue;

                        if (usuario.Personas.NroDocumento == "" || usuario.Personas.NroTarjeta=="") continue;

                        string numeroTarjetaConError = usuario.Personas.NroTarjeta;
                        if (!long.TryParse(numeroTarjetaConError, out long resultado))
                        {
                            _logger.LogWarning($"El número de tarjeta '{numeroTarjetaConError}' no es válido y será omitido.");
                            continue;
                        }

                        CombinedData data = await ConsultarMovimientos(empresa.UsernameWS, empresa.PasswordWS, usuario.Personas.NroDocumento, resultado, 100, 0);
                        List<Periodo> listaDePeriodos = await _context.Periodo.ToListAsync();

                        _logger.LogInformation("Usuario - " + usuario.UserName);
                        // --- MARCAR MOVIMIENTOS COMO PAGADOS ---
                        // 1. Crear un HashSet con las claves únicas (Solicitud+Cuota) que vienen del servicio.

                        if (data.Detalle.Resultado != "EXITO") continue;

                        var cuotasExternas = data.DetallesSolicitud.SelectMany(detalle => detalle.DetallesCuota.Select(cuota => new Tuple<string, string>(detalle.NumeroSolicitud, cuota.NumeroCuota))).ToHashSet();

                        // 2. Obtener movimientos no están pagados.
                        var movimientosLocalesSinPagar = await _context.MovimientoTarjeta.Where(m => m.Usuario.Id == usuario.Id && !m.Pagado).ToListAsync();

                        // 3. MARCAR COMO PAGADOS: Si un movimiento local no está en la nueva lista del servicio, se marca como Pagado.
                        foreach (var movimientoLocal in movimientosLocalesSinPagar)
                        {
                            var tuplaLocal = new Tuple<string, string>(movimientoLocal.NroSolicitud, movimientoLocal.NroCuota);
                            if (!cuotasExternas.Contains(tuplaLocal))
                            {
                                movimientoLocal.Pagado = true;
                                hayCambiosParaGuardar = true;
                                contUpdate++;
                            }
                        }

                        // 4. AÑADIR NUEVOS MOVIMIENTOS:
                        // Creamos un HashSet con las claves de los movimientos que ya tenemos en la BD para evitar duplicados.
                        var cuotasLocalesExistentes = movimientosLocalesSinPagar.Select(m => new Tuple<string, string>(m.NroSolicitud, m.NroCuota)).ToHashSet();

                        foreach (SolicitudDetail detalle in data.DetallesSolicitud)
                        {
                            string totalCuotasDeLaSolicitud = detalle.DetallesCuota.Max(x => x.NumeroCuota);

                            foreach (var cuota in detalle.DetallesCuota)
                            {
                                var tuplaExterna = new Tuple<string, string>(detalle.NumeroSolicitud, cuota.NumeroCuota);

                                // Si la cuota específica ya existe, saltamos a la siguiente.
                                if (cuotasLocalesExistentes.Contains(tuplaExterna)) continue;

                                // Si llegamos aquí, es una cuota nueva que debemos guardar.
                                hayCambiosParaGuardar = true;

                                DateTime fechaMovimiento = common.ConvertirFecha(cuota.Fecha);
                                var periodo = listaDePeriodos.FirstOrDefault(p => fechaMovimiento >= p.FechaDesde && fechaMovimiento <= p.FechaHasta);

                                if (periodo == null)
                                {
                                    periodo = CrearNuevoPeriodo(fechaMovimiento);
                                    if (periodo!=null)
                                    {
                                        _context.Periodo.Add(periodo);
                                        listaDePeriodos.Add(periodo);
                                    }

                                }

                                if (!decimal.TryParse(cuota.Monto, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out decimal monto))
                                {
                                    // Si falla, loguea la cuota problemática y sáltala.
                                    _logger.LogWarning($"El monto '{cuota.Monto}' no tiene un formato decimal válido. Se omite la cuota.");
                                    continue;
                                }

                                _context.MovimientoTarjeta.Add(new MovimientoTarjeta
                                {
                                    NroSolicitud = detalle.NumeroSolicitud,
                                    NombreComercio = detalle.NombreComercio,
                                    NroCuota = cuota.NumeroCuota,
                                    Monto = monto,
                                    Fecha = fechaMovimiento,
                                    Usuario = usuario,
                                    Periodo = periodo,
                                    CantidadCuotas = totalCuotasDeLaSolicitud,
                                    Pagado = false
                                });
                                contNew++;
                            }
                        }
                        // 5. GUARDAR TODOS LOS CAMBIOS UNA SOLA VEZ
                        if (hayCambiosParaGuardar)
                        {
                            await _context.SaveChangesAsync();
                            hayCambiosParaGuardar= false;
                        }
                    }
                    catch (Exception e)
                    {
                        continue; // Si hay un error con un usuario, lo saltamos y continuamos con el siguiente.
                    }
                }
                cronometro.Stop();

                await _context.LogProcedimientos.AddAsync(new LogProcedimientos
                {
                    Fecha = DateTime.Now,
                    Nombre = "ActualizarMovimientosAsync",
                    Codigo = "SynchronizeMovementIndividual",
                    Mesaje = $"Se actualizaron {contUpdate} registros y se crearon {contNew} nuevos movimientos.",
                    StatusCode = "200",
                    RegistrosActualizados = contUpdate,
                    RegistrosNuevos = contNew,
                    Tiempo = cronometro.ElapsedMilliseconds // Aquí puedes calcular el tiempo si lo deseas
                });
                await _context.SaveChangesAsync();

                return new JsonResult(new { mesanje = $"Se Actualizaron {contUpdate} Registros - Se crearon : {contNew} Registros", code = 200 });
            }
            catch (Exception e)
            {
                await _context.LogProcedimientos.AddAsync(new LogProcedimientos
                {
                    Fecha = DateTime.Now,
                    Nombre = "ActualizarMovimientosAsync",
                    Codigo = "SynchronizeMovementIndividual",
                    Mesaje = $"Error al Sincronizar - "+ e.Message,
                    StatusCode = "500",
                    RegistrosActualizados = contUpdate,
                    RegistrosNuevos = contNew,
                    Tiempo = 0 // Aquí puedes calcular el tiempo si lo deseas
                });
                await _context.SaveChangesAsync();
                return new JsonResult(new { mesanje = "Error - "+e.Message, code = 500 });
            }            
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

        /// <summary>
        /// Obtiene la lista de créditos (solicitudes) de una persona.
        /// </summary>
        public async Task<ObtenerCreditosResponse> ObtenerCreditosAsync(string login, string clave, int PersonaId)
        {
            // Creamos el cuerpo de la petición.
            var requestBody = new ObtenerCreditosRequest
            {
                LoginInterface = new LoginInterface
                {
                    Login = login,
                    Clave = clave,
                    Token = "" // El token podría ser necesario aquí si la API lo requiere.
                },
                IdPersona = PersonaId,
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            // Realizamos la llamada POST al endpoint de ObtenerCreditos.
            var response = await _httpClient.PostAsync($"{_apiBaseUrl}API/ECOMMERCE/OBTENERCREDITOS", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ObtenerCreditosResponse>(jsonResponse);
            }

            return null;
        }



        private Periodo CrearNuevoPeriodo(DateTime fechaMovimiento)
        {
            if (fechaMovimiento.Year<2025) return null;

            int diaDeCorte = 25;
            DateTime fechaHasta;

            if (fechaMovimiento.Day > diaDeCorte)
                fechaHasta = new DateTime(fechaMovimiento.Year, fechaMovimiento.Month, diaDeCorte).AddMonths(1);
            else
                fechaHasta = new DateTime(fechaMovimiento.Year, fechaMovimiento.Month, diaDeCorte);

            DateTime fechaDesde = fechaHasta.AddMonths(-1).AddDays(1);
            var mesDeVencimiento = fechaHasta.AddMonths(1);
            var fechaVencimiento = new DateTime(mesDeVencimiento.Year, mesDeVencimiento.Month, 15);

            return new Periodo
            {
                Descripcion = $"{fechaHasta:MMMM yyyy}",
                FechaDesde = fechaDesde,
                FechaHasta = fechaHasta,
                FechaVencimiento = fechaVencimiento,
                Activo = true
            };
        }

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
        public async Task<LoginUsuarioResponse> LoginApiLoanAsync(string usuario, string clave)
        {
            // Creamos el cuerpo de la petición según la documentación.
            var requestBody = new LoginUsuarioRequest
            {
                LoginInterface = new LoginInterface
                {
                    Login = usuario,
                    Clave = clave,
                    Token = "" // El token va vacío en la petición de login.
                },
                Login = usuario,
                Clave = clave
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_apiBaseUrl}API/ECOMMERCE/LOGINUSUARIO", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<LoginUsuarioResponse>(jsonResponse);
            }
            return null;
        }

        /// <summary>
        /// Obtiene los datos de una persona usando diferentes criterios de búsqueda.
        /// </summary>
        /// <param name="login">Login para autenticación.</param>
        /// <param name="clave">Clave para autenticación.</param>
        /// <param name="documento">Documento de la persona (opcional).</param>
        /// <returns>El objeto Persona si se encuentra, de lo contrario null.</returns>
        public async Task<PersonaLoanDTO> ObtenerPersonaAsync(string login, string clave, string documento)
        {
            // 1. Crear el cuerpo de la petición.
            var requestBody = new ObtenerPersonaRequest
            {
                LoginInterface = new LoginInterface
                {
                    Login = login,
                    Clave = clave,
                    Token = ""
                },
                Documento = documento
                // Puedes agregar otros filtros aquí si es necesario (Nombre, Sexo, etc.)
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            // 2. Realizar la llamada POST al endpoint ObtenerPersona.
            var response = await _httpClient.PostAsync($"{_apiBaseUrl}API/ECOMMERCE/OBTENERPERSONA", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var personaResponse = JsonConvert.DeserializeObject<ObtenerPersonaResponse>(jsonResponse);

                // Si el resultado es exitoso y se encontró una persona, la devolvemos.
                if (personaResponse?.Resultado?.Result == 1 && personaResponse.Persona != null)
                {
                    return personaResponse?.Persona;
                }
            }

            return null;
        }
    }


}