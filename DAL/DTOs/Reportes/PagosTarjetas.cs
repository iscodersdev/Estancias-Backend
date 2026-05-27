using DAL.Models.Core;
using System;
using System.Collections.Generic;

namespace DAL.DTOs.Reportes
{
    public class LiquidacionCuota
    {
        public int Id { get; set; }
        public Cuota Cuota { get; set; } = new Cuota();
        public EstadoCuota Estado { get; set; }
    }

    public class Cuota
    {
        public int Id { get; set; }
        public Prestamo Prestamo { get; set; } = new Prestamo();
        public decimal MontoCuota { get; set; }
    }

    public class Prestamo
    {
        public int Id { get; set; }
        public Cliente Cliente { get; set; } = new Cliente();
    }

    public class Cliente
    {
        public int Id { get; set; }
        public int Legajo { get; set; }
        public Persona Persona { get; set; } = new Persona();
    }

    public class Persona
    {
        public int Id { get; set; }
        public string NroDocumento { get; set; } = "";
        public string ApellidoNombre { get; set; } = "";
    }

    public enum EstadoCuota
    {
        PedienteLiquidar,
        Liquidada
    }

    public class BocaDePagoInfo
    {
        public string Descripcion { get; set; } = "General";
    }

    public class FiltroPagosViewModel
    {
        public string FechaDesde { get; set; } = "";
        public string FechaHasta { get; set; } = "";
        public int EstadoId { get; set; }
        public int PersonaId { get; set; }
        public string NombrePersona { get; set; } = "";
        public string Monto { get; set; } = "";

        public int Start { get; set; } = 0;
        public int Length { get; set; } = 10;

        public string SearchValue { get; set; } = "";
        public string SortColumn { get; set; } = "";
        public string SortDirection { get; set; } = "";

        public List<PagoTarjeta> Pagos { get; set; } = new List<PagoTarjeta>();
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }
    }

    public class PagoTarjetaReporteDTO
    {
        public int Id { get; set; }

        public int PersonaId { get; set; }
        public string Cliente { get; set; } = "";
        public string Apellido { get; set; } = "";
        public string Nombres { get; set; } = "";
        public string NroDocumento { get; set; } = "";

        public string NroTarjeta { get; set; } = "";
        public string Observacion { get; set; } = "";

        public DateTime FechaVencimiento { get; set; } = DateTime.MinValue;
        public string FechaVencimientoTexto { get; set; } = "";

        public decimal MontoAdeudado { get; set; }
        public string MontoAdeudadoTexto { get; set; } = "";

        public decimal MontoInformado { get; set; }
        public string MontoInformadoTexto { get; set; } = "";

        public DateTime FechaPagoProximaCuota { get; set; } = DateTime.MinValue;
        public string FechaPagoProximaCuotaTexto { get; set; } = "";

        public DateTime FechaComprobante { get; set; } = DateTime.MinValue;
        public string FechaComprobanteTexto { get; set; } = "";

        public DateTime FechaDePago { get; set; } = DateTime.MinValue;
        public string FechaDePagoTexto { get; set; } = "";

        public string EstadoPago { get; set; } = "";
        public string EstadoPagoTexto { get; set; } = "";
        public string EstadoCssClass { get; set; } = "";

        public bool TieneComprobantePago { get; set; }
        public string ComprobantePagoUrl { get; set; } = "";

        public string Acciones { get; set; } = "";
    }

    public class PagoTarjetaReporteDataTableResponseDTO
    {
        public string draw { get; set; } = "";
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public List<PagoTarjetaReporteDTO> data { get; set; } = new List<PagoTarjetaReporteDTO>();
    }

    public class PersonaComboReporteDTO
    {
        public string Text { get; set; } = "";
        public int Value { get; set; }
        public string Subtext { get; set; } = "";
        public string Icon { get; set; } = "";
    }

    public class PagoTarjetaExportDTO
    {
        public string Cliente { get; set; } = "";
        public string NroDocumento { get; set; } = "";
        public string FechaVencimiento { get; set; } = "";
        public string FechaComprobante { get; set; } = "";
        public string Estado { get; set; } = "";
        public decimal MontoAdeudado { get; set; }
        public decimal MontoInformado { get; set; }
    }

    public class FiltroClientesReporteViewModel
    {
        public string FechaIngresoDesdeFiltro { get; set; } = "";
        public string FechaIngresoHastaFiltro { get; set; } = "";

        public int Start { get; set; } = 0;
        public int Length { get; set; } = 10;

        public string SearchValue { get; set; } = "";
        public string SortColumn { get; set; } = "";
        public string SortDirection { get; set; } = "";
    }

    public class ClienteReporteDTO
    {
        public int Id { get; set; }

        public int PersonaId { get; set; }
        public string UsuarioId { get; set; } = "";

        public string Mail { get; set; } = "";
        public string NroDocumento { get; set; } = "";
        public string NombreCompleto { get; set; } = "";
        public string NroTarjeta { get; set; } = "";

        public DateTime FechaIngreso { get; set; } = DateTime.MinValue;
        public string FechaIngresoTexto { get; set; } = "";
    }

    public class ClienteReporteDataTableResponseDTO
    {
        public string draw { get; set; } = "";
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public List<ClienteReporteDTO> data { get; set; } = new List<ClienteReporteDTO>();
    }

    public class ClienteReporteExportDTO
    {
        public string Mail { get; set; } = "";
        public string NroDocumento { get; set; } = "";
        public string NombreCompleto { get; set; } = "";
        public string NroTarjeta { get; set; } = "";
        public string FechaIngreso { get; set; } = "";
    }
}