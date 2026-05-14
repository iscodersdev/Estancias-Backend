using DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.DTOs
{

    public class RequestApi
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }

    }

    public class ListCuponesDTO : RequestApi
    {
        public List<CuponesDTO> Cupones { get; set; }

    }

    public class CuponesDTO 
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string TerminosCondiciones { get; set; }
        public int Stock { get; set; }
        public int StockActual { get; set; }
        public long Puntos { get; set; }
        public DateTime Fecha { get; set; }
        public bool Activo { get; set; }
        public int CategoriaId { get; set; }
        public string CategoriaNombre { get; set; }
        public string FechaVencimiento { get; set; }
        public string DiasDeVencimiento { get; set; }
        public string Imagen { get; set; }
    }

    public class CanjearCuponDTO : RequestApi
    {
        public int CuponId { get; set; }
    }


    public class ListCuponesClienteDTO : RequestApi
    {
        public List<CuponesClienteDTO> Cupones { get; set; }

    }

    public class CuponesClienteDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Codigo { get; set; }
        public DateTime Fecha { get; set; }
        public bool Activo { get; set; }
        public string CategoriaNombre { get; set; }
        public int DiasRestantesVencimiento { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string Imagen { get; set; }
    }


    public class PuntosDTO : RequestApi
    {
        public long Puntos { get; set; }
    }
}
