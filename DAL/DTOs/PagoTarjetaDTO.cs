using DAL.Models.Core;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.DTOs
{
    public class PagoTarjetaDTO
    {
        public int Id { get; set; }
        public string Persona { get; set; }
        public string NroDocumento { get; set; }
        public string Usuario  { get; set; }
        public string NroTarjeta { get; set; }
        public DateTime FechaVencimiento { get; set; } = new DateTime();
        public Decimal MontoAdeudado { get; set; }
        public DateTime FechaPagoProximaCuota { get; set; } = new DateTime();
        public virtual EstadoPago EstadoPago { get; set; }
        public byte[] ComprobantePago { get; set; }
    }


    public class PagoTarjetaDataTableDTO
    {
        public int Id { get; set; }
        public string Persona { get; set; }
        public string NroDocumento { get; set; }
        public string Usuario { get; set; }
        public string NroTarjeta { get; set; }
        public string FechaVencimiento { get; set; }
        public string MontoAdeudado { get; set; }
        public string FechaPagoProximaCuota { get; set; }
        public string FechaComprobante { get; set; }
        public string EstadoPago { get; set; }
        public int EstadoPagoId { get; set; }
        public bool ComprobantePago { get; set; }
        public int FechaOrden { get; set; }
    }
}
