using System;
using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class Persona
    {
        public int Id { get; set; }
        public virtual TipoDocumento TipoDocumento { get; set; }
        public string NroDocumento { get; set; }
        [Required(ErrorMessage = "Ingrese el apellido")]
        public string Apellido { set; get; }
        [Required(ErrorMessage = "Ingrese el nombre")]
        public string Nombres { set; get; }
        public string Cuil { get; set; }
        public virtual Paises Pais { get; set; }
        [DataType(DataType.Date)]
        public DateTime? FechaNacimiento { get; set; }
        public int CantidadHijos { get; set; }
        public byte[] Foto { get; set; }

        public DateTime FechaActualizacion { get; set; } = DateTime.Now;
        public string Email { get; set; }
        public bool TokenEnviado { get; set; }
        public bool ValidandoTarjeta { get; set; }
        public string NroTarjeta { get; set; }
        public string FechaVencimiento { get; set; }
        public int GetEdad()
        {
            try
            {
                var today = DateTime.Today;
                var edad = today.Year - FechaNacimiento?.Year;
                if (FechaNacimiento?.Date > today.AddYears((int)-edad)) edad--;

                return (int)edad;
            }
            catch (Exception)
            {
                return 0;
            }

        }

        public string GetFechaNacimiento()
        {
            return FechaNacimiento == null ? "" : ((DateTime)FechaNacimiento).ToString("dd/MM/yyyy");
        }

        public string GetNombreCompleto()
        {
            return Apellido + " " + Nombres;
        }

        public string GetDocumentoCompleto()
        {
            return TipoDocumento?.Descripcion == null ? "NumeroDocumento - " + NroDocumento : TipoDocumento?.Descripcion + " - " + NroDocumento;
        }

        public string GetTipoDocumento()
        {
            return TipoDocumento?.Descripcion == null ? "NumeroDocumento" : TipoDocumento?.Descripcion;
        }


    }



}