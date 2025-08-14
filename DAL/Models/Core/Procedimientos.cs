using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DAL.Models.Core
{
    public class Procedimientos
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Codigo { get; set; }
        public int DiaEjecucion { get; set; }
        public bool Activo { get; set; }
    }

    public class LogProcedimientos
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public string Mesaje { get; set; }
        public string StatusCode { get; set; }
        public DateTime Fecha { get; set; }
        public long Tiempo { get; set; }
        public int RegistrosCreados { get; set; }
        public int RegistrosConErrores { get; set; }
        public virtual ICollection<LogResumenesTarjetas> DetalleErrores { get; set; }
    }

    public class LogResumenesTarjetas
    {
        public int Id { get; set; }
        public string UsuarioId { get; set; }
        public string Mensaje { get; set; }
        public DateTime Fecha { get; set; }
        public int LogProcedimientosId { get; set; }
    }
}
