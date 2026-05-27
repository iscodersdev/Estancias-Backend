using System.Collections.Generic;

namespace DAL.DTOs
{
    public class ProveedorDetalleDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string CUIT { get; set; }
        public string RazonSocial { get; set; }
        public string Domicilio { get; set; }
        public string Empresa { get; set; }
        public List<string> Rubros { get; set; }
        public List<ProveedorProductoDTO> Productos { get; set; }
    }

    public class ProveedorProductoDTO
    {
        public int Id { get; set; }
        public string Producto { get; set; }
        public string Rubro { get; set; }
        public string Detalle { get; set; }
        public decimal Precio { get; set; }
        public decimal PrecioOferta { get; set; }
        public bool Financiable { get; set; }
    }
}