using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class MTraeNovedadesDTO
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public int UltimaId { get; set; }
        public string Mensaje { get; set; }
        public List<MNovedad> Novedades { get; set; }
    }
    public class MNovedad
    {
        public Int64 Id { get; set; }
        public string Titulo { get; set; }
        public DateTime Fecha { get; set; }
        public string Texto { get; set; }
        public byte[] Imagen { get; set; }
    }
    public class MTraeCabeceraDTO
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public int UltimaId { get; set; }
        public string Mensaje { get; set; }
        public List<MCabecera> Novedades { get; set; }
    }
    public class MCabecera
    {
        public Int64 Id { get; set; }
        public byte[] Imagen { get; set; }
        public string Titulo { get; set; }
        public string Subtitulo { get; set; }        
        public string Detalle { get; set; }
        public string TextoBoton { get; set; }
        public bool Align { get; set; } = false;
    }
    public class MTraeCuentaCorrienteDTO
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public List<MCuentaCorriente> CuentaCorriente { get; set; }
    }
    public class MCuentaCorriente
    {
        public Int64 Id { get; set; }
        public string Concepto { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Importe { get; set; }
        public decimal Saldo { get; set; }
        public string Observaciones { get; set; }
    }
    public class NotificacionesPersonas
    {
        public int Id { get; set; }
        public virtual Clientes Cliente { get; set; }
        public DateTime FechaHora { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime? TomaConocimiento { get; set; }
    }
    public class MTraeNotificacionesDTO
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public int NotificacionId { get; set; }
        public string Mensaje { get; set; }
        public List<MNotificaciones> Notificaciones { get; set; }
    }
    public class MNotificaciones
    {
        public Int64 Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public bool TomaConocimiento { get; set; }
    }
}
