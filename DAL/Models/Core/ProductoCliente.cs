using System;

namespace DAL.Models.Core
{
    public class ProductoCliente
    {
            public int Id { get; set; }
            public string Descripcion { get; set; }
            public byte[] Foto { get; set; }
            public virtual Clientes Cliente { get; set; }
            public virtual Rubro Rubro { get; set; }
            public bool Activo { get; set; }
            public string DescripcionAmpliada { get; set; }
            public decimal Precio { get; set; }
            public decimal? Oferta { get; set; }
    }
    public class ProductoClienteContacto
    {
        public int Id { get; set; }
        public virtual Clientes ClienteComprador { get; set; }
        public virtual ProductoCliente Producto { get; set; }
        public DateTime FechaContacto { get; set; }        
    }
}
