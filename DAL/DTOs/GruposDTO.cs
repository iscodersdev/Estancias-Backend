namespace DAL.DTOs
{
    public class GruposDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    public class GrupoCreateRequest
    {
        public string Nombre { get; set; }
    }

    public class GrupoUpdateRequest
    {
        public string Nombre { get; set; }
    }
}