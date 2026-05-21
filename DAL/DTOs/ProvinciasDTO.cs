namespace DAL.DTOs
{
    public class ProvinciaDTO
    {
        public int Id { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public string Descripcion { get; set; }
        public string DescripcionCompleta { get; set; }
    }

    public class ProvinciaCreateRequest
    {
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public string Descripcion { get; set; }
        public string DescripcionCompleta { get; set; }
    }

    public class ProvinciaUpdateRequest
    {
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public string Descripcion { get; set; }
        public string DescripcionCompleta { get; set; }
    }
}