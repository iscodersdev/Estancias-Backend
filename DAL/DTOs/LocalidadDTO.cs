namespace DAL.DTOs
{
    public class LocalidadDTO
    {
        public int Id { get; set; }
        public string LocalidadNombre { get; set; }
        public string ProvinciaNombre { get; set; }
        public string GuarnicionNombre { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
    }

    public class LocalidadCreateRequest
    {
        public string Descripcion { get; set; }
        public int IdProvincia { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
    }

    public class LocalidadUpdateRequest
    {
        public string Descripcion { get; set; }
        public int IdProvincia { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
    }
}