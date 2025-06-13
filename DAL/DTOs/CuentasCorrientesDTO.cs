using System;

namespace DAL.DTOs
{
    public class CuentasCorrientesDTO
    {
        public int Id { get; set; }
        public string ClienteNombre { get; set; }
        public string Fecha { get; set; }
        public string Vencimiento { get; set; }
        public decimal Saldo { get; set; }
    }
}
