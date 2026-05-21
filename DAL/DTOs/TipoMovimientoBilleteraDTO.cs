namespace DAL.DTOs
{
    public class TipoMovimientoBilleteraDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public bool Credito { get; set; }
        public bool Debito { get; set; }
    }

    public class TipoMovimientoBilleteraCreateRequest
    {
        public string Nombre { get; set; }
        public bool Credito { get; set; }
        public bool Debito { get; set; }
    }

    public class TipoMovimientoBilleteraUpdateRequest
    {
        public string Nombre { get; set; }
        public bool Credito { get; set; }
        public bool Debito { get; set; }
    }
}