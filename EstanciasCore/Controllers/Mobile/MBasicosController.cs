using Commons.Controllers;
using Commons.Identity.Services;
using DAL.Data;
using DAL.DTOs;
using DAL.DTOs.API;
using DAL.Models;
using EstanciasCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;

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
        public MCatalogosDTO TraeLinkCatalogo()
        {
            MCatalogoDTO catalogoDTO = new MCatalogoDTO();
            MCatalogosDTO catalogoResponse = new MCatalogosDTO();
            try
            {
                List<Catalogo> catalogo = _context.Catalogo.Where(x => x.Activo == true).ToList();
                if (catalogo != null)
                {
                    catalogoResponse.Catalogos = catalogo.Select(c => new MCatalogoDTO()
                    {
                        Link = c.Link,
                        NombreDeMarca = c.Marca != null ? c.Marca.Nombre : "Sin Marca"
                    }).ToList();

                    catalogoResponse.Status = 200;
                    catalogoResponse.Mensaje = "Correcto";
                }
                else
                {
                    catalogoResponse.Status = 404;
                    catalogoResponse.Mensaje = "NotFound";
                }
            }
            catch (Exception e)
            {
                catalogoResponse.Status = 500;
                catalogoResponse.Mensaje = e.Message;
            }

            return catalogoResponse;
        }

    }
}