using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Models.Core
{
    public class Proveedor
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public Int64 CUIT { set; get; }
        public string RazonSocial { get; set; }
        public string Domicilio { get; set; }
        public virtual List<ProveedorRubro> Rubros { get; set; }
        public virtual Empresas Empresa { get; set; }
        public byte[] Foto { get; set; }

        public bool Activo { get; set; }
    }
    public class ProveedorRubro
    {
        public int Id { get; set; }
        public virtual Proveedor Proveedor { get; set; }
        public virtual Rubro Rubro { get; set; }
    }
    public class Rubro
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool Activo { get; set; }
        public virtual List<ProveedorRubro> Proveedores { get; set; }
    }

    public class Producto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public byte[] Foto { get; set; }
        public virtual Proveedor Proveedor { get; set; }
        public virtual Rubro Rubro { get; set; }
        public bool Activo { get; set; }
        public string DescripcionAmpliada { get; set; }
        public decimal Precio { get; set; }
        public decimal? Oferta { get; set; }
        public bool Financiable { get; set; }
        public virtual List<Financiacion> FinanciacionProducto { get; set; }
        public virtual TipoProducto TipoProducto { get; set; }
    }

    public class Financiacion
    {
        public int Id { get; set; }
        public virtual Producto Producto { get; set; }
        public int CantidadCuotas { get; set; }
        public decimal InteresesPorCuota { get; set; }
    }
    public class TipoProducto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
    }
    public class Talles
    {
        public int Id { get; set; }
        public string Abreviado { get; set; }
        public string Descripcion { get; set; }
    }

    public class TallesProductos
    {
        public int Id { get; set; }
        public virtual Producto Producto { get; set; }
        public virtual Talles Talles{ get; set; }
    }

    public class Venta
    {
        public int Id { get; set; }
        public virtual Producto Producto { get; set; }
        
    }
    public class ProductoPrecompra
    {
        public int Id { get; set; }
        public virtual Producto Producto { get; set; }
        public virtual Clientes Cliente { get; set; }
        public DateTime FechaPrecompra { get; set; }
        public DateTime FechaAnulacion { get; set; }
        public DateTime FechaConfirmacion { get; set; }
        public EstadoPrecompra Estado { get; set; }
    }
    public enum EstadoPrecompra
    {
        Precompra,
        CompraEfectuada,
        Anulado
    }

    public class ProductoAdjuntos
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public byte[] Foto { get; set; }
        public string Extension { get; set; }
        public virtual Producto Producto { get; set; }
        public DateTime Fecha { get; set; }
    }
}
