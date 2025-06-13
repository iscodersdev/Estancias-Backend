using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Servicio
    {
        public int ser_id { get; set; }
        public string ser_Nombre { get; set; }
        public string ser_Icono { get; set; }
    }
    public class Fecha_Servicio
    {
        public DateTime fse_Fecha { get; set; }
    }
    public class Hora_Servicio
    {
        public int hor_id { get; set; }
        public string hor_Nombre { get; set; }
        public bool hor_Anulable { get; set; }
    }
    public class Reserva
    {
        public string mut_UAT { get; set; }
        public int res_Ser_id { get; set; }
        public string res_Hor_id { get; set; }
        public string res_Observaciones { get; set; }
        public DateTime res_Fecha { get; set; }
        public string mensaje { get; set; }
        public int status { get; set; }

    }
    public class Lista_Reserva
    {
        public int res_id { get; set; }
        public string ser_Nombre { get; set; }
        public string res_Observaciones { get; set; }
        public DateTime res_Fecha { get; set; }
        public string res_Fechas { get; set; }
        public string hor_Nombre { get; set; }
        public string ser_Icono { get; set; }
        public string lot_Numero { get; set; }

    }
    public class Anula_Reserva
    {
        public string mut_UAT { get; set; }
        public int res_id { get; set; }
        public string mensaje { get; set; }
        public int status { get; set; }

    }

    public class MTraeServiciosDTO
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public int TipoServicioId { get; set; }
        public virtual List<MListaServicios> Servicios { get; set; }
    }
    public class MListaServicios
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
    public class MTraeReservasDTO
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public int TipoServicioId { get; set; }
        public virtual List<MListaReservas> Reservas { get; set; }
    }
    public class MListaReservas
    {
        public int Id { get; set; }
        public byte[] FondoServicio { get; set; }
        public string Nombre { get; set; }
        public string Fecha { get; set; }
        public string Horario { get; set; }
    }

    public class MTraeFechasDTO
    {
        public string UAT { get; set; }
        public int ServicioId { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public virtual List<MListaFechas> Fechas { get; set; }
    }
    public class MListaFechas
    {
        public string NombreFecha { get; set; }
        public DateTime Fecha { get; set; }
    }
    public class MTraeHorariosDTO
    {
        public string UAT { get; set; }
        public int ServicioId { get; set; }
        public DateTime Fecha { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public virtual List<MListaHorarios> Horarios { get; set; }
    }
    public class MListaHorarios
    {
        public int Id { get; set; }
        public string NombreHorario { get; set; }
        public int CupoDisponible { get; set; }
    }
    public class MGrabaReservaDTO
    {
        public string UAT { get; set; }
        public DateTime Fecha { get; set; }
        public int HorarioId { get; set; }
        public string Observaciones { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
    }
    public class MAnulaReservaDTO
    {
        public string UAT { get; set; }
        public int ReservaId { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
    }
    public class MTraeEmpresasDTO
    {
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public virtual List<MListaEmpresas> Empresas { get; set; }
    }
    public class MListaEmpresas
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public byte[] Logo { get; set; }
    }
    public class MUserDestroyDTO
    {
        public string UAT { get; set; }
        public int UserId { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
    }
}
