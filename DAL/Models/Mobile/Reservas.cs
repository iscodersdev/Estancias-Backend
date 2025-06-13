using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Reservas
    {
        public int Id { get; set; }
        public virtual Horarios Horario { get; set; }
        [DisplayName("Fecha")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; }
        public String Observaciones { get; set; }
        public virtual Clientes Cliente { get; set; }
        public DateTime ?FechaAnulada {get;set;}
        public virtual Clientes ClienteAnula { get; set; }
    }

    public class ReservasDTO
    {
        public int Id { get; set; }
        public string Servicio { get; set; }
        public DateTime Fecha { get; set; }
        public string Horario { get; set; }
        public string Cliente { get; set; }
        public string Tipo { get; set; }
        public string Observaciones { get; set; }
    }
}