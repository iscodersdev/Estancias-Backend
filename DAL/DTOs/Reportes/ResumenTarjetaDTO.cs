using DAL.DTOs.Servicios;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.DTOs.Reportes
{
    public class ResumenTarjetaDTO
    {
        public int Id { get; set; }
        public string NroTarjeta { get; set; }
        public string Periodo { get; set; }
        public int PeriodoId { get; set; }
        public string UsuarioId { get; set; }
        public string FechaVencimiento { get; set; }
        public decimal Monto { get; set; }
        public decimal Punitorios { get; set; }
    }


    public class DetallesCuotasResumenDTO
    {
        public string Fecha { get; set; }
        public string Concepto { get; set; }
        public string NroSolicitud { get; set; }
        public string NroCuota { get; set; }
        public string TotalDeCuotas { get; set; }
        public decimal Monto { get; set; }
    }

    public class TempalteResumenDTO
    {
        public decimal SaldoAnterior { get; set; }
        public decimal Pagos { get; set; }
        public decimal SaldoActual { get; set; }
        public decimal SaldoPendiente { get; set; }
        public decimal SaldoTotal { get; set; }
        public decimal Intereses { get; set; }
        public decimal Impuestos { get; set; }
        public string Nombre { get; set; }
        public string NroDocumento { get; set; }
        public string Mail { get; set; }
        public string NroSocio { get; set; }
        public string NroTarjeta { get; set; }
        public string Domicilio { get; set; }
        public string PeriodoDesde { get; set; }
        public string PeriodoHasta { get; set; }
        public string Vencimiento { get; set; }
        public List<DetallesCuotasResumenDTO> DetallesCuotas { get; set; }
        public List<ResultadoCuotasDTO> ConsumosAnteriores { get; set; }
        public List<ResultadoCuotasDTO> ConsumosDelMes { get; set; }
    }

}
