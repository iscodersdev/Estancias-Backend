using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class Invitaciones
    {
        public int Id { get; set; }
        public string Apellido { get; set; }
        public string Nombres { get; set; }
        public byte[] Foto { get; set; }
        public Int64 NumeroDocumento { get; set; }
        public virtual Clientes Cliente { get; set; }
        [DisplayName("Desde")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Desde { get; set; }
        [DisplayName("Hasta")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Hasta { get; set; }
        public string Link { get; set; }
        public string QR { get; set; }
        public string Hash { get; set; }
        public String Observaciones { get; set; }
        public String Patente { get; set; }
        public DateTime? Baja { get; set; }
        public int Estado { get; set; }
        public DateTime? Completado { get; set; }
        public void CambiarImagen(byte[] bytes)
        {
            Foto = bytes;
        }
    }

    public class CompletaInvitaciones
    {
        public int Id { get; set; }
        public DateTime? Completado { get; set; }
        public virtual Clientes Cliente { get; set; }
        [Required]
        public string Apellido { get; set; }
        [Required]
        public string Nombres { get; set; }
        [Required]
        public Int64 NumeroDocumento { get; set; }
        public string QR { get; set; }
        public String Observaciones { get; set; }
        [Required]
        public String Patente { get; set; }
        public string Hash { get; set; }
    }

}
