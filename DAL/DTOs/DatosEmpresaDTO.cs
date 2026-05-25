using System.Collections.Generic;

namespace DAL.DTOs
{
    public class DatosEmpresaDTO
    {
        public int Id { get; set; }
        public string CBU { get; set; }
        public string Alias { get; set; }
        public string Whatsapp { get; set; }
    }

    public class DatosEmpresaListadoDTO
    {
        public int TotalRegistros { get; set; }
        public int PaginaActual { get; set; }
        public int RegistrosPorPagina { get; set; }
        public List<DatosEmpresaDTO> Datos { get; set; }
    }

    public class DatosEmpresaResponseDTO
    {
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public DatosEmpresaDTO DatoEmpresa { get; set; }
    }
}