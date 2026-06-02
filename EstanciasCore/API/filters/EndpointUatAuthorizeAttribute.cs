using DAL.Data;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstanciasCore.API.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class EndpointUatAuthorizeAttribute : Attribute, IAsyncActionFilter
    {
        private readonly EstanciasContext _context;

        public EndpointUatAuthorizeAttribute(EstanciasContext context)
        {
            _context = context;
        }

        async Task IAsyncActionFilter.OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var request = context.HttpContext.Request;

            /*
             * Si tenés CORS/preflight, conviene dejar pasar OPTIONS.
             * Si no usás CORS, esto no molesta.
             */
            if (request.Method.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase))
            {
                await next();
                return;
            }

            string token = ObtenerTokenDesdeHeader(request);

            /*
             * Si no vino por header, intento por query string.
             * Ejemplo:
             * /endpoint/proveedor?UAT=TOKEN
             */
            if (string.IsNullOrWhiteSpace(token))
            {
                token = ObtenerTokenDesdeQueryString(request);
            }

            /*
             * Si no vino por header ni query, intento por FormData.
             * Esto sirve para multipart/form-data, por ejemplo subida de imagen.
             */
            if (string.IsNullOrWhiteSpace(token))
            {
                token = ObtenerTokenDesdeFormData(request);
            }

            /*
             * Si no vino por header, query ni form-data, intento por body JSON.
             * Esto sirve para POST, PUT, PATCH, DELETE con body JSON.
             */
            if (string.IsNullOrWhiteSpace(token))
            {
                token = await ObtenerTokenDesdeJsonBody(request);
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                Log.Warning("Consulta de endpoint sin UAT. Método: {Metodo} - Path: {Path}",
                    request.Method,
                    request.Path);

                context.Result = new ObjectResult(new RespuestaAPI
                {
                    Status = 403,
                    Mensaje = "Consulta API sin UAT."
                })
                {
                    StatusCode = 403
                };

                return;
            }

            token = token.Trim();

            var uatValida = _context.UAT.FirstOrDefault(x => x.Token == token);

            if (uatValida == null)
            {
                Log.Warning("Consulta de endpoint con UAT inválida. Método: {Metodo} - Path: {Path}",
                    request.Method,
                    request.Path);

                context.Result = new ObjectResult(new RespuestaAPI
                {
                    Status = 403,
                    UAT = token,
                    Mensaje = "UAT inválida."
                })
                {
                    StatusCode = 403
                };

                return;
            }

            Log.Information("Request endpoint autorizado. IP: {IP} - Método: {Metodo} - Path: {Path}",
                request.HttpContext.Connection.RemoteIpAddress,
                request.Method,
                request.Path);

            await next();
        }

        private string ObtenerTokenDesdeHeader(HttpRequest request)
        {
            string token = "";

            /*
             * Forma principal:
             * UAT: TOKEN
             */
            if (request.Headers.TryGetValue("UAT", out var uatHeader))
            {
                token = uatHeader.ToString();
            }

            /*
             * Forma alternativa:
             * Authorization: Bearer TOKEN
             */
            if (string.IsNullOrWhiteSpace(token) &&
                request.Headers.TryGetValue("Authorization", out var authorizationHeader))
            {
                string authorization = authorizationHeader.ToString();

                if (!string.IsNullOrWhiteSpace(authorization) &&
                    authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    token = authorization.Substring("Bearer ".Length).Trim();
                }
            }

            return token;
        }

        private string ObtenerTokenDesdeQueryString(HttpRequest request)
        {
            string token = "";

            if (request.Query.ContainsKey("UAT"))
            {
                token = request.Query["UAT"].ToString();
            }

            if (string.IsNullOrWhiteSpace(token) && request.Query.ContainsKey("uat"))
            {
                token = request.Query["uat"].ToString();
            }

            return token;
        }

        private string ObtenerTokenDesdeFormData(HttpRequest request)
        {
            string token = "";

            try
            {
                if (request.HasFormContentType)
                {
                    if (request.Form.ContainsKey("UAT"))
                    {
                        token = request.Form["UAT"].ToString();
                    }

                    if (string.IsNullOrWhiteSpace(token) && request.Form.ContainsKey("uat"))
                    {
                        token = request.Form["uat"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "No se pudo leer FormData para validar UAT.");
            }

            return token;
        }

        private async Task<string> ObtenerTokenDesdeJsonBody(HttpRequest request)
        {
            string token = "";

            try
            {
                if (request.ContentType == null ||
                    !request.ContentType.ToLower().Contains("application/json"))
                {
                    return token;
                }

                request.EnableRewind();
                request.Body.Position = 0;

                string bodyStr = "";

                using (StreamReader reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
                {
                    bodyStr = await reader.ReadToEndAsync();
                }

                request.Body.Position = 0;

                if (string.IsNullOrWhiteSpace(bodyStr))
                {
                    return token;
                }

                var jsonBody = JObject.Parse(bodyStr);

                var uatJson = jsonBody.GetValue("UAT") ?? jsonBody.GetValue("uat");

                if (uatJson != null)
                {
                    token = uatJson.ToString();
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "No se pudo leer body JSON para validar UAT.");
            }

            return token;
        }
    }
}