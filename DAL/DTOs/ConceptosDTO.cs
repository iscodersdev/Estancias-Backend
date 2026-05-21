namespace DAL.DTOs
{
    public class ConceptosDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Signo { get; set; }
    }

    public class ConceptoCreateRequest
    {
        public string Nombre { get; set; }
        public int Signo { get; set; }
    }

    public class ConceptoUpdateRequest
    {
        public string Nombre { get; set; }
        public int Signo { get; set; }
    }
}