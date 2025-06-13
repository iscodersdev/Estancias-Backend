using System.ComponentModel.DataAnnotations;

namespace DAL.Models.Core
{
    public class Inversor
    {
        public int Id { get; set; }
        [Display(Name = "Nombre del Inversor:")]
        public string Nombre { get; set; }
        [Display(Name = "Domicilio Legal:")]
        public string Domicilio { get; set; }
        [Display(Name = "CUIT:")]
        public string CUIT { get; set; }
        public virtual TasaInversor TasaActual { get; set; }
        public bool Activo { get; set; }
    }
    public class TasaInversor
    {
        public int Id { get; set; }
        [Display(Name = "Tasa:")]
        public decimal Tasa { get; set; }
        public int Inversor { get; set; }
    }
}
