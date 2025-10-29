using DAL.Models;
using DAL.Models.Core;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.DTOs.API
{
    public class MConciliacionDePagoDTO : RespuestaAPI
    {
        public string MercadoPagoId { get; set; }
    }


}