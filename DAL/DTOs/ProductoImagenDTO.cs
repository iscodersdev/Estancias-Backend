using DAL.Models.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.DTOs
{
    public class ProductoAdjuntosDTO
    {
        public int ProductoId { get; set; }
        public List<ProductoAdjuntos> ProductoAdjuntos { get; set; }
    }
}
