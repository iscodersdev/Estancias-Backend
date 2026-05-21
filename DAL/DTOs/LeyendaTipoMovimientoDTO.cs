namespace DAL.DTOs
{
    public class LeyendaTipoMovimientoDTO
    {
        public int Id { get; set; }
        public string NombreMovimiento { get; set; }
        public string TextoLeyenda { get; set; }
        public bool Activo { get; set; }
    }

    public class LeyendaTipoMovimientoCreateRequest
    {
        public string NombreMovimiento { get; set; }
        public string TextoLeyenda { get; set; }
    }

    public class LeyendaTipoMovimientoUpdateRequest
    {
        public string NombreMovimiento { get; set; }
        public string TextoLeyenda { get; set; }
        public bool Activo { get; set; }
    }
}