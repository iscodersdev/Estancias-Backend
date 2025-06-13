using DAL.Models.Core;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.DTOs
{
    public class PremiosImagenDTO
    {
        public int Id { get; set; }
        public List<string> Imagenes { get; set; }
    }
}
