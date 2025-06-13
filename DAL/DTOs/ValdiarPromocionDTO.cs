using DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.DTOs
{
    public class ValdiarPromocionDTO
    {
        public int Status { set; get; }
        public string Mensaje { set; get; }
        public bool Canjeado { set; get; }
    }
}