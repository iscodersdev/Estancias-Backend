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

namespace EstanciasCore.Controllers
{
    [Route("api/[controller]")]

    public class MBasicosController : BaseController
    {
        private readonly UserService<Usuario> _userManager;
        public EstanciasContext _context;
        public MBasicosController(EstanciasContext context, UserService<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("TraeLinkCatalogo")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MCatalogoDTO TraeLinkCatalogo()
        {
            MCatalogoDTO catalogoDTO = new MCatalogoDTO();
            try
            {
                catalogoDTO.Status = 200;
                catalogoDTO.Mensaje = "Correcto";
                Catalogo catalogo = _context.Catalogo.Where(x => x.Activo==true).FirstOrDefault();
                if (catalogo!=null)
                {
                    catalogoDTO.Link = catalogo.Link;
                }
                else
                {
                    catalogoDTO.Status = 404;
                    catalogoDTO.Mensaje = "NotFound";
                }
            }
            catch (Exception e)
            {
                catalogoDTO.Status = 500;
                catalogoDTO.Mensaje = e.Message;
            }

            return catalogoDTO;
        }        
        
    }
}