using DAL.Models.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.DTOs
{
    public class UsuarioBusquedaDTO
    {
        public string UserName { get; set; }
        public string DeviceId { get; set; }
        public string WonderPushDeviceId { get; set; }
        public string NroDocumento { get; set; }
        public string Apellidos { get; set; }
        public string Nombres { get; set; }
        public string NroTarjeta { get; set; }
        public string Celular { get; set; }
        public string FechaNacimiento { get; set; }   
        public int Usuario { get; set; }
        public int Persona { get; set; }
        public int Cliente { get; set; }
        public List<UsuarioBusquedaComprobanteDTO> Pagos { get; set; }
    }

    public class UsuarioBusquedaComprobanteDTO
    {
        public string FechaComprobante { get; set; }
        public byte[] ComprobantePago { get; set; }
    }
}
