using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.DTOs
{
        public class MarcaListDTO
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
            public string Alias { get; set; }
            public string CBU { get; set; }
            public string WhatsApp { get; set; }
            public int Orden { get; set; }
            public bool Activo { get; set; }
            public bool TieneImagen { get; set; }
        }

        public class MarcaDTO
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
            public string Alias { get; set; }
            public string CBU { get; set; }
            public string WhatsApp { get; set; }
            public int Orden { get; set; }
            public bool Activo { get; set; }
            public bool TieneImagen { get; set; }
        }

        public class MarcaCreateDTO
        {
            public string Nombre { get; set; }
            public string Alias { get; set; }
            public string CBU { get; set; }
            public string WhatsApp { get; set; }
            public int Orden { get; set; }
        }

        public class MarcaUpdateDTO
        {
            public string Nombre { get; set; }
            public string Alias { get; set; }
            public string CBU { get; set; }
            public string WhatsApp { get; set; }
            public int Orden { get; set; }
            public bool Activo { get; set; }
        }
}
