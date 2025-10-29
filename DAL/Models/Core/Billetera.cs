using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DAL.Models.Core
{
        public class Billetera
        {
            public int Id { get; set; }
            public virtual Clientes Cliente { get; set; }
            public decimal Saldo { get; set; }
            public string QRCobro { get; set; }
            public string AliasCVU { get; set; }
            public string CVU { get; set; }
            public virtual List<Tarjeta> Tarjetas { get; set; }
            public virtual List<CuentaBancaria> Cuentas { get; set; }
            public virtual List<ServicioBilletera> Servicios { get; set; }
            public virtual List<MovimientoBilletera> Movimientos { get; set; }
            public virtual List<ContactosBilletera> Contactos { get; set; }

            public bool ChequeaDebito(decimal monto) => monto <= this.Saldo;

        }

        public class ContactosBilletera
        {
            public int Id { get; set; }
            public virtual Clientes ClienteContacto { get; set; }
            public string Detalle { get; set; }
            public DateTime FechaCreacion { get; set; } = DateTime.Now;
            public bool Activo { get; set; }
        }

        public class Tarjeta
        {
            public int Id { get; set; }
            public string Numero { get; set; }
            public string Titular { get; set; }
            public string Vencimiento { get; set; }
            public virtual Banco Banco { get; set; }
            public virtual InstitucionFinanciera InstitucionFinanciera { get; set; }
            public virtual List<MovimientoBilletera> Movimientos { get; set; }
        }

        public class CuentaBancaria
        {
            public int Id { get; set; }
            public string Numero { get; set; }
            public string CBU { get; set; }
            public string Titular { get; set; }
            public string Alias { get; set; }
            public string Descripcion { get; set; }
            public bool Terceros { get; set; }
            public virtual Banco Banco { get; set; }
            public virtual List<MovimientoBilletera> Movimientos { get; set; }

        }

        public class ServicioBilletera
        {
            public int Id { get; set; }
            public string CodigoServicioFactura { get; set; }
            public string Nombre { get; set; }
        }

        public class Banco
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
        }

        public class InstitucionFinanciera
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
        }

        public enum TipoMovimientoBilleteraEnum
        {
            EnvioBilletera = 1,
            IngresoDinero = 2,
            PagoServicio = 3,
            RetiroDinero = 4
        }

        public class TipoMovimientoBilletera
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
            public bool Credito { get; set; }
            public bool Debito { get; set; }
        }

        public enum TipoOrigenMovimiento
        {
            [DisplayName("Billetera")]
            Billetera = 1,
            [DisplayName("Tarjeta")]
            Tarjeta = 2,
            [DisplayName("Cuenta")]
            Cuenta = 3
        }

        public class OrigenMovimiento
        {
            public int Id { get; set; }
            public TipoOrigenMovimiento TipoOrigen { get; set; }
            public string Descripcion { get; set; }
            public int IdAsociado { get; set; }
        }

        public class MovimientoBilletera
        {
            public virtual OrigenMovimiento OrigenAsociado { get; set; }
            public int Id { get; set; }
            public DateTime Fecha { get; set; } = new DateTime();
            public virtual TipoMovimientoBilletera TipoMovimiento { get; set; }
            public decimal Monto { get; set; }
            public string QR { get; set; }
            public string CBU { get; set; }
        }
        public class PagoTarjeta
        {
            public int Id { get; set; }
            public virtual Persona Persona { get; set; }
            public string NroTarjeta { get; set; }
            public string Observacion { get; set; }
            public DateTime? FechaVencimiento { get; set; } = new DateTime();
            public Decimal MontoAdeudado { get; set; }
            public Decimal MontoInformado { get; set; }
            public DateTime? FechaPagoProximaCuota { get; set; } = new DateTime();
            public DateTime? FechaComprobante { get; set; } = new DateTime();
            public DateTime? FechaDePago{ get; set; } = new DateTime();
            public virtual EstadoPago EstadoPago { get; set; }
            public byte[] ComprobantePago { get; set; }
        }
    public class TipoMovimientoTarjeta
        {
            public int Id { get; set; }
            public string Nombre { get; set; }            
        }
        public enum EstadoPago
        {
            [DisplayName("Pendiente")]
            Pendiente = 1,
            [DisplayName("Pagado")]
            Pagado = 2,
            [DisplayName("Pago Confirmado")]
            Aprobado = 3,
            [DisplayName("Rechazado")]
            Rechazado = 4
        }
}

