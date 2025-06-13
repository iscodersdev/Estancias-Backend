using Commons.Models;
using DAL.Data;
using DAL.DTOs;
using DAL.Models;
using DAL.Models.Core;
using EstanciasCore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace EstanciasCore.Controllers
{
    [Area("Core")]
    public class NotificacionesController : EstanciasCoreController
    {
        public NotificacionesController(EstanciasContext context) : base(context)
        {
            breadcumb.Add(new Message() { DisplayName = "Datos" });
        }

        public IActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Notificaciones" });
            ViewBag.Breadcrumb = breadcumb;
            ViewBag.ListaDistribucion = _context.ListaDistribucion.Select(g => new SelectListItem() { Text = g.Nombre, Value = g.Id.ToString() });
            return View();
        }

        public async Task<IActionResult> _HistorialNotificaciones(Page<EnvioNotificaciones> page)
        {
            var c = _context.EnvioNotificaciones.Count();
            if (c < 1) { c = 1; }
            page.SelectPage("/Notificaciones/_HistorialNotificaciones",
                _context.EnvioNotificaciones, c);

            return PartialView("_HistorialNotificaciones", page);
        }

        public JsonResult DestinatariosComboJson(string q)
        {
            var items = _context.Usuarios
                .Where(x => x.Personas.NroDocumento.Contains(q))
                .Select(x => new
                {
                    Text = $"{x.Personas.Apellido}, {x.Personas.Nombres}",
                    Value = x.Id,
                    Subtext = $"{x.UserName}",
                    Icon = "fa fa-user"
                }).Take(10).ToArray();

            return Json(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _EnvioDeNotificacion(EnvioNotificacionDTO notificacion)
        {
            try
            {      
                List<Usuario> usuarios = new List<Usuario>();
                List<string> deviceList = new List<string>();
                byte[] imagen = null;
                if (notificacion.TipoDeEnvio==1)
                {
                    Usuario user = _context.Usuarios.Where(x => x.Id==notificacion.NroDocumentoNotificacion).FirstOrDefault();
                    if (user==null)
                    {
                        AddPageAlerts(PageAlertType.Error, "Hubo un error, El Dni Ingresado no es válido.");
                        return RedirectToAction("Index", "Notificaciones");
                    }
                    deviceList.Add(user.DeviceId);
                    usuarios.Add(user);
                }
                else
                {
                    //Buscar Lista de distribución
                    ListaDistribucion lista = _context.ListaDistribucion.Where(x => x.Id==Convert.ToInt32(notificacion.DistribucionNotificacion)).FirstOrDefault();
                    List<DistribucionDestinatarios> destinatariosEnvio = _context.DistribucionDestinatarios.Where(x => x.ListaDistribucion.Id==lista.Id).ToList();
                    deviceList = destinatariosEnvio.Select(x => x.Destinatario.DeviceId).ToList();
                    usuarios = destinatariosEnvio.Select(x => x.Destinatario).ToList();
                }
                string[] deviceIds = deviceList.ToArray();

                if (notificacion.File!=null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await notificacion.File.CopyToAsync(memoryStream);
                        imagen = memoryStream.ToArray();
                    }
                }

                HttpStatusCode response = common.EnviaNotificationWonderPushId(notificacion.TituloNotificacion, notificacion.TextoNotificacion, deviceIds, imagen);

                if (response== HttpStatusCode.Accepted)
                {
                    GuardarNotificacion(usuarios, notificacion, imagen);
                }
                AddPageAlerts(PageAlertType.Success, "Se enviaron las Notificaciones correctamente.");
                return RedirectToAction("Index", "Notificaciones");
            }
            catch (Exception e)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error, "+ e.Message);
                return RedirectToAction("Index", "Notificaciones");
            }
        }


        public async Task<IActionResult> _Imagen(int Id)
        {
            EnvioNotificaciones notificacion = _context.EnvioNotificaciones.Where(x=>x.Id == Id).FirstOrDefault();
            ViewBag.Foto = notificacion.Foto;
            ViewBag.Titulo = notificacion.Titulo;
            return PartialView("_Imagen");
        }


        public async Task<IActionResult> _HistorialDestinatariosNotificaciones(Page<Usuario> page, int Id)
        {
            var c = _context.EnvioNotificacionesDestinatarios.Where(x => x.Id == Id).Select(x=>x.Destinatario).Count();
            if (c < 1) { c = 1; }
            page.SelectPage("/Notificaciones/_HistorialDestinatariosNotificaciones",
                _context.EnvioNotificacionesDestinatarios.Where(x => x.Id == Id).Select(x => x.Destinatario), c);
            return PartialView("_HistorialDestinatariosNotificaciones", page);
        }


        /*-------------------------------------------------- Funciones -------------------------------------------------------------*/

        private bool GuardarNotificacion(List<Usuario> destinatarios, EnvioNotificacionDTO notificacion, byte[] imagen = null)
        {
            EnvioNotificaciones envioNotificaciones = new EnvioNotificaciones()
            {
                Titulo = notificacion.TituloNotificacion,
                Texto = notificacion.TextoNotificacion,
                Foto = imagen,
                Fecha = DateTime.Now,
                Envio = true,
            };
            _context.EnvioNotificaciones.Add(envioNotificaciones);

            foreach (var item in destinatarios)
            {
                EnvioNotificacionesDestinatarios notificacionDestinatario = new EnvioNotificacionesDestinatarios()
                {
                    Notificacion = envioNotificaciones,
                    Destinatario = item,
                    Envio = true,
                };
                _context.EnvioNotificacionesDestinatarios.Add(notificacionDestinatario);
            }
            _context.SaveChanges();
            return true;
        }
    }
}