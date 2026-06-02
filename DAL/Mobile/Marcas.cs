using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class MTraeBotonesDTO
    {
        public int Status { get; set; }
        public string Mesaje { get; set; }
        public List<MBotonesDTO> Botones { get; set; }
    }

    public class MBotonesDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Imagen { get; set; }
        public int Orden { get; set; }
    }

}
