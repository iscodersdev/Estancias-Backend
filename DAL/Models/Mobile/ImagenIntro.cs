using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class ImagenIntro
    {
        public int Id { get; set; }
        [DisplayName("Fecha")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; }
        public string Titulo { get; set; }
        public string Foto { get; set; }
        public int Orden { get; set; }
        public bool EsVideo { get; set; }
        public virtual Empresas Empresa { get; set; }
    }

}