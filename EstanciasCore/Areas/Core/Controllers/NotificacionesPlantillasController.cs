using Commons.Models;
using DAL.Data;
using DAL.DTOs;
using DAL.Models;
using DAL.Models.Core;
using EstanciasCore.Interface;
using EstanciasCore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace EstanciasCore.Controllers
{
    [Area("Core")]
    public class NotificacionesPlantillasController : EstanciasCoreController
    {
        private readonly IWonderPushService _wonderPushService;

        public NotificacionesPlantillasController(EstanciasContext context, IWonderPushService wonderPushService) : base(context)
        {
            _wonderPushService = wonderPushService;
            breadcumb.Add(new Message() { DisplayName = "Datos" });
        }

        public IActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Plantillas" });
            ViewBag.Breadcrumb = breadcumb;
            return View();
        }

        
        public IActionResult ObtenerNotificacionesPlantillas(Page<NotificacionesPlantillas> page)
        {    
            page.SelectPage("/NotificacionesPlantillas/ObtenerNotificacionesPlantillas", _context.NotificacionesPlantillas, x => (x.Nombre.Contains(page.SearchText) || x.Nombre.Contains(page.SearchText)));
            return PartialView("_ListadoNotificacionesPlantillas", page);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(NotificacionViewModelDTO model)
        {
            NotificacionesPlantillas plantillas = new NotificacionesPlantillas()
            {
                Nombre = model.Nombre,
                Titulo = model.Titulo,
                Mensaje = model.Mensaje,
                ImagenUrl = model.ImagenUrl,
                PreferLargeImage = model.PreferLargeImage,
                DeepLink = model.DeepLink,
                Activo = true
            };
            await _context.NotificacionesPlantillas.AddAsync(plantillas);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Update(int id)
        {
            var plantilla = _context.NotificacionesPlantillas.FirstOrDefault(x => x.Id == id);
            if (plantilla == null)
            {
                return RedirectToAction("Index");
            }

            var model = new NotificacionViewModelDTO
            {
                Id = plantilla.Id,
                Nombre = plantilla.Nombre,
                Titulo = plantilla.Titulo,
                Mensaje = plantilla.Mensaje,
                ImagenUrl = plantilla.ImagenUrl,
                DeepLink = plantilla.DeepLink,
                PreferLargeImage = plantilla.PreferLargeImage
            };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Update(NotificacionViewModelDTO model)
        {
            var plantilla = _context.NotificacionesPlantillas.FirstOrDefault(x => x.Id == model.Id);
            if (plantilla != null)
            {
                plantilla.Nombre = model.Nombre;
                plantilla.Titulo = model.Titulo;
                plantilla.Mensaje = model.Mensaje;
                plantilla.ImagenUrl = model.ImagenUrl;
                plantilla.PreferLargeImage = model.PreferLargeImage;
                plantilla.DeepLink = model.DeepLink;
                
                _context.NotificacionesPlantillas.Update(plantilla);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            try
            {
                var estaEnUso = _context.Notificaciones.Any(x => x.NotificacionesPlantillas.Id == id);
                if (estaEnUso)
                {
                    return Json(new { success = false, message = "La plantilla está asignada a una notificación y no se puede borrar." });
                }

                var plantilla = _context.NotificacionesPlantillas.FirstOrDefault(x => x.Id == id);
                if (plantilla != null)
                {
                    _context.NotificacionesPlantillas.Remove(plantilla);
                    await _context.SaveChangesAsync();
                }

                return Json(new { success = true, message = "Borrada con éxito." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al borrar: " + ex.Message });
            }

        }



        [HttpGet]
        public JsonResult GetListasDistribucion()
        {
            var listas = _context.ListaDistribucion
                .Select(x => new { x.Id, x.Nombre })
                .OrderBy(x => x.Nombre)
                .ToList();
            return Json(listas);
        }

        [HttpPost]
        public async Task<JsonResult> EnviarTest(int id, int listaDistribucionId)
        {
            try
            {           
                var plantilla = _context.NotificacionesPlantillas.FirstOrDefault(x => x.Id == id);
                if (plantilla == null)
                    return Json(new { success = false, message = "Plantilla no encontrada." });

                var testList = _context.ListaDistribucion.FirstOrDefault(x => x.Id == listaDistribucionId);
                if (testList == null)
                    return Json(new { success = false, message = "Lista de distribución no encontrada." });

                var deviceIds = new List<string>();
                var destinatarios = new List<Usuario>();


                if (testList.Id==1)
                {
                    var allUsers = _context.Usuarios.Where(x => x.DeviceId!=null).ToList();
                    foreach (var user in allUsers)
                    {
                        if (!string.IsNullOrEmpty(user.DeviceId))
                        {
                            deviceIds.Add(user.DeviceId);
                            destinatarios.Add(user);
                        }
                    }
                }
                else
                {
                    var distDestinatarios = _context.DistribucionDestinatarios
                        .Include(x => x.Destinatario)
                        .Where(x => x.ListaDistribucion.Id == testList.Id)
                        .ToList();

                    foreach (var item in distDestinatarios)
                    {
                        if (item.Destinatario != null && !string.IsNullOrEmpty(item.Destinatario.DeviceId))
                        {
                            deviceIds.Add(item.Destinatario.DeviceId);
                            destinatarios.Add(item.Destinatario);
                        }
                    }
                }
                                  

                if (!deviceIds.Any())
                    return Json(new { success = false, message = $"La lista '{testList.Nombre}' no tiene destinatarios con DeviceId." });

                var dto = new NotificacionViewModelDTO
                {
                    Titulo = plantilla.Titulo,
                    Mensaje = plantilla.Mensaje,
                    ImagenUrl = plantilla.ImagenUrl,
                    PreferLargeImage = plantilla.PreferLargeImage,
                    DeepLink = plantilla.DeepLink
                };

                var result = await _wonderPushService.EnviarNotificacionAIds(dto, deviceIds);

                // Log to Database
                var envioNotificacion = new EnvioNotificaciones
                {
                    Titulo = plantilla.Titulo,
                    Texto = plantilla.Mensaje,
                    Fecha = DateTime.Now,
                    Envio = result,
                    NotificacionesPlantillas = plantilla,
                    Foto = null // Or handle image byte[] if needed, but currently we use URL
                };
                
                await _context.EnvioNotificaciones.AddAsync(envioNotificacion);
                await _context.SaveChangesAsync();

                var envioDestinatarios = new List<EnvioNotificacionesDestinatarios>();
                foreach (var user in destinatarios)
                {
                    envioDestinatarios.Add(new EnvioNotificacionesDestinatarios
                    {
                        Notificacion = envioNotificacion,
                        Destinatario = user,
                        Envio = result
                    });
                }
                
                await _context.EnvioNotificacionesDestinatarios.AddRangeAsync(envioDestinatarios);
                await _context.SaveChangesAsync();

                if (result)
                    return Json(new { success = true, message = "Notificación de prueba enviada y registrada." });
                else
                    return Json(new { success = false, message = "Error al enviar la notificación a WonderPush (Registrado como fallido)." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

    }
}