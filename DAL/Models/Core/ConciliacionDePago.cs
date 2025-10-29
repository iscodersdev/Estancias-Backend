using DAL.DTOs.API;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.Models.Core
{
    public class ConciliacionDePago
    {
        public int Id { get; set; }
        public virtual Usuario Usuario { get; set; }
        public string MercadoPagoId { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Monto { get; set; }
        public string Descripcion { get; set; }
        public MercadoPagoEstado Estado { get; set; }
        public string GetEstadoString()
        {
            return Estado.ToString();
        }

        public void SetEstado(string estado)
        {
            switch (estado)
            {
                case "pending":
                    this.Estado= MercadoPagoEstado.Pending;
                    break;
                case "in_process":
                    this.Estado = MercadoPagoEstado.in_process;
                    break;
                case "approved":
                    this.Estado = MercadoPagoEstado.approved;
                    break;
                case "authorized":
                    this.Estado = MercadoPagoEstado.authorized;
                    break;
                case "rejected":
                    this.Estado = MercadoPagoEstado.rejected;
                    break;
                case "cancelled":
                    this.Estado = MercadoPagoEstado.cancelled;
                    break;
                case "refunded":
                    this.Estado = MercadoPagoEstado.refunded;
                    break;
                case "charged_back":
                    this.Estado = MercadoPagoEstado.charged_back;
                    break;
            }
        }

    }
    public enum MercadoPagoEstado
    {
        [Display(Name = "Pendiente")]
        Pending,
        [Display(Name = "En proceso")]
        in_process,
        [Display(Name = "Aprobado")]
        approved,
        [Display(Name = "Autorizado")]
        authorized,
        [Display(Name = "Rechazado")]
        rejected,
        [Display(Name = "Cancelado")]
        cancelled,
        [Display(Name = "Devuelto")]
        refunded,
        [Display(Name = "Contracargo")]
        charged_back,
    }
}

