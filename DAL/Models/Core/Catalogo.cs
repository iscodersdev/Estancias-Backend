using System;
using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class Catalogo
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Link { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public bool Activo { get; set; }
    }

}