using System;

namespace DAL.DTOs
{
    public class LoginApiDTO
    {
        public string Usuario { get; set; }
        public string Password { get; set; }
    }

    public class LoginApiRespuestaDTO
    {
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public string UAT { get; set; }
        public string Usuario { get; set; }
        public DateTime FechaHora { get; set; }
    }
}