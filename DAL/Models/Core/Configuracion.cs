using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Models.Core
{
    public class Configuracion
    {
        public int Id { get; set; }
        public int Tipo { get; set; }
        public string TipoDescripcion { get; set; }
        public int Valor { get; set; }
        public string Observacion { get; set; }
    }
}
