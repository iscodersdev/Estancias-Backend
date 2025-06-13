namespace DAL.DTOs
{
    public class ClienteDTO
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
        public string NombreCompleto { get; set; }
        public string CUIL { get; set; }
        public string RazonSocial { get; set; }
        public string Empresa { get; set; }
        public string FechaIngreso { get; set; }
        public bool Estado { get; set; }
    }
}