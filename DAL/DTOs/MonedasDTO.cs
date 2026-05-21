namespace DAL.DTOs
{
    public class MonedasDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    public class MonedaCreateRequest
    {
        public string Nombre { get; set; }
    }

    public class MonedaUpdateRequest
    {
        public string Nombre { get; set; }
    }
}