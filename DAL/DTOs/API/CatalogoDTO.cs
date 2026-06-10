using DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.DTOs.API
{
    public class MCatalogosDTO : RespuestaAPI
    {
        public List<MCatalogoDTO> Catalogos { get; set; }
    }
    public class MCatalogoDTO
    {
        public string Link { get; set; }
        public string NombreDeMarca { get; set; }
    }
      
}
