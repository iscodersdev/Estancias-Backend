using DAL.Models.Core;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DAL.DTOs
{
    public class ListProductosDTO
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string DescripcionAmpliada { get; set; }
        public decimal Precio { get; set; }
        public string Oferta { get; set; }
        public string Financiable { get; set; }
        public string Rubro { get; set; }
    }
    public class ProductoDTO
    {
        public int ProveedorId { get; set; }
        public int ProductoId { get; set; }
        [Display(Name = "Descripción del Producto")]
        public string Descripcion { get; set; }
        [Display(Name = "Detalle del Producto")]
        public string DescripcionAmpliada { get; set; }
        [Display(Name = "Precio del Producto")]
        public decimal Precio { get; set; }
        [Display(Name = "Precio de Oferta del Producto")]
        public decimal? Oferta { get; set; }
        [Display(Name = "¿Es financiable en cuotas")]
        public bool Financiable { get; set; }
        public IEnumerable<SelectListItem> Rubros { get; set; }
        [Display(Name = "Rubro del Producto")]
        public int RubroSeleccionado { get; set; }
    }
    public class ListFinanciacionDTO
    {
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; }
        public decimal PrecioProducto { get; set; }
        public int? CantidadCuotasAdd { get; set; }
        public decimal? InteresCoutasAdd { get; set; }
        public List<FinanciacionDTO> ListaFinanciacion = new List<FinanciacionDTO>();
    }
    public class FinanciacionDTO
    {
        public int FinaciacionId { get; set; }
        public int CantidadCuotas { get; set; }
        public decimal InteresCuota { get; set; }
        public decimal ValorCuota { get; set; }
    }
}
