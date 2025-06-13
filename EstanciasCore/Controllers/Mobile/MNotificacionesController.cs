using DAL.Data;
using DAL.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using System;
using Commons.Controllers;
using Commons.Identity.Services;
using QRCoder;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Security.Policy;
using DAL.DTOs;
using EstanciasCore.Services;
using DAL.DTOs.API;
using Newtonsoft.Json;
using static EstanciasCore.Services.common;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;

namespace EstanciasCore.Controllers
{
    [Route("api/[controller]")]

    public class MNotificacionesController : BaseController
    {
        private readonly UserService<Usuario> _userManager;
        public EstanciasContext _context;
        public MNotificacionesController(EstanciasContext context, UserService<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("TraeNotificaciones")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public NotificacionDTO TraeNotificaciones([FromBody] NotificacionDTO uat)
        {

            try
            {
                var Uat = _context.UAT.FirstOrDefault(x => x.Token == uat.UAT);
                if (Uat == null)
                {
                    uat.Status = 500;
                    uat.Mensaje = "UAT Invalida";
                    return uat;
                }

                NotificacionDTO notificacion = new NotificacionDTO();

                notificacion.ListNotificaciones = _context.NotificacionesPersonas.Where(x => x.Cliente.Id==Uat.Cliente.Id).Select(x => new NotificacionDetalleDTO()
                {
                    Id = x.Id,
                    Titulo = x.Titulo,
                    Texto = x.Descripcion,
                    Fecha = x.FechaHora.ToString("dd/MM/yyyy"),
                    Leido = (x.TomaConocimiento==null ? false : true),
                    FechaLeido = x.TomaConocimiento.HasValue ? x.TomaConocimiento.Value.ToString("dd/MM/yyyy") : ""
                }).ToList();

                var NoLeidas = notificacion.ListNotificaciones.Where(x => x.Leido==false).ToList();

                notificacion.CantidadNotificaciones = notificacion.ListNotificaciones.Count();
                notificacion.CantidadNotificacionesNoLeida = NoLeidas.Count();
                notificacion.NotificacionNoLeida = NoLeidas.Count()>0 ? true : false;
                notificacion.Status = 200;
                notificacion.Mensaje = "Novedades";
                notificacion.UAT = uat.UAT;
                return notificacion;
            }
            catch (Exception e)
            {
                uat.Status = 500;
                uat.Mensaje = "Error al traer las Notificaciones";
                return uat;
            }

           
        }



        [HttpPost]
        [Route("AcusoNotificacion")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public AcusoNotificacionDTO AcusoNotificacion([FromBody] AcusoNotificacionDTO uat)
        {
            try
            {
                var Uat = _context.UAT.FirstOrDefault(x => x.Token == uat.UAT);
                if (Uat == null)
                {
                    uat.Status = 500;
                    uat.Mensaje = "UAT Invalida";
                    return uat;
                }

                NotificacionesPersonas notificacion = _context.NotificacionesPersonas.Where(x => x.Id==uat.NotificaionId).FirstOrDefault();
                notificacion.TomaConocimiento = DateTime.Now;
                _context.NotificacionesPersonas.Update(notificacion);
                _context.SaveChanges();
                uat.Status = 200;
                uat.Mensaje = "Notificacion Leída";
                uat.UAT = uat.UAT;
                return uat;
            }
            catch (Exception e)
            {
                uat.Status = 500;
                uat.Mensaje = "Error";
                return uat;
            }
            
        }

        public class WonderPush
        {
            public string title { get; set; }
            public string message { get; set; }
            public string deviceId { get; set; }
            public string imagen { get; set; }
        }

        [HttpPost]
        [Route("EnviaNotificationWonderPushId")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public HttpStatusCode EnviaNotificationWonderPushId(WonderPush wonder)
        {
            string title = wonder.title;
            string message= wonder.message;
            string deviceId= wonder.deviceId;
            string imagen=  wonder.imagen;

            HttpClient _httpClient = new HttpClient();
            //string iconPath = "https://cdn.by.wonderpush.com/upload/01hvf7n5tnuj29id/bfd983f284b4b409a187529e60a1f38da1750d97/v1/small";
            string iconPath = imagen;
            var accessToken = "ZDdmZTQ3YzQxZDI5YmNhYTUyMGEwOGVhZjI4YmQwMWMxZjg4ZDc1Mjk5NjVmOGVkMzE5YjIwYzcwNzBhZTE1NQ";
            var url = "https://management-api.wonderpush.com/v1/deliveries?accessToken=" + accessToken;

            var notification = new NotificationWonderPush
            {
                Alert = new AlertWonderPush
                {
                    Title = title,
                    Text = message
                },
                ImageUrl  = iconPath
            };
            var notificationJson = JsonConvert.SerializeObject(notification);

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("targetDeviceIds", deviceId),
                new KeyValuePair<string, string>("notification", notificationJson)
            });
            var response = _httpClient.PostAsync(url, content);
            var status = response.Result.StatusCode;
            return status;
        }
    }
}