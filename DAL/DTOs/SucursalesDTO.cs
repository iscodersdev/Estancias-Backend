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

    public class SucursalesDTO 
    {
        public string name { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string group { get; set; }

    }
}
