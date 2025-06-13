using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class MTraePromocionesDTO
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public int UltimaId { get; set; }
        public string Mensaje { get; set; }
        public List<MPromociones> Promociones { get; set; }
    }
    public class MPromociones
    {
        public Int64 Id { get; set; }
        public string Titulo { get; set; }
        public string Subtitulo { get; set; }
        public string Link { get; set; }
        public DateTime Fecha { get; set; }
        public string Texto { get; set; }
        public bool PromocionFija { get; set; }
        public bool QR { get; set; }
        public int Orden { get; set; }
        public byte[] Imagen { get; set; }
    }
    public class MTraeCabeceraPromoDTO
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public int UltimaId { get; set; }
        public string Mensaje { get; set; }
        public List<MCabeceraPromo> Promociones { get; set; }
    }
    public class MCabeceraPromo
    {
        public Int64 Id { get; set; }
        public byte[] Imagen { get; set; }
        public string Titulo { get; set; }
        public string Subtitulo { get; set; }
        public string Detalle { get; set; }
        public string TextoBoton { get; set; }
        public bool Align { get; set; } = false;
        public bool QR { get; set; } = false;
        public int Orden { get; set; }
    }

    public class MSolicitarPromocionDTO
    {
        public string UAT { get; set; }
        public int PromocionId { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
    }

    public class MPromocionQR
    {
        public string UAT { get; set; }
        public int PromocionQRId { get; set; }
        public string QR { get; set; }
        public Int64 Id { get; set; }
        public string Titulo { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime FechaUlitizado { get; set; }
        public string Texto { get; set; }
        public byte[] Imagen { get; set; }
        public int PromocionId { get; set; }
        public int Status { get; set; }
        public bool Activo { get; set; }
        public string Mensaje { get; set; }           
        
    }

}
