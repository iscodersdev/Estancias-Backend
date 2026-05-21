namespace DAL.DTOs
{
    public class TiposClientesDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int CantidadActividadesSemanales { get; set; }
    }

    public class TipoClienteCreateRequest
    {
        public string Nombre { get; set; }
        public int CantidadActividadesSemanales { get; set; }
    }

    public class TipoClienteUpdateRequest
    {
        public string Nombre { get; set; }
        public int CantidadActividadesSemanales { get; set; }
    }
}