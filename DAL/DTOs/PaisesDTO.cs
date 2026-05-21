namespace DAL.DTOs
{
   public class PaisesDTO
    {
        public int Id { get; set; }
        public string Nombre{ get; set; }
    }

    public class PaisCreateRequest
    {
        public string Nombre { get; set; }
    }

    public class PaisUpdateRequest
    {
        public string Nombre { get; set; }
    }
}
