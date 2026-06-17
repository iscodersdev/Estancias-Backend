using System;
using System.Collections.Generic;

namespace DAL.DTOs
{
    public class MovimientoBilleteraDTO
    {
        public int Id { get; set; }

        public DateTime Fecha { get; set; }

        public TipoMovimientoBilleteraDTO TipoMovimiento { get; set; }

        public decimal Monto { get; set; }

        public string CBU { get; set; }
    }

    public class MovimientoBilleteraCreateDTO
    {
        public int TipoMovimientoId { get; set; }

        public decimal Monto { get; set; }

        public string CBU { get; set; }
    }

    public class MovimientoBilleteraUpdateDTO
    {
        public int Id { get; set; }

        public int TipoMovimientoId { get; set; }

        public decimal Monto { get; set; }

        public string CBU { get; set; }
    }

    public class MovimientoBilleteraResponseDTO
    {
        public object Data { get; set; }

        public int Status { get; set; }

        public string Mensaje { get; set; }
    }

    public class MovimientoBilleteraListadoResponseDTO
    {
        public List<MovimientoBilleteraDTO> MovimientosBilletera { get; set; }
    }
}