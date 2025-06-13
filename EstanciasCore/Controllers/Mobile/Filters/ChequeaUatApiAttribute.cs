using DAL.Data;
using DAL.Models;
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
    public class ChequeaUatApiAttribute : Attribute, IAsyncActionFilter
    {
        private readonly EstanciasContext _context;
        public ChequeaUatApiAttribute(EstanciasContext context)
        {
            _context = context;
        }

        async Task IAsyncActionFilter.OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var bodyStr = "";
            var req = context.HttpContext.Request;
            var jsonBody = new JObject();
            if (req.Method.ToUpper() == "POST")
            {
                req.EnableRewind();
                req.Body.Position = 0;
                using (StreamReader reader = new StreamReader(req.Body, Encoding.UTF8, true, 1024, true))
                {
                    bodyStr = reader.ReadToEnd();
                }
                req.Body.Position = 0;
                jsonBody = JObject.Parse(bodyStr);
                var uatPost = jsonBody.GetValue("UAT") ?? "";
                if (_context.UAT.FirstOrDefault(x => x.Token == uatPost.ToString().Trim()) == null)
                {
                    Log.Error("Consulta de APi sin UAT o UAT invalida");
                    context.Result = new BadRequestObjectResult(new RespuestaAPI { UAT = uatPost.ToString().Trim(), Mensaje = "UAT invalida", Status = 403 });
                    return;
                }
            }

            if (req.Method.ToUpper() == "GET")
            {
                if (!context.HttpContext.Request.Headers.TryGetValue("UAT", out var uat))
                {
                    Log.Error("Consulta de APi sin UAT");
                    context.Result = new BadRequestObjectResult(new RespuestaAPI { Mensaje = "Consulta API sin UAT" });
                    return;
                }

                if (_context.UAT.FirstOrDefault(x => x.Token == uat) == null)
                {
                    Log.Error("Consulta de APi con UAT invalida");
                    context.Result = new BadRequestObjectResult(new RespuestaAPI { Mensaje = "UAT invalida" });
                    return;
                }
            }

            Log.Warning($"Request API: {req.HttpContext.Connection.RemoteIpAddress} - {req.Path} -  {jsonBody}");

            await next();
        }
    }
}
