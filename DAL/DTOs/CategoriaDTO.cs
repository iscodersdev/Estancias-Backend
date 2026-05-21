namespace DAL.DTOs
{
    public class CategoriasDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }
    }

    public class CategoriaCreateRequest
    {
        public string Nombre { get; set; }
        public bool Activo { get; set; }
    }

    public class CategoriaUpdateRequest
    {
        public string Nombre { get; set; }
        public bool Activo { get; set; }
    }
}