using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class Periodo
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public bool Activo { get; set; }
    }

    public class MovimientoTarjeta
    {
        public int Id { get; set; }
        public string NroSolicitud { get; set; }
        public string NombreComercio { get; set; }
        public string NroCuota { get; set; }
        public string CantidadCuotas { get; set; }
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
        public virtual Periodo Periodo { get; set; }
        public virtual Usuario Usuario { get; set; }
        public bool Pagado { get; set; } = false;
        public DateTime FechaPago { get; set; }
    }

    public class ResumenTarjeta
    {
        public int Id { get; set; }
        public string NroComprobante { get; set; }
        public decimal Monto { get; set; }
        public decimal MontoAdeudado { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public int PeriodoId { get; set; }
        [ForeignKey("PeriodoId")]
        public virtual Periodo Periodo { get; set; }
        public string UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; }
        public byte[] Adjunto { get; set; }
    }

    public class DistribucionResumen
    {
        public int Id { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual Periodo Periodo { get; set; }
        public virtual ResumenTarjeta ResumenTarjeta { get; set; }
        public virtual CanalesDistribucion CanalesDistribucion { get; set; }
        public string Estado { get; set; }
        public DateTime Fecha { get; set; }
    }

    public class CanalesDistribucion
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal Activo { get; set; }
    }
}