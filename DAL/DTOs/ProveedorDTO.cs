using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DAL.DTOs
{
    public class ListProveedorDTO
    {
        public int Id { get; set; }        
        public string NombreCompleto { get; set; }
        public string CUIT { get; set; }
        public string RazonSocial { get; set; }
        public string Empresa { get; set; }
    }
    public class ProveedorDTO
    {
        public int ProveedorId { get; set; }
        [Display(Name = "Nombre del Proveedor")]
        public string Nombre { get; set; }
        public Int64 CUIT { set; get; }
        [Display(Name = "Razon Social")]
        public string RazonSocial { get; set; }
        public string Domicilio { get; set; }
        public IEnumerable<SelectListItem> Rubros { get; set; }
        [Display(Name = "Rubros del Proveedor")]
        public List<int> RubrosSeleccionados { get; set; }
        public IEnumerable<SelectListItem> Empresas { get; set; }
        [Display(Name = "Empresa del Proveedor")]
        public int EmpresaId { get; set; }
    }
}
