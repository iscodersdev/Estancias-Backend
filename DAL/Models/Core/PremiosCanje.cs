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
        public int Stock { get; set; }
        [Display(Name = "Canjes Totales")]
        public int StockActual { get; set; }
        public int Puntos { get; set; }
        public DateTime Fecha { get; set; }
        public bool Activo { get; set; }
        [Display(Name = "Categoría")]
        public virtual Categorias Categoria { get; set; }
    }

    public class RelacionPuntos
    {
        public int Id { get; set; }
        public decimal Monto { get; set; }
        public int Puntos { get; set; }
        public DateTime Fecha { get; set; }
        public bool Activo { get; set; }
    }


    public class PuntosClientes
    {
        public int Id { get; set; }
        public virtual Clientes Cliente { get; set; }
        [Display(Name = "Nro Tarjeta")]
        public string NroTarjeta { get; set; }
        public int Puntos { get; set; }
    }

    public class HistorialCanje
    {
        public int Id { get; set; }
        public virtual Premios Premio { get; set; }
        public virtual Clientes Cliente { get; set; }
        public string NroTarjeta { get; set; }
        [Display(Name = "Puntos Consumidos")]
        public int PuntosConsumidos { get; set; }
        [Display(Name = "Puntos Restantes")]
        public int PuntosRestantes { get; set; }
        public DateTime Fecha { get; set; }
    }
    public class HistorialDePuntos
    {
        public int Id { get; set; }
        [Display(Name = "Puntos Obtenidos")]
        public int PuntosObtenidos { get; set; }
        [Display(Name = "Puntos Totales")]
        public int PuntosTotales { get; set; }
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
}