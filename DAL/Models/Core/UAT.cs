using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class UAT
    {
        public int Id { get; set; }
        public virtual Persona Persona { get; set; }
        public virtual Clientes Cliente { get; set; }
        public virtual Usuario Usuario { get; set; }
        public string Token { get; set; }
        [DisplayName("Fecha y Hora")]
        public DateTime FechaHora { get; set; }
    }
}
