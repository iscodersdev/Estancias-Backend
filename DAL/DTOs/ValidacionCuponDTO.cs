using System;
using System.Collections.Generic;

namespace DAL.DTOs
{
    public class ValidacionCuponResponseDTO
    {
        public bool Ok { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
    }

    public class ValidacionCuponListadoResponseDTO : ValidacionCuponResponseDTO
    {
        public bool EsBusqueda { get; set; }
        public List<ValidacionCuponDetalleDTO> Cupones { get; set; } = new List<ValidacionCuponDetalleDTO>();
    }

    public class ValidacionCuponDetalleDTO
    {
        public int Id { get; set; }

        public string Cliente { get; set; }

        public string NroDocumento { get; set; }

        public string Premio { get; set; }

        public string CodigoCupon { get; set; }

        public DateTime Fecha { get; set; }

        public DateTime FechaVencimientoCupon { get; set; }

        public bool Activo { get; set; }

        public bool Vencido { get; set; }

        public string Estado { get; set; }
    }

    public class ValidarCuponRequestDTO
    {
        public string Codigo { get; set; }
    }

    public class ValidarCuponResponseDTO : ValidacionCuponResponseDTO
    {
        public ValidacionCuponDetalleDTO Cupon { get; set; }
    }
}