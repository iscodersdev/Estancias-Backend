using System;
using System.ComponentModel.DataAnnotations;

namespace DAL.DTOs
{
    public class MorososDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo DNI es obligatorio.")]
        [Display(Name = "DNI")]
        public string DNI { get; set; }

        [Required(ErrorMessage = "El campo Nombre y Apellido es obligatorio.")]
        [Display(Name = "Nombre y Apellido")]
        public string NombreCompleto { get; set; }

        [Display(Name = "Fecha de Carga")]
        public DateTime FechaCarga { get; set; } = DateTime.Now;
    }
}
