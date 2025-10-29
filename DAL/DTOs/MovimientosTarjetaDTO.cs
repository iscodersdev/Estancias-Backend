using DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.DTOs
{
    public class MovimientosTarjetaDTO : RespuestaAPI
    {       
        public int PeriodoId { get; set; }
        public string NroDocumento { get; set; }
        public byte[] Adjunto { get; set; }
    }

    public class TraePeriodosDTO : RespuestaAPI
    {
        public List<PeriodoDTO> Periodos { get; set; }
    }

    public class PeriodoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
    }
}
