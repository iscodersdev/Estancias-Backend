using DAL.DTOs.Reportes;
using DAL.DTOs.Servicios;
using DAL.DTOs.Servicios.DatosTarjeta;
using DAL.Models;
using EstanciasCore.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EstanciasCore.Interface
{
    public interface IResumenTarjetaService
    {
        /// <summary>
        /// Genera Resumento de Tarjeta en formato PDF y los guarda en la base de datos (ResumenTarjeta)
        /// </summary>
        /// <returns></returns>
        Task<bool> GenerarResumenTarjetas();
    }
}
