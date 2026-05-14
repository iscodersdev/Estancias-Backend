using Commons.Identity;
using DAL.Models.Core;
using System.Collections.Generic;

namespace DAL.Models
{
    public class UsuariosCategorias
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Color { get; set; }
        public string CodigoColor { get; set; }
        public int Orden { get; set; }
        public bool Activo { get; set; }
    }

}