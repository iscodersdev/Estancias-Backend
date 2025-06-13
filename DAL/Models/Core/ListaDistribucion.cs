using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DAL.Models.Core
{
    public class ListaDistribucion
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        [DisplayName("Descripción")]
        public string Descripcion { get; set; }

        public bool Activo { get; set; }
    }

    public class DistribucionDestinatarios
    {
        public int Id { get; set; }
        public virtual ListaDistribucion ListaDistribucion { get; set; }
        public virtual Usuario Destinatario { get; set; }
    }
}
