using System;
using System.Collections.Generic;

namespace DAL.Mobile
{
    public class MTraeProvinciasDTO
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public int? ProvinciaId { get; set; }

        public List<ProvinciaDTO> Provincias { get; set; }
    }
    public class ProvinciaDTO
    {
        public int Id { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public string Descripcion { get; set; }
        public string DescripcionCompleta { get; set; }
    }
}