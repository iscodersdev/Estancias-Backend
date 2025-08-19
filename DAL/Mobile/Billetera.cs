using DAL.Models;
using DAL.Models.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Mobile
{
    public class SaldoDTO : RespuestaAPI
    {
        public decimal Saldo { get; set; }
    }

    public class CVUBilleteraDTO : RespuestaAPI
    {
        public string CVU { get; set; }
        public string Alias { get; set; }
    }


    public class ListaMovimientoDTO : RespuestaAPI
    {
        public List<MovimientoBilleteraDTO> Movimientos { get; set; }
    }

    public class MovimientoBilleteraDTO
    {
        public decimal Monto { get; set; }
        public string TipoMovimiento { get; set; }
        public DateTime Fecha { get; set; }
    }


    public class ListaPagoTarjetaDTO : RespuestaAPI
    {
        public int id { get; set; }
        public List<PagoTarjetaDTO> PagoTarjetaDTO { get; set; }
    }
    public class PagoTarjetaDTO: RespuestaAPI
    {
        public int id { get; set; }
        public int Persona { get; set; }
        public string NroTarjeta { get; set; }
        public string FechaVencimiento { get; set; }
        public string MontoAdeudado { get; set; }
        public string MontoInformado { get; set; }
        public string FechaPagoProximaCuota { get; set; }
        public string FechaComprobante { get; set; }
        public virtual EstadoPago EstadoPago { get; set; }
        public byte[] ComprobantePago { get; set; }
        public string CBU { get; set; }
        public string alias { get; set; }
    }

    public class PagoTarjetaNewDTO : RespuestaAPI
    {
        public int id { get; set; }
        public int Persona { get; set; }
        public string NroTarjeta { get; set; }
        public string FechaVencimiento { get; set; }
        public string MontoAdeudado { get; set; }
        public string FechaPagoProximaCuota { get; set; }
        public string FechaComprobante { get; set; }
        public virtual EstadoPago EstadoPago { get; set; }
        public byte[] ComprobantePago { get; set; }
        public string CBU { get; set; }
        public string alias { get; set; }
        public string MontoInformado { get; set; }
    }


    public class ComprobantesDTO : RespuestaAPI
	{
		public int PersonaId { get; set; }
        public List<ListComprobantesDTO> ListComprobantes;
	}

	public class ListComprobantesDTO
	{
		public string NroTarjeta { get; set; }
		public string FechaVencimiento { get; set; }
		public string MontoAdeudado { get; set; }
		public string FechaPagoProximaCuota { get; set; }
		public virtual EstadoPago EstadoPago { get; set; }
		public string EstadoPagoDescripcion { get; set; }
		public byte[] ComprobantePago { get; set; }
        public string FechaComprobante { get; set; }
    }


	//Aca trae el movimiento de tarjetas. Devuelva ok o no. Si es no que lo lleve al inicial

	public class ListaMovimientoTarjetaDTO : RespuestaAPI
    {
        public long NroTarjeta { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public long NroDocumento { get; set; }
        public long CantMovimientos { get; set; }
        public string Resultado { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string MontoAdeudado { get; set; }
        public string ProximaFechaPago { get; set; }
        public string TotalProximaCuota { get; set; }
        public string FechaPagoProximaCuota { get; set; }
        public string MontoDisponible { get; set; }
        public bool CuotaVencida { get; set; }
        public string LeyendaCuotaVencida { get; set; }
        public bool ContieneLeyenda { get; set; }
        public string Leyenda { get; set; }
        public string Telefono { get; set; }
        public int tipomovimiento { get; set; }

        public List<MovimientoTarjetaDTO> MovimientosTarjeta { get; set; }
        public List<MovimientoTarjetaDTO> MovimientosTarjetaSuma { get; set; }
        public List<DetalleMovimientoTarjetaDTO> DetalleMovimientosTarjeta { get; set; }
    }

    public class MovimientoTarjetaDTO
    {
        public string Monto { get; set; }
        public string TipoMovimiento { get; set; }
        public string Fecha { get; set; }
        public string Recargo { get; set; }
    }

	public class ListDetalleCuotaDTO
	{
		public string Monto { get; set; }
		public string Recargo { get; set; }
		public string Fecha { get; set; }
	}

	public class DetalleMovimientoTarjetaDTO
    {
        public string Monto { get; set; }
        public DateTime Fecha { get; set; }
        public string NroCuota { get; set; }
        public string NumeroSolicitud { get; set; }     

    }


    public class ListaTarjetasDTO : RespuestaAPI
    {
        public List<TarjetasBilleteraDTO> Tarjetas { get; set; }
    }

    public class TarjetasBilleteraDTO
    {
        public string Titular { get; set; }
        public string Numero { get; set; }
        public string Vencimiento { get; set; }
    }

    public class ListaCuentasDTO : RespuestaAPI
    {
        public List<CuentaBilleteraDTO> Cuentas { get; set; }
    }

    public class CuentaBilleteraDTO
    {
        public string CBU { get; set; }
        public string Descripcion { get; set; }
    }

    public class ListaBilleterasDTO : RespuestaAPI
    {
        public List<BilleteraAsociadaDTO> Billeteras { get; set; }
    }

    public class BilleteraAsociadaDTO
    {
        public string Titular { get; set; }
        public string CVU { get; set; }
    }


    public class ListaMediosPagoDTO : RespuestaAPI
    {
        public List<MedioPagoDTO> MediosPago { get; set; }
    }

    public class MedioPagoDTO
    {
        public int Id { get; set; }
        public TipoMedioPago TipoMedioPago { get; set; }
        public string Descripcion { get; set; }
        public string DetalleAdicional { get; set; }
    }

    public enum TipoMedioPago
    {
        TipoBilletera = 1,
        TipoTarjeta = 2,
        TipoCuenta = 3
    }

    public class EnvioBilleteraDTO : RespuestaAPI
    {
        public string CVU { get; set; }
        public string Monto { get; set; }
    }

    public class ClienteBilleteraDTO : RespuestaAPI
    {
        public string CVU { get; set; }
        public string Nombre { get; set; }
        public string Foto { get; set; }
    }


    public class AltaTarjetaDTO : RespuestaAPI
    {
        public string Numero { get; set; }
        public string Titular { get; set; }
        public string Vencimiento { get; set; }
    }

    public class AltaCuentaDTO : RespuestaAPI
    {
        public string Numero { get; set; }
        public string Titular { get; set; }
        public string CBU { get; set; }
        public string Alias { get; set; }
        public bool Terceros { get; set; }
    }

    public class DetallePagoBarrasDTO : RespuestaAPI
    {
        public string CodigoBarras { get; set; }
        public string NombreServicio { get; set; }
        public decimal Monto { get; set; }
    }

    public class ConfirmaPagoBarrasDTO : RespuestaAPI
    {
        public string CodigoBarras { get; set; }
        public int MetodoPagoId { get; set; }
        public TipoMedioPago TipoMedioPago { get; set; }
    }

    public class IngresoDineroDTO : RespuestaAPI
    {
        public string Monto { get; set; }
        public int MetodoPagoId { get; set; }
        public TipoMedioPago TipoMedioPago { get; set; }
    }


    public class RetiroDineroDTO : RespuestaAPI
    {
        public string Monto { get; set; }
        public int MetodoPagoId { get; set; }
        public TipoMedioPago TipoMedioPago { get; set; }
    }


    public class MovimientoLoanDTO
    {
        public string UAT { get; set; }
        public string NroDocumento { get; set; }
        public string NroTarjeta { get; set; }
        public int Tipomovimiento { get; set; }
        public int CantMovimientos { get; set; }
    }

}
