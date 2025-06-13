using System;
using System.Collections.Generic;


namespace DAL.Models
{
    public class MAccesosDTO
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public int FilaActual { get; set; }
        public string Mensaje { get; set; }
        public List<MAccesos> Accesos { get; set; }
    }
    public class MAccesos
    {
        public Int64 Id { get; set; }
        public DateTime? FechaHora { get; set; }
        public string Puesto { get; set; }
        public string Tipo { get; set; }
        public string Deuda { get; set; }
    }
    public class MGrabaAccesoDTO
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public string Token { get; set; }
        public string Coordenadas { get; set; }
    }
    public class AccesosDTO
    {
        public int Id { get; set; }
        public string Apellido { get; set; }
        public string Nombres { get; set; }
        public string FechaHora { get; set; }
        public string FechaOrden { get; set; }
        public string Puesto { get; set; }
        public string Club { get; set; }
        public string Estado { get; set; }
        public int EstadoId { get; set; }
        public string NumeroDocumento { get; set; }
        public string Categoria { get; set; }
        public int TurnoId { get; set; }
        public string Turno { get; set; }
    }
    public class EstadosDeudas
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
    public class Accesos
    {
        public int Id { get; set; }
        public virtual Clientes Cliente { get; set; }
        public virtual Puestos Puesto { get; set; }
        public virtual TiposAccesos TipoAcceso { get; set; }
        public string Coordenadas { get; set; }
        public bool ValidadoPorGPS { get; set; }
        public DateTime FechaHora { get; set; }
        public string Observaciones { get; set; }
        public virtual UAT UATPuesto { get; set; }
        public virtual Decimal Deuda { get; set; }
        public bool SinCuentaCorriente { get; set; }
        public virtual EstadosDeudas EstadoDeuda {get;set;}
        public string Turnos { get; set; }
    }
    public class TiposAccesos
    {
        public int Id { get; set; }
        public String Nombre { get; set; }
    }
    public class Puestos
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public virtual Empresas Empresa { get; set; }
        public virtual TipoPuesto Tipo { get; set; }
        public string Coordenadas { get; set; }
        public virtual List<PuestosCodigos> PuestoCodigos { get; set; } = new List<PuestosCodigos>();
        public DateTime? FechaBaja { get; set; }

    }
    public class PuestosDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; }
    }

    public class TipoPuesto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    public class PuestosCodigos
    {
        public int Id { get; set; }
        public virtual Puestos Puesto { get; set; }
        public DateTime ValidoDesde { get; set; }
        public DateTime ValidoHasta { get; set; }
        public string HashQR { get; set; }
        public byte[] Imagen { get; set; }
    }
}
