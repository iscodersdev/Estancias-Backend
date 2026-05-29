namespace DAL.DTOs
{
    public class ProveedorProductoCreateDTO
    {
        public int ProductoId { get; set; }
        public string Producto { get; set; }
        public string Detalle { get; set; }
        public decimal Precio { get; set; }
        public decimal PrecioOferta { get; set; }
        public bool Financiable { get; set; }
        public int RubroId { get; set; }
    }
}