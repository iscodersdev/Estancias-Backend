using System;
using System.Net;
using System.IO;
using System.Text;

namespace EstanciasCore.Services
{
    public class TarjetaObtenerDatosClient2
    {
        public string ObtenerDatos(string usuario, string clave, long documento, long numeroTarjeta, long cantidadMovimientos)
        {
            string soapRequest =
                $@"<?xml version=""1.0"" encoding=""utf-8""?>
                <soap12:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap12=""http://www.w3.org/2003/05/soap-envelope"">
                  <soap12:Body>
                    <TarjetaRequest xmlns=""http://tempuri.org/"">
                      <TarjetaRequest>
                        <usuario>{usuario}</usuario>
                        <clave>{clave}</clave>
                        <documento>{documento}</documento>
                        <numeroTarjeta>{numeroTarjeta}</numeroTarjeta>
                        <cantidadMovimientos>{cantidadMovimientos}</cantidadMovimientos>
                      </TarjetaRequest>
                    </TarjetaRequest>
                  </soap12:Body>
                </soap12:Envelope>";

            string url = "http://sistema.cpecreditos.com.ar/Loan/ServiciosWeb/TarjetaWebService.asmx";
            string action = "http://tempuri.org/TarjetaObtenerDatos";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers.Add("SOAPAction", action);
            request.ContentType = "application/soap+xml; charset=utf-8";
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
}
