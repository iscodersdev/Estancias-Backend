using System.Collections.Generic;

namespace DAL.DTOs
{
    public class BilleteraDTO
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public string ClienteNombre { get; set; }
        public string ClienteEmail { get; set; }
        public decimal Saldo { get; set; }
        public string QRCobro { get; set; }
        public string AliasCVU { get; set; }
        public string CVU { get; set; }
    }

    public class BilleteraCreateRequest
    {
        public int ClienteId { get; set; }
        public decimal Saldo { get; set; }
        public string QRCobro { get; set; }
        public string AliasCVU { get; set; }
        public string CVU { get; set; }
    }

    public class BilleteraUpdateRequest
    {
        public int ClienteId { get; set; }
        public decimal Saldo { get; set; }
        public string QRCobro { get; set; }
        public string AliasCVU { get; set; }
        public string CVU { get; set; }
    }

    public class BilleteraClienteComboDTO
    {
        public string Text { get; set; }
        public int Value { get; set; }
        public string Subtext { get; set; }
        public string Icon { get; set; }
    }
}