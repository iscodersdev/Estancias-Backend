using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class CuentaCorriente
    {
        public int Id { get; set; }
        [DisplayName("Fecha")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; }
        public DateTime? Vencimiento { get; set; }
        public string Observaciones { get; set; }
        public decimal Importe { get; set; }
        public decimal Saldo { get; set; }
        public virtual Clientes Cliente { get; set; }
        public virtual Conceptos Concepto { get; set; }
    }

    public class Conceptos
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Signo { get; set; }
    }

    public class CuentaCorrienteDTO
    {
        public int Id { get; set; }
        public string Socio { get; set; }
        public string NumeroDocumento { get; set; }
        public DateTime Fecha { get; set; }
        public string Concepto { get; set; }
        public decimal Creditos { get; set; }
        public decimal Debitos { get; set; }
        public decimal Saldo { get; set; }
        public string NumeroSocio { get; set; }
        public string Apellido{ get; set; }
        public string Nombres { get; set; }
        public string Vencimiento { get; set; }
        public string Observaciones { get; set; }
    }
    public class ImportaExcelDTO
    {
        public int Id { get; set; }
        public bool BorraRenglonesExistentes { get; set; }
        public string Texto { get; set; }
    }
}