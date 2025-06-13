using DAL.Models.Core;
using System;
using System.Collections.Generic;

namespace DAL.Models
{
    public class MTraeProveedoresDTO
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public int? ProveedorId { get; set; }
        
        public List<ProveedoresDTO> Proveedores  { get; set; }
    }
    public class ProveedoresDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string CUIT { get; set; }
        public string RazonSocial { get; set; }
        public string Domicilio { get; set; }
        public byte[] Foto { get; set; }
    }
    public class MTraeClientesDTO
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public List<int> ClientesId { get; set; } = new List<int>();

        public List<MClienteDTO> Clientes { get; set; }
    }

    public class MClienteDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string NroDocumento { get; set; }
        public string Telefono { get; set; }
        public string Celular { get; set; }
        public string Domicilio { get; set; }
        public string Mail { get; set; }
        public int? LocalidadId { get; set; }
        public string Localidad { get; set; }
        public string DeviceId { get; set; }
        public int? GuarnicionId { get; set; }
    }
    public class MTraeRubrosDTO
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public List<RubrosDTO> Rubros { get; set; }
    }

    public class RubrosDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
    }
    public class MTraeProductosDTO
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public  int? ProductoId { get; set; }
        public int? ProveedorId { get; set; }
        public int? ClienteId { get; set; }
        public int? Cantidad { get; set; }
        public List<MProductosDTO> Productos { get; set; }
    }

    public class MTraeProductosPorRubroDTO
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public int RubroId { get; set; }
        public List<MProductosDTO> Productos { get; set; }
    }

    public class MProductosDTO
    {
        public Int64 Id { get; set; }
        public string Descripcion { get; set; }
        public int? ProveedorId { get; set; }
        public string ProveedorNombre { get; set; }
        public int? ClienteId { get; set; }
        public string ClienteNombre { get; set; }        
        public string ClienteMail { get; set; }
        public string ClienteCel { get; set; }
        public string Rubro { get; set; }
        public decimal Precio { get; set; }
        public string DescripcionAmpliada { get; set; }
        public List<byte[]> Fotos { get; set; }
        public List<Talles> Talles { get; set; }
        public string TipoProducto { get; set; }
        public bool Activo { get; set; }

    }
    public class MVentaDTO
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public int ProductoId { get; set; }
        public int PrestamoId { get; set; }
    }

    public class MProducto
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public int? ProveedorId { get; set; }
        public int? ClienteId { get; set; }
        public int? ProductoId { get; set; }
        public string Descripcion { get; set; }
        public byte[] Foto { get; set; }
        public int? RubroId { get; set; }
        public bool Activo { get; set; }
        public string DescripcionAmpliada { get; set; }
        public decimal Precio { get; set; }
        public decimal? Oferta { get; set; }
        public int? Cantidad { get; set; }

    }
    public class MProductoClienteCompra
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public List<int> Productos { get; set; }
    }
    public class MListaProductosClienteCompra
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public List<MProductosClientesComprados> Productos { get; set; }
    }
    public class MProductosClientesComprados
    {
        public string FechaInteres { get; set; }
        public Int64 Id { get; set; }
        public string Descripcion { get; set; }
        public int? ProveedorId { get; set; }
        public string ProveedorNombre { get; set; }
        public int? ClienteId { get; set; }
        public string ClienteNombre { get; set; }
        public string ClienteMail { get; set; }
        public string ClienteCel { get; set; }
        public string Rubro { get; set; }
        public decimal Precio { get; set; }
        public string DescripcionAmpliada { get; set; }
        public byte[] Foto { get; set; }
    }
    public class MProductosPrecompras
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public List<int> Productos { get; set; }
    }
    public class MTraeProductosPrecompraDTO
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public List<MProductoPrecompraDTO> Productos { get; set; }

    }
    public class MProductoPrecompraDTO
    {
        public Int64 Id { get; set; }
        public string Descripcion { get; set; }
        public int? ProveedorId { get; set; }
        public string ProveedorNombre { get; set; }
        public string Rubro { get; set; }
        public decimal Precio { get; set; }
        public string DescripcionAmpliada { get; set; }
        public byte[] Foto { get; set; }
        public DateTime FechaPrecompra { get; set; }
        public EstadoPrecompra Estado { get; set; }

    }
}
