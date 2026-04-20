using Commons.Models;
using DAL.Data;
using DAL.DTOs;
using DAL.Models;
using DAL.Models.Core;
using EstanciasCore.Interface;
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
    public class EnvioDeNotificaciones : EstanciasCoreController
    {
        private readonly IPushService _wonderPushService;

        public EnvioDeNotificaciones(EstanciasContext context, IPushService wonderPushService) : base(context)
        {
            _wonderPushService = wonderPushService;
            breadcumb.Add(new Message() { DisplayName = "Datos" });
        }

        [HttpGet]
        public IActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Notificaciones" });
            var list = _context.EnvioNotificaciones.ToList();
            return View(list);
        }

        [HttpGet]
        public IActionResult PlantillaNotificacion()
        {
            breadcumb.Add(new Message() { DisplayName = "Nueva Notificación" });
            return View(new NotificacionViewModelDTO());
        }

        [HttpPost]
        public async Task<IActionResult> Enviar(NotificacionViewModelDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View("PlantillaNotificacion", model);
            }

            var result = await _wonderPushService.EnviarNotificacionGeneral(model);

            if (result)
            {
                TempData["MensajeExito"] = "Notificación enviada correctamente.";
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Hubo un error al enviar la notificación.");
                return View("PlantillaNotificacion", model);
            }
        }
    }
}