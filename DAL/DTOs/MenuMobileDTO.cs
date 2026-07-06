using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.DTOs
{
    public class CrearMenuMobileDTO
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; }

        public string Descripcion { get; set; }
    }
    public class ActualizarMenuMobileDTO
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; }

        public string Codigo { get; set; }

        public string Descripcion { get; set; }

        public bool Activo { get; set; }
    }

    public class AsignarUsuarioMenuMobileDTO
    {
        [Required(ErrorMessage = "El DNI es obligatorio.")]
        public string Dni { get; set; }
    }
}
