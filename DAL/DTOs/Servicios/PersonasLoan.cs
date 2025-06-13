using System;
using System.ComponentModel.DataAnnotations;

namespace DAL.DTOs.Servicios
{
    public class PersonaLoan
    {
        public string NroDocumento { get; set; }
        public string Apellido { set; get; }
        public string Nombres { set; get; }
        public string Email { set; get; }
        public int Verificado { set; get; }
        public string NroTarjeta { set; get; }
        public string FechaNacimiento { set; get; }
    }

}