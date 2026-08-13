using System;

namespace DAL.Models
{
    public class SolicitudDeTarjeta
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string DNI { get; set; }
        public string NumeroTarjeta { get; set; }
        public string Email { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Domicilio { get; set; }
        public DateTime FechaSolicitud { get; set; } = DateTime.Now;
        public DateTime FechaDeRechazoAprobacion { get; set; } = DateTime.Now;
        public virtual EstadoSolicitudDeTarjeta Estado{ get; set; }
    }

    public class EstadoSolicitudDeTarjeta
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; } = true;
    }
}
