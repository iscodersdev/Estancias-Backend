using System;
using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class Categorias
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }
    }

    public class Premios
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }
        [Display(Name = "Términos y Condiciones ")]
        public string TerminosCondiciones { get; set; }
        public int Stock { get; set; }
        [Display(Name = "Canjes Totales")]
        public int StockActual { get; set; }
        public long Puntos { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public int DiasDeVencimiento
        {
            get
            {
                TimeSpan diferencia = FechaVencimiento.Date - Fecha.Date;
                int dias = diferencia.Days;

                return dias;
            }
        }
        public bool Activo { get; set; }
        [Display(Name = "Categoría")]
        public virtual Categorias Categoria { get; set; }
    }

    public class RelacionPuntos
    {
        public int Id { get; set; }
        public decimal Monto { get; set; }
        public long Puntos { get; set; }
        public DateTime Fecha { get; set; }
        public bool Activo { get; set; }
    }


    public class PuntosClientes
    {
        public int Id { get; set; }
        public virtual Clientes Cliente { get; set; }
        [Display(Name = "Nro Tarjeta")]
        public string NroTarjeta { get; set; }
        public long Puntos { get; set; }
    }

    public class HistorialCanje
    {
        public int Id { get; set; }
        public virtual Premios Premio { get; set; }
        public virtual Clientes Cliente { get; set; }
        public string CodigoCupon { get; set; }
        public DateTime FechaVencimientoCupon { get; set; }
        public string NroTarjeta { get; set; }
        [Display(Name = "Puntos Consumidos")]
        public long PuntosConsumidos { get; set; }
        [Display(Name = "Puntos Restantes")]
        public long PuntosRestantes { get; set; }
        public DateTime Fecha { get; set; }
        public bool Activo { get; set; }
    }
    public class HistorialDePuntos
    {
        public virtual Clientes Cliente { get; set; }
        public int Id { get; set; }
        [Display(Name = "Puntos Obtenidos")]
        public long PuntosObtenidos { get; set; }
        [Display(Name = "Puntos Totales")]
        public long PuntosTotales { get; set; }
        public DateTime Fecha { get; set; }
    }

    public class FotosPremios
    {
        public int Id { get; set; }
        public virtual Premios Premio { get; set; }
        public string Foto { get; set; }
        public int Orden { get; set; }
        public DateTime Fecha { get; set; }
    }

    public class PuntosObtenidosClientes
    {
        public int Id { get; set; }
        public virtual Usuario Usuario { get; set; }
        public long IdSolicitud { get; set; }
        public string IdOperacion { get; set; }
        public decimal MontoCompra { get; set; }
        public DateTime FechaCompra { get; set; }
        public string Compania { get; set; }
        public int CompaniaId { get; set; }
        public long PuntosObtenidos { get; set; }
        public long PuntosDisponibles { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public DateTime FechaProcesada { get; set; }
    }
}