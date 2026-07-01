using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class MenuMobile
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public bool Activo { get; set; }
    }

    public class MenuMobileUsuariosHabilitados
    {
        public int Id { get; set; }
        public virtual MenuMobile MenuMobile { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}