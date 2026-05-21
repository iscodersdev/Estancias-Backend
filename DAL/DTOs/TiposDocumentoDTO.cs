namespace DAL.DTOs
{
    public class ListTiposDocumentoDTO
    {
        public int Id { get; set; }
        public string NroDocumento { get; set; }
    }

    public class TipoDocumentoCreateRequest
    {
        public string Descripcion { get; set; }
    }

    public class TipoDocumentoUpdateRequest
    {
        public string Descripcion { get; set; }
    }
}