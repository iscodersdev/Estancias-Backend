using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class Marcas
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string NomAliasbre { get; set; }
        public string CBU { get; set; }
        public string WhatsApp { get; set; }
        public byte[] Imagen { get; set; }
        public int Orden { get; set; }
        public bool Activo { get; set; }
    }
}