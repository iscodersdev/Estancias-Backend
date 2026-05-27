using System;
using System.Collections.Generic;

namespace DAL.DTOs.Reportes
{
    public class CuponesReportesDTO
    {
        public int Id { get; set; }

        public int ClienteId { get; set; }
        public int PersonaId { get; set; }
        public int PremioId { get; set; }

        public string Cliente { get; set; } = "";
        public string Apellido { get; set; } = "";
        public string Nombres { get; set; } = "";
        public string NroDocumento { get; set; } = "";

        public DateTime Fecha { get; set; } = DateTime.MinValue;
        public string FechaTexto { get; set; } = "";

        public string Cupon { get; set; } = "";
        public string PremioNombre { get; set; } = "";
    }

    public class FiltroCuponesReportesViewModel
    {
        public string FechaDesde { get; set; } = "";
        public string FechaHasta { get; set; } = "";

        public int PersonaId { get; set; }
        public string NombrePersona { get; set; } = "";

        public int Start { get; set; } = 0;
        public int Length { get; set; } = 10;

        public string SearchValue { get; set; } = "";
        public string SortColumn { get; set; } = "";
        public string SortDirection { get; set; } = "";
    }

    public class CuponesReportesDataTableResponseDTO
    {
        public string draw { get; set; } = "";
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public List<CuponesReportesDTO> data { get; set; } = new List<CuponesReportesDTO>();
    }

    public class CuponesReportesExportDTO
    {
        public string Cliente { get; set; } = "";
        public string NroDocumento { get; set; } = "";
        public string Fecha { get; set; } = "";
        public string Cupon { get; set; } = "";
    }
}