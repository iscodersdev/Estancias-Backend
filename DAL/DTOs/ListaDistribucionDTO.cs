using System;

namespace DAL.DTOs
{
    public class ListaDistribucionDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool Activo { get; set; }
        public int CantidadDestinatarios { get; set; }
    }

    public class ListaDistribucionCreateUpdateDTO
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool Activo { get; set; }
    }

    public class CrearDestinatarioDTO
    {
        public string DestinatarioId { get; set; }
    }

    public class DistribucionDestinatarioDTO
    {
        public int Id { get; set; }

        public int ListaDistribucionId { get; set; }
        public string ListaDistribucionNombre { get; set; }

        public string DestinatarioId { get; set; }
        public string UserName { get; set; }

        public string Apellido { get; set; }
        public string Nombres { get; set; }
        public string NroDocumento { get; set; }

        public string NombreCompleto { get; set; }
    }

}