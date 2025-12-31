using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DAL.Models.Core
{
    public class EnvioNotificaciones
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Texto { get; set; }
        public byte[] Foto { get; set; }
        public DateTime Fecha { get; set; }
        public bool Envio { get; set; }
        public virtual NotificacionesPlantillas NotificacionesPlantillas { get; set; }
    }

    public class EnvioNotificacionesDestinatarios
    {
        public int Id { get; set; }
        public virtual EnvioNotificaciones Notificacion { get; set; }
        public virtual Usuario Destinatario { get; set; }
        public bool Envio { get; set; }
    }

    /*-------------------------------------------------- Subtipos de Notificaciones -------------------------------------------------------------*/
    public class NotificacionPersonalizada
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Texto { get; set; }
        public byte[] Imagen { get; set; }
        public bool ImagenGrande { get; set; }



        public virtual EnvioNotificaciones Notificacion { get; set; }
        public virtual Usuario Destinatario { get; set; }
        public bool Envio { get; set; }
    }

    public class TipoNotificacion
    {
        public int Id { get; set; }
        public virtual EnvioNotificaciones Notificacion { get; set; }
        public virtual Usuario Destinatario { get; set; }
        public bool Envio { get; set; }
    }

    public class NotificacionViewModelDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Titulo { get; set; }
        public string Mensaje { get; set; }
        public string ImagenUrl { get; set; }
        public string DeepLink { get; set; }
        public bool PreferLargeImage { get; set; }
        public bool AttachDataPayload { get; set; }
        public bool ShowPopup { get; set; }
    }


    /*-------------------------------------------------- Nuevo Notificaciones -------------------------------------------------------------*/



    public class Notificaciones
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Codigo { get; set; }
        public DateTime FechaEjecucion { get; set; }
        public DateTime FechaUltimaEjecucion { get; set; }
        public int Variable1 { get; set; }
        public int Variable2 { get; set; }
        public int Variable3 { get; set; }
        public bool Activo { get; set; }
        public virtual TipoNotificacionesProcedimientos TipoNotificacionesProcedimientos { get; set; }
        public virtual NotificacionesPlantillas NotificacionesPlantillas { get; set; }
        public virtual ListaDistribucion ListaDistribucion { get; set; }
    }

    public class TipoNotificacionesProcedimientos
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public bool Activo { get; set; }
    }
    public class NotificacionesPlantillas
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Titulo { get; set; }
        public string Mensaje { get; set; }
        public string ImagenUrl { get; set; }
        public string DeepLink { get; set; }
        public bool PreferLargeImage { get; set; }
        public bool Activo { get; set; } = true;
    }


}