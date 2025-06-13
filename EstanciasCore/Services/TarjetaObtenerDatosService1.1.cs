using System;
using System.Net;
using System.IO;
using System.Text;
using System.Xml;
using System.Collections.Generic;
using DAL.Mobile;
using Castle.Core.Internal;
using System.Xml.Linq;
using System.Linq;
using System.Globalization;
using Microsoft.AspNetCore.Routing;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using SixLabors.ImageSharp;
using Microsoft.CodeAnalysis.Differencing;

namespace EstanciasCore.Services
{
    public class TarjetaObtenerDatosClient
    {
        public string ObtenerDatos(string usuario, string clave, long documento, long numeroTarjeta, long cantidadMovimientos)
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

            using (Stream stream = request.GetRequestStream())
            {
                byte[] data = Encoding.UTF8.GetBytes(soapRequest);
                stream.Write(data, 0, data.Length);
            }

            string soapResult;
            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    soapResult = reader.ReadToEnd();
                }
            }

            return soapResult;
        }
    }

    public class MovPersona
    {
        public ListaMovimientoTarjetaDTO ConsultarMovimientosTarjetas(string usuario, string clave, String documento, long numeroTarjeta, long cantidadMovimientos, int tipomovimientotarjeta)
        {
            // Instancia el cliente SOAP para obtener los datos de la tarjeta
            var cliente = new TarjetaObtenerDatosClient();
            
            

            // Realiza la llamada al servicio SOAP para obtener los datos de la tarjeta
            string soapResponse = cliente.ObtenerDatos(usuario, clave,Convert.ToInt32(documento), numeroTarjeta, cantidadMovimientos);

            // Procesa la respuesta XML
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(soapResponse);

            XmlNodeList DatosNodes = xmlDoc.GetElementsByTagName("TarjetaObtenerDatosResult");
            ListaMovimientoTarjetaDTO datos = new ListaMovimientoTarjetaDTO();

            // Aquí procesas y extraes los datos que necesitas del XML

            foreach (XmlNode item in DatosNodes)
            {

                datos.Resultado = item.ChildNodes[0].ChildNodes[0].ChildNodes[0].InnerText;
                if (datos.Resultado == "EXITO")
                {
                    datos.NroDocumento = Convert.ToInt64(item.ChildNodes[1].InnerText);
                    datos.Nombre = item.ChildNodes[2].InnerText;
                    datos.Direccion = item.ChildNodes[3]?.InnerText;
                    datos.MontoAdeudado = item.ChildNodes[4]?.InnerText;
                    datos.ProximaFechaPago = item.ChildNodes[5]?.InnerText;
                    datos.TotalProximaCuota = item.ChildNodes[6]?.InnerText;
                    datos.MontoDisponible = item.ChildNodes[10]?.InnerText.Replace(".",",");
                    datos.tipomovimiento = tipomovimientotarjeta;
                    string fechaproximapago ;
                    if (DateTime.Now.Day > 15 ) 
                    {
                        
                        fechaproximapago = DateTime.Now.Year + "-" + (DateTime.Now.Month + 1)  + "-15";
                    } 
                    else 
                    {
                        fechaproximapago = DateTime.Now.Year + "-" + DateTime.Now.Month + "-15";

                    }
                    //datos.FechaPagoProximaCuota = item.ChildNodes[7]?.InnerText;
                    datos.FechaPagoProximaCuota = fechaproximapago;
                    datos.NroTarjeta = numeroTarjeta;
                    datos.CantMovimientos = Convert.ToInt64(cantidadMovimientos.ToString());
                    datos.MovimientosTarjeta = new List<MovimientoTarjetaDTO>();
                    datos.DetalleMovimientosTarjeta = new List<DetalleMovimientoTarjetaDTO>();
                    for (int i = 0; i < item.ChildNodes[8].ChildNodes.Count; i++)
                    {
                        if (item.ChildNodes[8].ChildNodes[i].ChildNodes[0] != null )
                        {
                            MovimientoTarjetaDTO movimimientotarjeta = new MovimientoTarjetaDTO();
                            movimimientotarjeta.Fecha = item.ChildNodes[8].ChildNodes[i].ChildNodes[0]?.InnerText;
                            movimimientotarjeta.TipoMovimiento = item.ChildNodes[8].ChildNodes[i].ChildNodes[1]?.InnerText;
                            movimimientotarjeta.Monto = item.ChildNodes[8].ChildNodes[i].ChildNodes[3]?.InnerText;
                            datos.MovimientosTarjeta.Add(movimimientotarjeta);
                        }

                    }

                    for (int i = 0; i < item.ChildNodes[9].ChildNodes.Count; i++)
                    {
                        if (item.ChildNodes[9].ChildNodes[i].ChildNodes[2].ChildNodes[0] != null)
                        {
                            for (int z = 0; z < item.ChildNodes[9].ChildNodes[i].ChildNodes[2].ChildNodes.Count; z++)
                            {
                                DetalleMovimientoTarjetaDTO detallemovimimientotarjeta = new DetalleMovimientoTarjetaDTO();
                                detallemovimimientotarjeta.NumeroSolicitud = item.ChildNodes[9].ChildNodes[i].ChildNodes[0]?.InnerText;
                                detallemovimimientotarjeta.Monto = item.ChildNodes[9].ChildNodes[i].ChildNodes[2]?.ChildNodes[z].ChildNodes[0]?.InnerText;
                                detallemovimimientotarjeta.NroCuota = item.ChildNodes[9].ChildNodes[i].ChildNodes[2]?.ChildNodes[z].ChildNodes[1]?.InnerText;
                                var fechav = item.ChildNodes[9].ChildNodes[i].ChildNodes[2]?.ChildNodes[z].ChildNodes[2]?.InnerText.Split("T");
                                detallemovimimientotarjeta.Fecha = Convert.ToDateTime(fechav[0].ToString());
                                datos.DetalleMovimientosTarjeta.Add(detallemovimimientotarjeta);
                            }
                        }

                    }

                }
            }           
            return (datos);
        
        }

        public CombinedData ConsultarMovimientosTarjetas2(string usuario, string clave, String documento, long numeroTarjeta, long cantidadMovimientos, int tipomovimientotarjeta)
		{

			// Instancia el cliente SOAP para obtener los datos de la tarjeta
			var cliente = new TarjetaObtenerDatosClient();



			// Realiza la llamada al servicio SOAP para obtener los datos de la tarjeta
			string soapResponse = cliente.ObtenerDatos(usuario, clave, Convert.ToInt32(documento), numeroTarjeta, cantidadMovimientos);

			// Procesa la respuesta XML
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(soapResponse);

			XmlNodeList DatosNodes = xmlDoc.GetElementsByTagName("TarjetaObtenerDatosResult");
			ListaMovimientoTarjetaDTO datos = new ListaMovimientoTarjetaDTO();

			// Aquí procesas y extraes los datos que necesitas del XML

			XDocument doc = XDocument.Parse(soapResponse);
			XNamespace ns = "http://tempuri.org/";


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
	}

	public class Detail
	{
		public string Resultado { get; set; }
		public string Mensaje { get; set; }
		public string Documento { get; set; }
		public string Nombre { get; set; }
		public string MontoDisponible { get; set; }
		public string Direccion { get; set; }
		public string MontoAdeudado { get; set; }
		public DateTime ProximaFechaPago { get; set; }
		public string TotalProximaCuota { get; set; }
		public DateTime FechaPagoProximaCuota { get; set; }
	}
	public class MovementDetail
	{
		public DateTime Fecha { get; set; }
		public string Descripcion { get; set; }
		public string Monto { get; set; }
		public string Recargo { get; set; }
	}

	public class SolicitudDetail
	{
		public string NumeroSolicitud { get; set; }
		public string NombreComercio { get; set; }
		public List<DetalleCuota> DetallesCuota { get; set; }
	}

	public class DetalleCuota
	{
		public string Fecha { get; set; }
		public string NumeroCuota { get; set; }
		public string Monto { get; set; }
	}

	public class CombinedData
	{
		public Detail Detalle { get; set; }
		public List<MovementDetail> Movimientos { get; set; }
		public List<SolicitudDetail> DetallesSolicitud { get; set; }
	}

}

