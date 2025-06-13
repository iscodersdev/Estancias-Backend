using Microsoft.AspNetCore.Mvc;
using QRCoder;
using DAL.Data;
using DAL.Models;
using System.Linq;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using Commons.Controllers;
using Commons.Identity.Services;

namespace EstanciasCore.Controllers
{
    [Route("api/[controller]")]

    public class MInvitacionesController : BaseController
    {
        private readonly UserService<Usuario> _userManager;
        public EstanciasContext _context;
        public MInvitacionesController(EstanciasContext context, UserService<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("TraeInvitaciones")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public List<MInvitaciones> TraeInvitaciones(string UAT)
        {
            UAT uat = _context.UAT.FirstOrDefault(x => x.Token == UAT);
            if (uat == null)
            {
                return null;
            }
            var Invitaciones = _context.Invitaciones.Where(x => x.Cliente.Id==uat.Cliente.Id && x.Baja==null && DateTime.Today<= x.Hasta.Date && DateTime.Today>=x.Desde.Date).OrderBy(x=>x.Apellido).ThenBy(x=>x.Nombres);
            List<MInvitaciones> invitaciones = new List<MInvitaciones>();
            foreach (var Invitacion in Invitaciones)
            {
                MInvitaciones invitacion = new MInvitaciones();
                invitacion.Nombres = Invitacion.Nombres;
                invitacion.Apellido = Invitacion.Apellido;
                invitacion.NumeroDocumento = Convert.ToString(Invitacion.NumeroDocumento);
                invitacion.Observaciones = Invitacion.Observaciones;
                invitacion.Patente = Invitacion.Patente;
                invitacion.Desde = Invitacion.Desde;
                invitacion.Hasta = Invitacion.Hasta;
                invitacion.AutorizacionId = Invitacion.Id;
                invitacion.Estado = Invitacion.Estado;
                invitaciones.Add(invitacion);
            }
            return invitaciones;
        }

        [HttpPost]
        [Route("GrabaInvitacion")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public IActionResult GrabaInvitacion([FromBody] MGrabaInvitacion invitacion)
        {
            UAT uat = _context.UAT.FirstOrDefault(x => x.Token == invitacion.UAT);
            if (uat == null)
            {
                invitacion.Status = 500;
                invitacion.Mensaje = "Usuario o Password Incorrecto";
                return Json(invitacion);
            }
            Invitaciones invitaciones = new Invitaciones();
            invitaciones.Apellido = invitacion.Apellido;
            invitaciones.Cliente = uat.Cliente;
            invitaciones.Patente = invitacion.Patente;
            invitaciones.Observaciones = invitacion.Observaciones;
            invitaciones.Nombres= invitacion.Nombres;
            invitaciones.Estado = invitacion.Estado;
            if (invitacion.NumeroDocumento == null)
            {
                invitaciones.NumeroDocumento = 0;
            }
            else
            {
                try
                {
                    invitaciones.NumeroDocumento = Convert.ToInt64(invitacion.NumeroDocumento);
                }
                catch
                {
                    invitaciones.NumeroDocumento = 0;
                }
            }
            if (invitacion.Estado == 1)
            {
                invitaciones.Desde = DateTime.Today;
                invitaciones.Hasta = DateTime.Today;
            }
            else
            {
                if (invitacion.Desde.Date.AddDays(-1)==DateTime.Today)
                {
                    invitaciones.Desde = DateTime.Today;
                }
                invitaciones.Hasta = invitacion.Hasta.Date;
            }
            _context.Invitaciones.Add(invitaciones);
            _context.SaveChanges();
            invitacion.Id = invitaciones.Id;
            invitacion.Mensaje = "Invitacion Grabada";
            invitacion.Status = 200;
            return Json(invitacion);
        }


        [HttpPost]
        [Route("BorraInvitacion")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public IActionResult BorraInvitacion([FromBody] BorraInvitacion borra_invitacion)
        {
            UAT uat = _context.UAT.FirstOrDefault(x => x.Token == borra_invitacion.UAT);
            if (uat == null)
            {
                borra_invitacion.Status = 500;
                borra_invitacion.Mensaje = "Usuario o Password Incorrecto";
                return Json(borra_invitacion);
            }
          Invitaciones autorizacion = _context.Invitaciones.Find(borra_invitacion.AutorizacionId);
           autorizacion.Baja = DateTime.Now;
            _context.SaveChanges();
            borra_invitacion.Mensaje = "Invitacion Borrada";
            borra_invitacion.Status = 200;
            return Json(borra_invitacion);
        }

        [Route("LinkInvitacion")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public IActionResult LinkInvitacion([FromBody] LinkInvitacion link_invitacion)
        {
            UAT uat = _context.UAT.FirstOrDefault(x => x.Token == link_invitacion.UAT);
            if (uat == null)
            {
                link_invitacion.Status = 500;
                link_invitacion.Mensaje = "Usuario o Password Incorrecto";
                return Json(link_invitacion);
            }
            Invitaciones invitacion = _context.Invitaciones.Find(link_invitacion.AutorizacionId);
            MD5 md5hash = MD5.Create();
            byte[] data = md5hash.ComputeHash(Encoding.UTF8.GetBytes(link_invitacion.AutorizacionId.ToString()));
            invitacion.Hash = "";
            for (int i = 0; i < data.Length; i++)
            {
              invitacion.Hash+=data[i].ToString("x2");
            }
            invitacion.Link = "http://www.homit.com.ar:612/invitaciones/CompletaInvitacion?hash=" + invitacion.Hash;
            var stream = new MemoryStream();
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(invitacion.Link, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            qrCodeImage.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
            invitacion.QR = "data:image/jpeg;base64," + Convert.ToBase64String(stream.ToArray());
            _context.Invitaciones.Update(invitacion);
            _context.SaveChanges();
            link_invitacion.Link = invitacion.Link;
            link_invitacion.QR = invitacion.QR;
            link_invitacion.Mensaje = "Link y QR Generados Correctamente";
            link_invitacion.Status = 200;
            return Json(link_invitacion);
        }
    }
}