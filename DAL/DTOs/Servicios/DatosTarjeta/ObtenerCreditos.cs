using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.DTOs.Servicios.DatosTarjeta
{
    public class CreditoLOAN
    {
        public int IdSolicitud { get; set; }
        public string Producto { get; set; }
        public string Fecha { get; set; }
        public string FechaCobro { get; set; }
        public string Operacion { get; set; }
        public string Estado { get; set; }
        public string EstadoToolTip { get; set; }

        // NOTA: Los importes son string porque usan comas como decimales.
        // Deberás convertirlos a decimal manualmente si necesitas hacer cálculos.
        public string ImporteCredito { get; set; }
        public string CapitalPedido { get; set; }
        public string ImporteCuota { get; set; }
        public string CantidadCuotas { get; set; }
        public string ImporteGastos { get; set; }
        public string ImporteInteres { get; set; }
        public string ImporteImpuestos { get; set; }
        public string Tna { get; set; }
        public string Tea { get; set; }
        public string FechaProximoVencimiento { get; set; }
        public string Pendiente { get; set; }
    }

    // Clase Raíz para la respuesta completa
    public class ApiResponseObtenerCreditos
    {
        public ResultadoInfo Resultado { get; set; }
        public List<CreditoLOAN> Credito { get; set; }
    }
}
