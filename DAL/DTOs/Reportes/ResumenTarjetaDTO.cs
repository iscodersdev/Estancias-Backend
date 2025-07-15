using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.DTOs.Reportes
{
    public class ResumenTarjetaDTO
    {
        public int Id { get; set; }
        public string NroTarjeta { get; set; }
        public string Periodo { get; set; }
        public int PeriodoId { get; set; }
        public string UsuarioId { get; set; }
        public string FechaVencimiento { get; set; }
        public decimal MontoAdeudado { get; set; }
    }    
}
