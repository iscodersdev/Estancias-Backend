using Commons.Identity;
using Commons.Models;
using DAL.Models.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class LoginDTO
    {
        public string UsuarioId { get; set; }
        public string Password { get; set; }
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
    }
    public class ClientesDTO
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public int DNI { get; set; }
        public bool Activo { get; set; }
        public string Celular { get; set; }
        public string eMail { get; set; }
        public string Domicilio { get; set; }
        public string Codigo { get; set; }
    }

    public class PrestamosClienteDTO
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public int DNI { get; set; }
        public List<PrestamosDTO> Prestamos { get; set; }
    }
    public class PrestamosDTO
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
        public decimal Capital { get; set; }
        public int CantidadCuotas { get; set; }
        public decimal MontoCuota { get; set; }
        public decimal Saldo { get; set; }
        public int CuotasRestantes { get; set; }
        public DateTime Fecha { get; set; }
    }
    public class PrestamosCuotasDTO
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public int Id { get; set; }
        public List<CuotasDTO> Cuotas { get; set; }
    }
    public class CuotasDTO
    {
        public DateTime FechaCuota { get; set; }
        public decimal Importe { get; set; }
        public decimal Pagado { get; set; }
        public decimal Saldo { get; set; }
        public DateTime FechaPagado { get; set; }
        public int NroCuota { get; set; }
    }
    public class CampanaDTO
    {
        public int CantidadPersonas { get; set; }
        public int CantidadMinutos { get; set; }
        public int CantidadProvincias { get; set; }
        public int CantidadUnidades { get; set; }
    }
    public class PrestamosReportesDTO
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
        public string Cliente { get; set; }
        public string DNI { get; set; }
        public string Categoria { get; set; }
        public decimal Capital { get; set; }
        public int CantidadCuotas { get; set; }
        public decimal MontoCuota { get; set; }
        public string Estado { get; set; }
        public int EstadoId { get; set; }
        public string Fecha { get; set; }
        public string Celular { get; set; }
        public string eMail { get; set; }
    }

    public class RespuestaAPI
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }

    }

    public class RespuestaAPIWH : RespuestaAPI
    {
        public string CUIL { get; set; }
        public string Telefono { get; set; }
        public string ErrorMensaje { get; set; }
        public int Resultado { get; set; }
    }

    public class MockServicio
    {
        public int Id { get; set; }
        public string CodigoBarras { get; set; }
        public string CodigoServicioFactura { get; set; }
        public string NombreServicio { get; set; }
        public decimal Monto { get; set; }
    }

    public class CompraPorFinanciacionProductoDTO
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public int ProductoId { get; set; }
        public int FinanciacionProductoId { get; set; }
    }
    /*

    public class FinanciacionesProductoDTO
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public int ProductoId { get; set; }
        public List<FinanciacionDTO> Financiacion { get; set; }
    }*/

    /*
    public class FinanciacionDTO
    {
        public int Id { get; set; }
        public int CantidadCuota { set; get; }
        public decimal MontoCuota { set; get; }
    }*/
    public class MCompraProductoDTO
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public int ProductoId { get; set; }
        public List<PlanesDisponiblesDTO> PlanesDisponibles { get; set; }
    }


    public class MSolicitaPrestamoProductoDTO
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public int LineaPrestamoId { get; set; }
        public decimal ImporteSolicitado { get; set; }
        public int ProductoId { get; set; }
        public int TipoPersonaId { get; set; }
        public int CantidadCuotas { get; set; }
        public decimal MontoCuota { get; set; }
        public byte[] FotoDNIAnverso { get; set; }
        public byte[] FotoDNIReverso { get; set; }
        public byte[] FotoSosteniendoDNI { get; set; }
        public byte[] FotoReciboHaber { get; set; }
        public byte[] FotoCertificadoDescuento { get; set; }
        public byte[] LegajoElectronico { get; set; }
        public byte[] FirmaOlografica { get; set; }
        public List<MPrecancelacionesDTO> Precancelaciones { get; set; }
    }

    public class MTraeComprasProductos
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public List<MComprasDTO> Compras { get; set; }

    }


    public class MComprasDTO
    {
        public int Id { get; set; }
        public MProductosDTO Producto { get; set; }
        public DateTime FechaCompra { get; set; }
        public EstadoCompra Estado { get; set; }
        public TipoCompra TipoCompra { get; set; }
        public DateTime FechaAnulacion { get; set; }
    }
}