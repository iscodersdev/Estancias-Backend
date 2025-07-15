using DAL.Data;
using DAL.Mobile;
using DAL.Models;
using DAL.Models.Core;
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
using System;  
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
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


        public DatosTarjetaService(IConfiguration configuration, EstanciasContext context, IRazorViewEngine razorViewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider)
        {
            _Configuration = configuration;
            _context=context;
            _razorViewEngine = razorViewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
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

                foreach (var usuario in listUsuario)
                {
                    bool hayCambiosParaGuardar = false;
                    bool nuevasEntidadesParaGuardar = false;
                    DatosEstructura empresa = await _context.DatosEstructura.FirstOrDefaultAsync();
                    CombinedData data = await ConsultarMovimientos(empresa.UsernameWS, empresa.PasswordWS, usuario.Personas.NroDocumento, Convert.ToInt64(usuario.Personas.NroTarjeta), 100, 0);
                    List<Periodo> listaDePeriodos = await _context.Periodo.ToListAsync();

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
                return new JsonResult( new { mesanje = "Error - "+e.Message, code = 500 });
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

    }
}