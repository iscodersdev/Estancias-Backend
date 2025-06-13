using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.DTOs
{
    public class EnvioNotificacionDTO
    {
        public int TipoDeEnvio { get; set; }
        public string TituloNotificacion { get; set; }
        public string DistribucionNotificacion { get; set; }
        public string NroDocumentoNotificacion { get; set; }
        public string TextoNotificacion { get; set; }
        public IFormFile File { get; set; }
    }

}
