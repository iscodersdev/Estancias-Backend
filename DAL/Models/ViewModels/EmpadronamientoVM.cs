using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.Models.ViewModels
{
    public class EmpadronamientoVM
    {
        public virtual Persona Persona { get; set; }
        public string Domicilio { get; set; }
        public string Localidad { get; set; }
        public virtual Provincia Provincia { get; set; }
        [DisplayName("CP")]
        public string CodigoPostal { get; set; }
        [Required(ErrorMessage = "El eMail es un dato requerido")]
        public string eMail { get; set; }
        public string Celular { get; set; }
        [DisplayName("Número de Afiliado")]
        [Required(ErrorMessage = "El Número de Afiliado es un dato requerido")]
        public string NumeroAfiliado { get; set; }
        [DisplayName("Fecha y Hora")]
        public DateTime FechaHora { get; set; }
        public DateTime? Validez { get; set; }

    }
}
