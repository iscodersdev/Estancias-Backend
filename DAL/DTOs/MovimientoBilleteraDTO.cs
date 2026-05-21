using System;

namespace DAL.DTOs
{
    public class MovimientoBilleteraDTO
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public int TipoMovimientoId { get; set; }
        public string TipoMovimientoNombre { get; set; }
        public decimal Monto { get; set; }
        public string QR { get; set; }
        public string CBU { get; set; }
    }

    public class MovimientoBilleteraCreateRequest
    {
        public DateTime Fecha { get; set; }
        public int TipoMovimientoId { get; set; }
        public decimal Monto { get; set; }
        public string QR { get; set; }
        public string CBU { get; set; }
    }

    public class MovimientoBilleteraUpdateRequest
    {
        public DateTime Fecha { get; set; }
        public int TipoMovimientoId { get; set; }
        public decimal Monto { get; set; }
        public string QR { get; set; }
        public string CBU { get; set; }
    }
}