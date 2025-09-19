using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.DTOs.Servicios
{
    public class UsuarioParaProcesarDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string NombreCompleto { get; set; }
        public string NroDocumento { get; set; }
        public string NroTarjeta { get; set; }
    }

    public class ResultadoCuotasDTO
    {
        // Esta clase representa el objeto plano que necesitas
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string Fecha { get; set; }
        public string NumeroCuota { get; set; }
        public string NumeroCuotaTotal { get; set; } = "0";
        public string Monto { get; set; }
    }
}
