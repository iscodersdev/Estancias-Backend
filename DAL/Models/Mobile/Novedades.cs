using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Novedades
    {
        public int Id { get; set; }
        [DisplayName("Fecha")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; }
        public string Titulo { get; set; }
        public string Subtitulo { get; set; }
        public string Foto { get; set; }
        public string Texto { get; set; }
        public string TextoBoton { get; set; }
        public bool Publica { get; set; }
        public virtual Empresas Empresa { get; set; }
        public virtual Colores Color { get; set; }
    }

    public class Colores
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
}