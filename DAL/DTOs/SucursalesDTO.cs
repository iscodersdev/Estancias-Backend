using DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.DTOs
{
    public class TraeSucursalesDTO : RespuestaAPI
    {
        public virtual List<SucursalesDTO> Sucursales { get; set; }
    }

    //latitude y longitude lo converti a string porque en la base de datos esta en string y no en double

    public class SucursalesDTO
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string group { get; set; }

    }

    public class SucursalCreateRequest
    {
        public string name { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string group { get; set; }
    }

    public class SucursalUpdateRequest
    {
        public string name { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string group { get; set; }
    }

}