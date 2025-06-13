using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Mobile
{
    public class MTraeLocalidadDTO
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public int? LocalidadId { get; set; }
        public int? ProvinciaId { get; set; }
        public int? GuarnicionId { get; set; }
        public List<LocalidadDTO> Localidades { get; set; }
    }
    public class LocalidadDTO
    {
        public int Id { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public string NombreLocalidad { get; set; }
        public int IdDepartamento { get; set; }
        public int IdProvincia { get; set; }
        public string NombreProvincia { get; set; }
        public int IdGuarnicion { get; set; }
        public string NombreGuarnicion { get; set; }
    }
}
