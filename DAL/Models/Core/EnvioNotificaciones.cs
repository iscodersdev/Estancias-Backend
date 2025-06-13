using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DAL.Models.Core
{
    public class EnvioNotificaciones
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Texto { get; set; }
        public byte[] Foto { get; set; }
        public DateTime Fecha { get; set; }
        public bool Envio { get; set; }
    }

    public class EnvioNotificacionesDestinatarios
    {
        public int Id { get; set; }
        public virtual EnvioNotificaciones Notificacion { get; set; }
        public virtual Usuario Destinatario { get; set; }
        public bool Envio { get; set; }
    }
}
