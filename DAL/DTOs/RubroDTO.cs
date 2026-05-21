namespace DAL.DTOs
{
    public class RubroDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool Activo { get; set; }
    }

    public class RubroCreateRequest
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
    }

    public class RubroUpdateRequest
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool Activo { get; set; }
    }
}