using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.DTOs.Servicios
{
    public class UsuarioParaProcesarDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string NombreCompleto { get; set; }
        public string NroDocumento { get; set; }
        public string NroTarjeta { get; set; }
    }
}
