using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Promociones
    {
        public int Id { get; set; }
        [DisplayName("Fecha")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; }
        public string Titulo { get; set; }
        public string Link { get; set; }
        public string Subtitulo { get; set; }
        public string Foto { get; set; }
        public string Texto { get; set; }
        public string TextoBoton { get; set; }
        public bool PromocionFija { get; set; }
        public bool Publica { get; set; }
        public bool QR { get; set; }
        public virtual Empresas Empresa { get; set; }
        public virtual Colores Color { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public bool Vencimiento { get; set; }
        public int Orden { get; set; }
    }

    public class PromocionesQR
    {
        public int Id { get; set; }
        public virtual Clientes Cliente { get; set; }
        public virtual Promociones Promociones { get; set; }
        public string Hash { get; set; }
        public string Titulo { get; set; }
        public string Subtitulo { get; set; }
        public string Foto { get; set; }
        public string Texto { get; set; }
        public string QR { get; set; }
        [DisplayName("Fecha")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; }
        [DisplayName("Fecha Utilizado")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FechaUtilizado { get; set; }
        public bool Activo { get; set; }
    }


}