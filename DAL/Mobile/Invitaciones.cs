using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class MInvitaciones
    {
        public int AutorizacionId { get; set; }
        public string Apellido { get; set; }
        public string Nombres { get; set; }
        public string Patente { get; set; }
        public string Observaciones { get; set; }
        public string NumeroDocumento { get; set; }
        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }
        public int Estado { get; set; }
    }
    public class MGrabaInvitacion
    {
        public string UAT { get; set; }
        public string Apellido { get; set; }
        public string Nombres { get; set; }
        public string NumeroDocumento { get; set; }
        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }
        public string Patente { get; set; }
        public string Observaciones { get; set; }
        public int Estado { get; set; }
        public int SocioId { get; set; }
        public int Id { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
    }
    public class BorraInvitacion
    {
        public int AutorizacionId { get; set; }
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
    }
    public class ConfirmaInvitacionDTO
    {
        public int AutorizacionId { get; set; }
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
    }
    public class LinkInvitacion
    {
        public int AutorizacionId { get; set; }
        public string Link { get; set; }
        public string QR { get; set; }
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
    }
}
