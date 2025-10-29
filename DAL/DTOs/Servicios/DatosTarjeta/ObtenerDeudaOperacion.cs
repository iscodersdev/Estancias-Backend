using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.DTOs.Servicios.DatosTarjeta
{
    public class DeudaOperacionLOAN
    {
        public string NumeroOperacion { get; set; }
        public string Compania { get; set; }
        public string Producto { get; set; }
        public string FechaMora { get; set; }
        public decimal DeudaActualizada { get; set; }
        public string Nombre { get; set; }
        public decimal ImporteCuota { get; set; }
        public string CodigoCompania { get; set; }
    }

    // Clase Raíz para la respuesta completa
    public class DeudaApiResponseLOAN
    {
        // La propiedad se llama "resultado" en minúscula para coincidir con el JSON
        public ResultadoInfo Resultado { get; set; }
        public List<DeudaOperacionLOAN> DeudasOperacion { get; set; }
    }
}
