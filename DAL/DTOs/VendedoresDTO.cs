using System;
using System.Collections.Generic;

namespace DAL.DTOs
{
    public class ListVendedoresDTO
    {
        public int Id { get; set; }
        public string NroDocumento { get; set; }
        public string Nombre { get; set; }
        public string Domicilio { get; set; }
        public string Telefono { get; set; }
        public string Mail { get; set; }
    }

    public class VendedorDTO
    {
        public int Id { get; set; }

        public int? TipoDocumentoId { get; set; }
        public string TipoDocumento { get; set; }

        public int? PaisId { get; set; }
        public string Pais { get; set; }

        public string NroDocumento { get; set; }
        public string Cuil { get; set; }
        public string Nombres { get; set; }
        public string Apellido { get; set; }
        public DateTime? FechaNacimiento { get; set; }

        public string Domicilio { get; set; }
        public string Telefono { get; set; }
        public string Mail { get; set; }
    }

    public class VendedoresListadoDTO
    {
        public int TotalRegistros { get; set; }
        public int PaginaActual { get; set; }
        public int RegistrosPorPagina { get; set; }
        public List<ListVendedoresDTO> Vendedores { get; set; }
    }

    public class VendedorResponseDTO
    {
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public VendedorDTO Vendedor { get; set; }
    }

    public class VendedorSelectDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    public class VendedorFormDataDTO
    {
        public List<VendedorSelectDTO> TiposDocumento { get; set; }
        public List<VendedorSelectDTO> Paises { get; set; }
    }
}