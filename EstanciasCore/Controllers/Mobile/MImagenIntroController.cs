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

namespace EstanciasCore.Controllers
{
    [Route("api/[controller]")]

    public class MImagenIntroController : BaseController
    {
        private readonly UserService<Usuario> _userManager;
        public EstanciasContext _context;
        public MImagenIntroController(EstanciasContext context, UserService<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("TraeImagenIntro")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MTraeImagenIntroDTO TraeImagenIntro([FromBody] MTraeImagenIntroDTO uat)
        {
            //var banner = _context.ImagenIntro.Where(x => x.Empresa.Id == Uat.Cliente.Empresa.Id && (uat.UltimaId == 0 || x.Id < uat.UltimaId) && x.Foto != null)
            //    .OrderByDescending(x => x.Id).Select(x => new MImagenIntro { Fecha = x.Fecha, Id = x.Id, Titulo = x.Titulo, Orden = x.Orden,  Imagen = Convert.FromBase64String(x.Foto) }).ToList();

            var iamgenes = _context.ImagenIntro.Where(x => x.Orden != 0 && x.Foto!=null)
                .OrderBy(x => x.Orden).Take(4).Select(x => new MImagenIntro { Fecha = x.Fecha, Id = x.Id, Titulo = x.Titulo, Orden = x.Orden, Imagen = (x.EsVideo?null:Convert.FromBase64String(x.Foto)), Video = (x.EsVideo ? x.Foto: null), EsVideo=x.EsVideo }).ToList();
            if (iamgenes.Count > 0)
            {
                uat.ImagenIntro = iamgenes;
            }
            uat.Status = 200;
            return uat;
        }

        [HttpPost]
        [Route("TraeCabecera")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MTraeCabeceraImagenIntroDTO TraeCabecera([FromBody] MTraeCabeceraImagenIntroDTO uat)
        {
            try
            {
                if (uat == null) uat = new MTraeCabeceraImagenIntroDTO();
                var imagenIntro = _context.ImagenIntro.Where(x => (uat.UltimaId == 0 || x.Id < uat.UltimaId) && x.Foto != null).OrderByDescending(x => x.Id).Take(2).Select(x => 
                new MCabeceraImagenIntro {
                    Id = x.Id,
                    Imagen = Convert.FromBase64String(x.Foto),
                    Titulo = x.Titulo, 
                    }).ToList();
                if (imagenIntro.Count > 0)
                {
                    uat.ImagenIntro = imagenIntro;
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
        
    }
}