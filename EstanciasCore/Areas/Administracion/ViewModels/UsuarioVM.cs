using DAL.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EstanciasCore.Areas.Administracion.ViewModels
{
    public class UsuarioVM
    {
        public string UserId { get; set; }
        [Required(ErrorMessage = "Campo Requerido"), Display(Name = "Mail")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Mail { get; set; }
        [Required(ErrorMessage = "Campo Requerido"), Display(Name = "Password")]
        public string Password { get; set; }
        [DisplayName("¿Es Administrador?")]
        public bool Administrador { get; set; }
        public virtual Persona Persona { get; set; }
        public IEnumerable<SelectListItem> TipoDocumento { get; set; }
        public IEnumerable<SelectListItem> Pais { get; set; }
        public string TarjetaEstancia { get; set; }
    }

    public class UsuarioEstanciasVM
    {
        public string NroDocumento { get; set; }
        public string Mail { get; set; }
        [Required(ErrorMessage = "Campo Requerido"), Display(Name = "Password")]
        public string Password { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
        public string TarjetaEstancia { get; set; }
        public bool Error { get; set; }
        public string Mensaje { get; set; }
    }
}
