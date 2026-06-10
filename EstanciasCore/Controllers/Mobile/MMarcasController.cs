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

    public class MMarcasController : BaseController
    {
        private readonly UserService<Usuario> _userManager;
        public EstanciasContext _context;
        public MMarcasController(EstanciasContext context, UserService<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("TraeBotones")]
        //[EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MTraeBotonesDTO TraeBotones()
        {
            MTraeBotonesDTO uat = new MTraeBotonesDTO();
            var botones = _context.Marcas.Where(x => x.Activo==true).OrderBy(x => x.Orden)
                .Select(x => new MBotonesDTO { Id = x.Id, Nombre= x.Nombre, Alias = x.NomAliasbre, CBU = x.CBU, Whatsapp = x.WhatsApp, Orden=x.Orden, Imagen = Convert.ToBase64String(x.Imagen) }).ToList();


            if (botones.Count > 0)
            {
                uat.Botones = botones;
            }
            uat.Status = 200;
            uat.Mesaje = "Exito";
            return uat;
        }
        
    }

}
