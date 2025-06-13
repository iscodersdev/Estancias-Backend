using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.DTOs
{
    public class EmpresaDTO
    {
        public int Id { get; set; }
        public string CUIT { get; set; }
        public string RazonSocial { get; set; }
        public string Grupo { get; set; }
    }
}
