using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class MTraeBannersDTO
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public int UltimaId { get; set; }
        public string Mensaje { get; set; }
        public List<MBanners> Banners { get; set; }
    }
    public class MBanners
    {
        public Int64 Id { get; set; }
        public string Titulo { get; set; }
        public string Subtitulo { get; set; }
        public string Link { get; set; }
        public DateTime Fecha { get; set; }
        public string Texto { get; set; }
        public string Video { get; set; }
        public bool BannerFijo { get; set; }
        public bool EsVideo { get; set; }
        public int Orden { get; set; }
        public byte[] Imagen { get; set; }
    }
    public class MTraeCabeceraBannersDTO
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public int UltimaId { get; set; }
        public string Mensaje { get; set; }
        public List<MCabeceraBanners> Banners { get; set; }
    }
    public class MCabeceraBanners
    {
        public Int64 Id { get; set; }
        public byte[] Imagen { get; set; }
        public string Titulo { get; set; }
        public string Subtitulo { get; set; }
        public string Detalle { get; set; }
        public string TextoBoton { get; set; }
        public bool Align { get; set; } = false;
    }

    public class MSolicitarBannersDTO
    {
        public string UAT { get; set; }
        public int BannerId { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
    }


}
