using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DAL.DTOs
{
    public class DestinoFondosDTO
    {
        public int DestinoFondosId { get; set; }
        [Display(Name = "Destino de Fondos")]
        public string Nombre { get; set; }
        public IEnumerable<SelectListItem> Rubros { get; set; }
        [Display(Name = "Rubros relacionados al Destino de Fondos")]
        public List<int> RubrosSeleccionados { get; set; }
    }
}
