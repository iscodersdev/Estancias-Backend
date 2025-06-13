using System;

namespace DAL.Models.Core
{
    public class Compras
    {
        public int Id { get; set; }
        public virtual Clientes Cliente { get; set; }
        public virtual Producto Producto { get; set; }
        public virtual MovimientoBilletera Movimiento { get; set; }
        public DateTime FechaCompra { get; set; }
        public EstadoCompra Estado { get; set; }
        public TipoCompra TipoCompra { get; set; }
        public DateTime FechaAnulacion { get; set; }
    }
    public enum EstadoCompra
    {
        Efectuado,
        Pendiente,
        Anulado
    }

    public enum TipoCompra
    {
        Financiado,
        DebitoFinanciado,
        Debito
    }
}
