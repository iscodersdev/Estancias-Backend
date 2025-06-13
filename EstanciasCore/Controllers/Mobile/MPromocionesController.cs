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

    public class MPromocionesController : BaseController
    {
        private readonly UserService<Usuario> _userManager;
        public EstanciasContext _context;
        public MPromocionesController(EstanciasContext context, UserService<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("TestPromociones")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MTraeNovedadesDTO TestPromociones()
        {
            return new MTraeNovedadesDTO { Mensaje= " desde alla",Status=200};
        }

        [HttpPost]
        [Route("TraePromociones")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MTraePromocionesDTO TraePromociones([FromBody] MTraePromocionesDTO uat)
        {
            var Uat = _context.UAT.FirstOrDefault(x => x.Token == uat.UAT);
            if (Uat == null)
            {
                uat.Status = 500;
                uat.Mensaje = "UAT Invalida";
                return uat;
            }

            var promociones = _context.Promociones.Where(x => x.Empresa.Id == Uat.Cliente.Empresa.Id && (uat.UltimaId == 0 || x.Id < uat.UltimaId) && x.Foto != null && x.Vencimiento==false).OrderBy(x => x.Orden)
                .Select(x => new MPromociones { PromocionFija= x.PromocionFija, Fecha = x.Fecha, Texto = x.Texto, Id = x.Id, Titulo = x.Titulo, Subtitulo=x.Subtitulo, Link = x.Link, QR = x.QR, Imagen = Convert.FromBase64String(x.Foto), Orden = x.Orden }).ToList();

            var promocionesVencimiento = _context.Promociones
                .Where(x => x.Vencimiento == true &&
                            x.FechaDesde.Date <= DateTime.Now.Date &&
                            x.FechaHasta.Date >= DateTime.Now.Date)
                .OrderBy(x => x.Orden)
                .Select(x => new MPromociones
                {
                    PromocionFija = x.PromocionFija,
                    Fecha = x.Fecha,
                    Texto = x.Texto,
                    Id = x.Id,
                    Titulo = x.Titulo,
                    Subtitulo = x.Subtitulo,
                    Link = x.Link,
                    QR = x.QR,
                    Imagen = Convert.FromBase64String(x.Foto),
                    Orden = x.Orden
                })
                .ToList();

            if (promociones.Count > 0 || promocionesVencimiento.Count > 0)
            {
                uat.Promociones = promociones.Concat(promocionesVencimiento).OrderBy(x => x.Orden).ToList();
            }
            uat.Status = 200;
            return uat;
        }

        [HttpPost]
        [Route("TraeCabecera")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MTraeCabeceraPromoDTO TraeCabecera([FromBody] MTraeCabeceraPromoDTO uat)
        {
            try
            {
                if (uat == null) uat = new MTraeCabeceraPromoDTO();
                var promociones = _context.Promociones.Where(x => (uat.UltimaId == 0 || x.Id < uat.UltimaId) && x.Foto != null && x.Vencimiento==false).OrderBy(x => x.Orden).Take(2).Select(x => 
                new MCabeceraPromo {
                    Id = x.Id,
                    Imagen = Convert.FromBase64String(x.Foto),
                    Titulo = x.Titulo, 
                    Subtitulo = x.Subtitulo,
                    Detalle = x.Texto,
                    TextoBoton = x.TextoBoton,
                    QR = x.QR
                    }).ToList();

                var promocionesVencimiento = _context.Promociones
                    .Where(x => x.Vencimiento == true &&
                                x.FechaDesde.Date <= DateTime.Now.Date &&
                                x.FechaHasta.Date >= DateTime.Now.Date)
                    .OrderBy(x => x.Orden)
                    .Take(2)
                    .Select(x => new MCabeceraPromo
                    {
                        Id = x.Id,
                        Imagen = Convert.FromBase64String(x.Foto),
                        Titulo = x.Titulo,
                        Subtitulo = x.Subtitulo,
                        Detalle = x.Texto,
                        TextoBoton = x.TextoBoton,
                        QR = x.QR
                    })
                    .ToList();

                if (promociones.Count > 0 || promocionesVencimiento.Count > 0)
                {
                    uat.Promociones = promociones.Concat(promocionesVencimiento).OrderBy(x => x.Orden).ToList();
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
        [Route("SolicitarPromocion")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MPromocionQR SolicitarPromocion([FromBody] MSolicitarPromocionDTO uat)
        {
            MPromocionQR promocion = new MPromocionQR();
            var Uat = _context.UAT.FirstOrDefault(x => x.Token == uat.UAT);
            if (Uat == null)
            {
                promocion.Status = 500;
                promocion.Mensaje = "UAT Invalida";
                return promocion;
            }

            var promociones = _context.Promociones.Where(x => x.Empresa.Id == Uat.Cliente.Empresa.Id && (uat.PromocionId==x.Id) && x.Foto != null).FirstOrDefault();

            PromocionesQR promocionesQR = new PromocionesQR()
            {
                Fecha = DateTime.Now,
                Texto = promociones.Texto,
                Subtitulo= promociones.Subtitulo,
                Titulo = promociones.Titulo,
                Foto = promociones.Foto,
                Promociones = promociones,
                Cliente = Uat.Cliente,
                Activo = true,
            };

            _context.PromocionesQR.Add(promocionesQR);
            _context.SaveChanges();

            string Hash = GenerarHash(promocionesQR.Id);
            string QR = GenerarQR(Hash, "http://portalestancias.com.ar/api/MPromociones/ValidarPromocion?hash=");           
            //string QR = GenerarQR(Hash, "https://localhost:44371/api/MPromociones/ValidarPromocion?hash=");           

            promocionesQR.QR = QR;
            promocionesQR.Hash = Hash;

            _context.PromocionesQR.Update(promocionesQR);
            _context.SaveChanges();

            promocion.Status = 200;

            if (promociones!=null)
            {
                promocion = new MPromocionQR { 
                    PromocionQRId = promocionesQR.Id, 
                    QR = QR ,
                    Fecha = promocionesQR.Fecha, 
                    Texto = promocionesQR.Texto,
                    Titulo = promocionesQR.Titulo,
                    Activo = true,
                    Imagen = Convert.FromBase64String(promocionesQR.Foto),
                };
            }
            return promocion;
        }

        private string GenerarHash(int Id)
        {
            string Hash = "";
            MD5 md5hash = MD5.Create();
            byte[] data = md5hash.ComputeHash(Encoding.UTF8.GetBytes(Id.ToString()));
            for (int i = 0; i < data.Length; i++)
            {
                Hash+=data[i].ToString("x2");
            }
            return Hash;
        }
        private string GenerarQR(string Hash, string url)
        {
            string QR = "";
            string contenido = url + Hash;
            var stream = new MemoryStream();
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(contenido, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            qrCodeImage.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
            QR = Convert.ToBase64String(stream.ToArray());
            return QR;
        }

        [HttpGet]
        [Route("ValidarPromocion")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public ValdiarPromocionDTO ValidarPromocion(string Hash)
        {
            try
            {
                var promocion = _context.PromocionesQR.FirstOrDefault(x => x.Hash == Hash);
                if (promocion != null)
                {
                    if (promocion.Activo == true)
                    {
                        promocion.Activo = false;
                        promocion.FechaUtilizado = DateTime.Now;
                        _context.PromocionesQR.Update(promocion);
                        _context.SaveChanges();
                        return new ValdiarPromocionDTO { Status = 200, Canjeado = true, Mensaje = "La promoción se utilizo con éxito." };
                    }
                    else
                    {
                        return new ValdiarPromocionDTO { Status = 200, Canjeado = false, Mensaje = "Esta promocion ya no es válida." };
                    }
                }
                else
                {
                    return new ValdiarPromocionDTO { Status = 200, Canjeado = false, Mensaje = "La promoción no existe." };
                }

            }
            catch (Exception e)
            {
                return new ValdiarPromocionDTO { Status = 500, Canjeado = false, Mensaje = "Error al leer la promoción." };
            }
        }
    }
}