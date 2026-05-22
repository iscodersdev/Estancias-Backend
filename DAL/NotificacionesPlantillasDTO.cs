using System;

namespace DAL.DTOs
{
    public class NotificacionesPlantillasDTO
    {
        public int Id { get; set; }

        public string Nombre { get; set; }

        public string Titulo { get; set; }

        public string Mensaje { get; set; }

        public string ImagenUrl { get; set; }

        public string ImagenIcon { get; set; }

        public string DeepLink { get; set; }

        public bool PreferLargeImage { get; set; }

        public bool Activo { get; set; }

        public bool NotificacionAutomatica { get; set; }
    }

    public class NotificacionesPlantillasRequestDTO
    {
        public string Nombre { get; set; }

        public string Titulo { get; set; }

        public string Mensaje { get; set; }

        public string ImagenUrl { get; set; }

        public string ImagenIcon { get; set; }

        public string DeepLink { get; set; }

        public bool PreferLargeImage { get; set; }
    }

    public class EnviarTestPlantillaRequestDTO
    {
        public int ListaDistribucionId { get; set; }
    }

    public class ListaDistribucionSimpleDTO
    {
        public int Id { get; set; }

        public string Nombre { get; set; }
    }
}