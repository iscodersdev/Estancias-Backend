using DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Text;
using DAL.Mobile;

namespace DAL.DTOs.API
{
    public class NotificacionDetalleDTO
    {
        public int Id { get; set; }
        public string Fecha { get; set; }
        public string Titulo { get; set; }
        public string Texto { get; set; }
        public bool Leido { get; set; }
        public string FechaLeido { get; set; }
    }

    public class NotificacionDTO : RespuestaAPI
    {
        public int CantidadNotificaciones { get; set; }        
        public int CantidadNotificacionesNoLeida { get; set; }        
        public bool NotificacionNoLeida { get; set; }
        public List<NotificacionDetalleDTO> ListNotificaciones;
    }

    public class AcusoNotificacionDTO : RespuestaAPI
    {
        public int NotificaionId { get; set; }
    }

}
