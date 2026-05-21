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

    public class CuentaCorrienteDetalleDTO
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public string ClienteNombre { get; set; }
        public int ConceptoId { get; set; }
        public string ConceptoNombre { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime? Vencimiento { get; set; }
        public string Observaciones { get; set; }
        public decimal Importe { get; set; }
        public decimal Saldo { get; set; }
    }

    public class CuentaCorrienteCreateRequest
    {
        public DateTime Fecha { get; set; }
        public DateTime? Vencimiento { get; set; }
        public string Observaciones { get; set; }
        public decimal Importe { get; set; }
        public decimal Saldo { get; set; }
        public int ClienteId { get; set; }
        public int ConceptoId { get; set; }
    }

    public class CuentaCorrienteUpdateRequest
    {
        public DateTime Fecha { get; set; }
        public DateTime? Vencimiento { get; set; }
        public string Observaciones { get; set; }
        public decimal Importe { get; set; }
        public decimal Saldo { get; set; }
        public int ClienteId { get; set; }
        public int ConceptoId { get; set; }
    }
}