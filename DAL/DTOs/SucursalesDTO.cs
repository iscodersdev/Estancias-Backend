using DAL.Models;
using System.Collections.Generic;

namespace DAL.DTOs
{
    public class TraeSucursalesDTO : RespuestaAPI
    {
        public virtual List<SucursalesDTO> Sucursales { get; set; }
    }

    // latitude y longitude lo converti a string porque en la base de datos esta en string y no en double

    public class SucursalesDTO
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string group { get; set; }

        // Lo dejo como List<string> para que el código viejo no tenga problemas
        // Ejemplo: ["Estancias", "Baciver"]
        public List<string> Marcas { get; set; }
    }

    // Esta clase aparte la usamos para el endpoint nuevo
    public class SucursalesEndpointDTO
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string group { get; set; }

        // Para mostrar directo en la grilla: "Estancias, Baciver"
        public string Marca { get; set; }

        // Para devolver lista de nombres
        public List<string> Marcas { get; set; }

        // Para editar y tener seleccionadas las marcas
        public List<int> MarcasId { get; set; }
    }

    public class SucursalCreateRequest
    {
        public string name { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string group { get; set; }

        // IDs de las marcas seleccionadas
        public List<int> MarcasId { get; set; }
    }

    public class SucursalUpdateRequest
    {
        public string name { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string group { get; set; }

        // IDs de las marcas seleccionadas
        public List<int> MarcasId { get; set; }
    }
}