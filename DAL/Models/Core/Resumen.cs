using System;
using System.ComponentModel.DataAnnotations;

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
}