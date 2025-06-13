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

    public class MBannersController : BaseController
    {
        private readonly UserService<Usuario> _userManager;
        public EstanciasContext _context;
        public MBannersController(EstanciasContext context, UserService<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("TestBanners")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MTraeNovedadesDTO TestBanners()
        {
            return new MTraeNovedadesDTO { Mensaje= " desde alla",Status=200};
        }

        [HttpPost]
        [Route("TraeBanners")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MTraeBannersDTO TraeBanners([FromBody] MTraeBannersDTO uat)
        {
            var Uat = _context.UAT.FirstOrDefault(x => x.Token == uat.UAT);
            if (Uat == null)
            {
                uat.Status = 500;
                uat.Mensaje = "UAT Invalida";
                return uat;
            }

            //var banner = _context.Banners.Where(x => x.Empresa.Id == Uat.Cliente.Empresa.Id && (uat.UltimaId == 0 || x.Id < uat.UltimaId) && x.Foto != null).Where(x=>x.FechaDesde<=DateTime.Now && (x.FechaHasta>=DateTime.Now || x.Vencimiento==false)).Where(x=>x.Foto!=null || x.Video!=null).OrderBy(x => x.Orden)
            //    .Select(x => new MBanners { BannerFijo= x.BannerFijo, Fecha = x.Fecha, Texto = x.Texto, Id = x.Id, Titulo = x.Titulo, Subtitulo=x.Subtitulo, Link = x.Link, Imagen = (x.EsVideo ? null : Convert.FromBase64String(x.Foto)), Video = (x.EsVideo ? x.Foto : null), EsVideo=x.EsVideo }).ToList();

            var banner = _context.Banners.Where(x => x.FechaDesde<=DateTime.Now && (x.FechaHasta>=DateTime.Now || x.Vencimiento==false)).Where(x => x.Foto!=null || x.Video!=null).OrderBy(x => x.Orden)
                .Select(x => new MBanners { BannerFijo= x.BannerFijo, Orden=x.Orden, Fecha = x.Fecha, Texto = x.Texto, Id = x.Id, Titulo = x.Titulo, Subtitulo=x.Subtitulo, Link = x.Link, Imagen = (x.EsVideo ? null : Convert.FromBase64String(x.Foto)), Video = (x.EsVideo ? x.Video : null), EsVideo=x.EsVideo }).Take(3).ToList();


            if (banner.Count > 0)
            {
                uat.Banners = banner;
            }
            uat.Status = 200;
            return uat;
        }

        [HttpPost]
        [Route("TraeCabecera")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MTraeCabeceraBannersDTO TraeCabecera([FromBody] MTraeCabeceraBannersDTO uat)
        {
            try
            {
                if (uat == null) uat = new MTraeCabeceraBannersDTO();
                var banner = _context.Banners.Where(x => (uat.UltimaId == 0 || x.Id < uat.UltimaId) && x.Foto != null).Where(x => x.FechaDesde<=DateTime.Now && (x.FechaHasta>=DateTime.Now || x.Vencimiento==false)).OrderBy(x => x.Orden).Take(2).Select(x => 
                new MCabeceraBanners {
                    Id = x.Id,
                    Imagen = Convert.FromBase64String(x.Foto),
                    Titulo = x.Titulo, 
                    Subtitulo = x.Subtitulo,
                    Detalle = x.Texto,
                    TextoBoton = x.TextoBoton
                    }).ToList();
                if (banner.Count > 0)
                {
                    uat.Banners = banner;
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