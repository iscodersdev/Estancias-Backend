using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.DTOs.AgilPagosDTO
{

    public class ValidarClienteDTO
    {
        public string email { get; set; }
        public string codigoPais { get; set; }
        public string codigoArea { get; set; }
        public string numero { get; set; }
    }


    public class NovedadCVUDTO
    {
        public string idNovedad { get; set; }
        public long cuit { get; set; }
        public string numeroCuenta { get; set; }
        public string cvu { get; set; }
        public int moneda { get; set; }
        public int idEstado { get; set; }
        public string accion { get; set; }
        public DateTime fechaSolicitud { get; set; }
        public DateTime fechaBaja { get; set; }
    }


    public class SaldoDTO
    {
        public int IdMoneda { get; set; }
        public float Importe { get; set; }
        public DateTime FechaSaldo { get; set; }
    }


    public class TransaccionesDTO
    {
        public string idTransaccion { get; set; }
        public int idTipoTransaccion { get; set; }
        public string numeroCuenta { get; set; }
        public float importe { get; set; }
        public int idMoneda { get; set; }
        public DateTime fechaOperacion { get; set; }
        public DateTime fechaContable { get; set; }
        public string observaciones { get; set; }
    }


    public class MovimientoEntidadDTO
    {
        public DateTime fechaDesde { get; set; }
        public DateTime fechaHasta { get; set; }
        public int pagina { get; set; }
        public int registrosPorPagina { get; set; }
    }

    #region Notificacion

    public class AvisoMovimientoEnEntidadDTO
    {
        public string idTransaccion { get; set; }
        public string idEntidad { get; set; }
        public int idTipoTransaccion { get; set; }
        public string numeroCuenta { get; set; }
        public float importe { get; set; }
        public int idMoneda { get; set; }
        public DateTime fechaOperacion { get; set; }
        public DateTime fechaContable { get; set; }
        public string observaciones { get; set; }
    }


    public class AvisoNovedadCuentasEnEntidadDTO
    {
        public int idNovedad { get; set; }
        public string idEntidad { get; set; }
        public long cuit { get; set; }
        public string numeroCuenta { get; set; }
        public int idTipoCuenta { get; set; }
        public int moneda { get; set; }
        public int idEstado { get; set; }
        public string accion { get; set; }
        public DateTime fechaSolicitud { get; set; }
        public DateTime fechaAprobacion { get; set; }
        public object fechaBaja { get; set; }
    }

    public class AvisoNovedadCuentasEnEntidadCvuDTO : AvisoNovedadCuentasEnEntidadDTO
    {
        public string cvu { get; set; }
    }


    public class ResponseNotificionApiDTO
    {
        public string pCodRespuesta { get; set; }
        public string pDescRespuesta { get; set; }
    }

    public class ResponseNotificionApiCvuDTO : ResponseNotificionApiDTO
    {
        public string cvu { get; set; }
    }

    #endregion

}
