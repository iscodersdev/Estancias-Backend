using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class MTraeImagenIntroDTO
    {
        public int Status { get; set; }
        public int UltimaId { get; set; }
        public string Mensaje { get; set; }
        public List<MImagenIntro> ImagenIntro { get; set; }
    }

    public class MImagenIntro
    {
        public Int64 Id { get; set; }
        public string Titulo { get; set; }
        public DateTime Fecha { get; set; }
        public int Orden { get; set; }
        public byte[] Imagen { get; set; }
        public string Video { get; set; }
        public bool EsVideo { get; set; }
    }
    public class MTraeCabeceraImagenIntroDTO
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public int UltimaId { get; set; }
        public string Mensaje { get; set; }
        public List<MCabeceraImagenIntro> ImagenIntro { get; set; }
    }
    public class MCabeceraImagenIntro
    {
        public Int64 Id { get; set; }
        public byte[] Imagen { get; set; }
        public string Titulo { get; set; }
        public string Subtitulo { get; set; }
        public string Detalle { get; set; }
        public string TextoBoton { get; set; }
        public bool Align { get; set; } = false;
    }

    public class MSolicitarImagenIntroDTO
    {
        public string UAT { get; set; }
        public int ImagenIntroId { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
    }


}
