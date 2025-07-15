using System;
using System.ComponentModel.DataAnnotations;

namespace DAL.Models.Core
{
    public class Procedimientos
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Codigo { get; set; }
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
        public int RegistrosActualizados { get; set; }
        public int RegistrosNuevos { get; set; }
    }
}
