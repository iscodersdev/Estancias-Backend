using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.DTOs.Servicios.DatosTarjeta
{
    public class CreditoDetalleLOAN
    {
        public int Id { get; set; }
        public string Fecha { get; set; }
        public string Cuota { get; set; }
        public string Estado { get; set; }

        // NOTA: Los importes son string porque usan comas como decimales.
        public string ImporteCuota { get; set; }
        public string ImportePunitorios { get; set; }
        public int IdTipoEntidad { get; set; }
    }

    // Clase Raíz para la respuesta completa
    public class ApiResponseCreditoDetalles
    {
        public ResultadoInfo Resultado { get; set; }
        public List<CreditoDetalleLOAN> CreditoDetalles { get; set; }
    }
}
