using DAL.Data;
using DAL.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using System;
using Commons.Controllers;
using Commons.Identity.Services;

namespace EstanciasCore.Controllers
{
    [Route("api/[controller]")]

    public class MNovedadesController : BaseController
    {
        private readonly UserService<Usuario> _userManager;
        public EstanciasContext _context;
        public MNovedadesController(EstanciasContext context, UserService<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("TestNovedades")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MTraeNovedadesDTO TestNovedades()
        {
            return new MTraeNovedadesDTO { Mensaje= " desde alla",Status=200};
        }

        [HttpPost]
        [Route("TraeNovedades")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MTraeNovedadesDTO TraeNovedades([FromBody] MTraeNovedadesDTO uat)
        {
            var Uat = _context.UAT.FirstOrDefault(x => x.Token == uat.UAT);
            if (Uat == null)
            {
                uat.Status = 500;
                uat.Mensaje = "UAT Invalida";
                return uat;
            }

            var novedades = _context.Novedades.Where(x => x.Empresa.Id == Uat.Cliente.Empresa.Id && (uat.UltimaId == 0 || x.Id < uat.UltimaId) && x.Foto != null).OrderByDescending(x => x.Id).Take(3).Select(x => new MNovedad { Fecha = x.Fecha, Texto = x.Texto, Id = x.Id, Titulo = x.Titulo, Imagen = Convert.FromBase64String(x.Foto) }).ToList();
            if (novedades.Count > 0)
            {
                uat.Novedades = novedades;
            }
            uat.Status = 200;
            return uat;
        }

        [HttpPost]
        [Route("TraeCabecera")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MTraeCabeceraDTO TraeCabecera([FromBody] MTraeCabeceraDTO uat)
        {
            try
            {
                var Uat = _context.UAT.FirstOrDefault(x => x.Token == uat.UAT);
                if (uat == null) uat = new MTraeCabeceraDTO();
                var novedades = _context.Novedades.Where(x => (x.Empresa.Id == 3 && uat.UltimaId == 0 || x.Id < uat.UltimaId) && x.Foto != null).OrderByDescending(x => x.Id).Take(2).Select(x => 
                new MCabecera {
                    Id = x.Id,
                    Imagen = Convert.FromBase64String(x.Foto),
                    Titulo = x.Titulo, 
                    Subtitulo = x.Subtitulo,
                    Detalle = x.Texto,
                    TextoBoton = x.TextoBoton
                    }).ToList();
                if (novedades.Count > 0)
                {
                    uat.Novedades = novedades;
                }
                uat.Status = 200;
                return uat;
            }
            catch(Exception e){
                uat.Mensaje = e.Message;
                uat.Status = 500;
                return uat;
            }

        }

        [HttpPost]
        [Route("TraeMisNotificaciones")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MTraeNotificacionesDTO TraeMisNotificaciones([FromBody] MTraeNotificacionesDTO uat)
        {
            var Uat = _context.UAT.FirstOrDefault(x => x.Token == uat.UAT);
            if (Uat == null)
            {
                uat.Status = 500;
                uat.Mensaje = "UAT Invalida";
                return uat;
            }
            uat.Status = 200;
            uat.Mensaje = "Visto Grabado Correctamente";
            var notificaciones = _context.NotificacionesPersonas.Where(x => x.Cliente.Id == Uat.Cliente.Id && (x.Id < uat.NotificacionId || uat.NotificacionId == 0)).OrderByDescending(x => x.FechaHora).Take(50).Select(x => new MNotificaciones { Fecha = x.FechaHora, Descripcion = x.Descripcion, Id = x.Id, Titulo = x.Titulo, TomaConocimiento = x.TomaConocimiento == null ? false : true }).ToList();
            uat.Notificaciones = notificaciones;
            return uat;
        }
    }
}