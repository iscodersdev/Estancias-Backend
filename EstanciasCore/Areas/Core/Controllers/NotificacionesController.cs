using Commons.Models;
using DAL.Data;
using DAL.DTOs;
using DAL.Models;
using DAL.Models.Core;
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
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

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
            return View();
        }

        
        public IActionResult ObtenerNotificaciones(Page<Notificaciones> page)
        {
            ViewBag.BotonNuevo = true;
            ViewBag.BotonBorrar = true;
            ViewBag.Titulo = "Listado Notificaciones Manuales";
            page.SelectPage("/NotificacionesPlantillas/ObtenerNotificaciones", _context.Notificaciones.Where(x => x.TipoNotificacionesProcedimientos.Codigo=="MA" && !x.Borrado && (string.IsNullOrEmpty(page.SearchText) || x.Nombre.Contains(page.SearchText))));
            return PartialView("_ListadoNotificaciones", page);
        }
        
        public IActionResult ObtenerNotificacionesAutomaticas(Page<Notificaciones> page)
        {
            ViewBag.BotonNuevo = false;
            ViewBag.BotonBorrar = true;
            ViewBag.Titulo = "Listado Notificaciones Automáticas";
            page.SelectPage("/NotificacionesPlantillas/ObtenerNotificacionesAutomaticas", _context.Notificaciones.Where(x => x.TipoNotificacionesProcedimientos.Codigo=="AU" && (string.IsNullOrEmpty(page.SearchText) || x.Nombre.Contains(page.SearchText))));
            return PartialView("_ListadoNotificacionesAutomaticas", page);
        }



        public ActionResult _Create()
        {
            ViewBag.TipoNotificacionesProcedimientos = _context.TipoNotificacionesProcedimientos.Select(x => new SelectListItem() { Text = x.Nombre, Value = x.Id.ToString() }).ToList();
            ViewBag.ListaDistribucion = _context.ListaDistribucion.Select(x => new SelectListItem() { Text = x.Nombre, Value = x.Id.ToString() }).ToList();
            ViewBag.Plantillas = _context.NotificacionesPlantillas.Select(x => new SelectListItem() { Text = x.Titulo, Value = x.Id.ToString() }).ToList();
            return PartialView();
        }

        [HttpPost]
        public async Task<ActionResult> _Create(Notificaciones notificaciones)
        {
            var tipoNotificacion = _context.TipoNotificacionesProcedimientos.Where(x => x.Codigo=="MA").FirstOrDefault();
            notificaciones.TipoNotificacionesProcedimientos = tipoNotificacion;
            notificaciones.Activo = true;

            await _context.Notificaciones.AddAsync(notificaciones);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }




        public ActionResult _Update(int Id)
        {
            Notificaciones notificaciones = _context.Notificaciones.Where(x => x.Id==Id).FirstOrDefault();
            return PartialView(notificaciones);
        }


        [HttpPost]
        public ActionResult _Update(Notificaciones notificaciones)
        {
            Notificaciones notificacionesUpdate = _context.Notificaciones.Where(x => x.Id==notificaciones.Id).FirstOrDefault();
            notificacionesUpdate.Nombre = notificaciones.Nombre;
            notificacionesUpdate.Descripcion = notificaciones.Descripcion;
            notificacionesUpdate.FechaEjecucion = notificaciones.FechaEjecucion;
            notificacionesUpdate.FechaUltimaEjecucion = DateTime.MinValue;
            _context.Notificaciones.Update(notificacionesUpdate);
            _context.SaveChanges();
            return RedirectToAction("Index");
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
                    if (lista.Nombre!="Todos")
                    {
                        List<DistribucionDestinatarios> destinatariosEnvio = _context.DistribucionDestinatarios.Where(x => x.ListaDistribucion.Id==lista.Id).ToList();
                        deviceList = destinatariosEnvio.Select(x => x.Destinatario.DeviceId).ToList();
                        usuarios = destinatariosEnvio.Select(x => x.Destinatario).ToList();
                    }
                    else
                    {
                        usuarios = _context.Usuarios.Where(x=>x.DeviceId!=null).ToList();
                        deviceList = usuarios.Select(x=>x.DeviceId).ToList();
                    }
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

        [HttpGet]
        public IActionResult _AsignarPlantilla(int id)
        {
            var notificacion = _context.Notificaciones.FirstOrDefault(x => x.Id == id);
            if (notificacion == null)
            {
                return NotFound();
            }

            ViewBag.NotificacionId = id;
            var plantillas = _context.NotificacionesPlantillas
                .Where(x => x.Activo && !x.NotificacionAutomatica)
                .Select(x => new SelectListItem 
                { 
                    Text = x.Nombre, 
                    Value = x.Id.ToString(),
                    Selected = notificacion.NotificacionesPlantillas != null && notificacion.NotificacionesPlantillas.Id == x.Id
                })
                .ToList();

            ViewBag.Plantillas = plantillas;

            return PartialView("_AsignarPlantilla");
        }

        [HttpGet]
        public IActionResult _AsignarPlantillaAutomaticas(int id)
        {
            var notificacion = _context.Notificaciones.FirstOrDefault(x => x.Id == id);
            if (notificacion == null)
            {
                return NotFound();
            }

            ViewBag.NotificacionId = id;
            var plantillas = _context.NotificacionesPlantillas
                .Where(x => x.Activo && x.NotificacionAutomatica)
                .Select(x => new SelectListItem 
                { 
                    Text = x.Nombre, 
                    Value = x.Id.ToString(),
                    Selected = notificacion.NotificacionesPlantillas != null && notificacion.NotificacionesPlantillas.Id == x.Id
                })
                .ToList();

            ViewBag.Plantillas = plantillas;

            return PartialView("_AsignarPlantilla");
        }

        [HttpPost]
        public async Task<IActionResult> AsignarPlantilla(int notificacionId, int plantillaId)
        {
            try
            {
                var notificacion = await _context.Notificaciones.Include(x => x.NotificacionesPlantillas).FirstOrDefaultAsync(x => x.Id == notificacionId);
                
                if (notificacion == null)
                     return Json(new { success = false, message = "Notificación no encontrada." });

                if (plantillaId <= 0)
                {
                    // Desasignar plantilla
                    notificacion.NotificacionesPlantillas = null;
                    _context.Notificaciones.Update(notificacion);
                    await _context.SaveChangesAsync();
                     return Json(new { success = true, message = "Plantilla desasignada correctamente." });
                }

                var plantilla = await _context.NotificacionesPlantillas.FirstOrDefaultAsync(x => x.Id == plantillaId);

                if (plantilla != null)
                {
                    notificacion.NotificacionesPlantillas = plantilla;
                    _context.Notificaciones.Update(notificacion);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Plantilla asignada correctamente." });
                }

                return Json(new { success = false, message = "Error al asignar la plantilla. Verifique los datos." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpGet]
        public IActionResult _AsignarListaDistribucion(int id)
        {
            var notificacion = _context.Notificaciones.FirstOrDefault(x => x.Id == id);
            if (notificacion == null)
            {
                return NotFound();
            }

            ViewBag.NotificacionId = id;
            var listas = _context.ListaDistribucion
                .Select(x => new SelectListItem 
                { 
                    Text = x.Nombre, 
                    Value = x.Id.ToString(),
                    Selected = notificacion.ListaDistribucion != null && notificacion.ListaDistribucion.Id == x.Id
                })
                .ToList();

            ViewBag.Listas = listas;

            return PartialView("_AsignarListaDistribucion");
        }

        [HttpPost]
        public async Task<IActionResult> AsignarListaDistribucion(int notificacionId, int listaDistribucionId)
        {
            try
            {
                var notificacion = await _context.Notificaciones.Include(x => x.ListaDistribucion).FirstOrDefaultAsync(x => x.Id == notificacionId);
                
                if (notificacion == null)
                     return Json(new { success = false, message = "Notificación no encontrada." });

                if (listaDistribucionId <= 0)
                {
                    // Desasignar lista
                    notificacion.ListaDistribucion = null;
                    _context.Notificaciones.Update(notificacion);
                    await _context.SaveChangesAsync();
                     return Json(new { success = true, message = "Lista desasignada correctamente." });
                }

                var lista = await _context.ListaDistribucion.FirstOrDefaultAsync(x => x.Id == listaDistribucionId);

                if (lista != null)
                {
                    notificacion.ListaDistribucion = lista;
                    _context.Notificaciones.Update(notificacion);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Lista asignada correctamente." });
                }

                return Json(new { success = false, message = "Error al asignar la lista. Verifique los datos." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> ToggleEstado(int id)
        {
            try
            {
                var notificacion = await _context.Notificaciones.FirstOrDefaultAsync(x => x.Id == id);
                if (notificacion != null)
                {
                    notificacion.Activo = !notificacion.Activo;
                    _context.Notificaciones.Update(notificacion);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Estado actualizado correctamente." });
                }
                return Json(new { success = false, message = "Notificación no encontrada." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> BorrarPermanente(int id)
        {
            try
            {
                var notificacion = await _context.Notificaciones.FirstOrDefaultAsync(x => x.Id == id);
                if (notificacion != null)
                {
                    notificacion.Borrado = true;
                    _context.Notificaciones.Update(notificacion);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Estado actualizado correctamente." });
                }
                return Json(new { success = false, message = "Notificación no encontrada." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpGet]
        public IActionResult _CambiarFecha(int id)
        {
            var notificacion = _context.Notificaciones.FirstOrDefault(x => x.Id == id);
            if (notificacion == null)
            {
                return NotFound();
            }
            return PartialView("_CambiarFecha", notificacion);
        }

        [HttpPost]
        public async Task<JsonResult> CambiarFecha(int Id, int Dia, DateTime Hora)
        {
            try
            {
                var notificacion = await _context.Notificaciones.FirstOrDefaultAsync(x => x.Id == Id);
                if (notificacion != null)
                {                    
                    var baseDate = notificacion.FechaEjecucion;
                    
                    try {
                        var newDate = new DateTime(baseDate.Year, baseDate.Month, Dia, Hora.Hour, Hora.Minute, 0);
                        
                         notificacion.FechaEjecucion = newDate;
                        _context.Notificaciones.Update(notificacion);
                        await _context.SaveChangesAsync();
                        return Json(new { success = true, message = "Fecha actualizada correctamente." });
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                         return Json(new { success = false, message = "El día seleccionado no es válido para el mes actual." });
                    }
                }
                return Json(new { success = false, message = "Notificación no encontrada." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
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