using DAL.DTOs.Reportes;
using DAL.DTOs.Servicios;
using DAL.DTOs.Servicios.DatosTarjeta;
using DAL.Mobile;
using DAL.Models;
using EstanciasCore.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EstanciasCore.Interface
{
    public interface IDatosTarjetaService
    {       

        /// <summary>
        /// Consulta los movimiento sy montos de loan API Antigua.
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="clave"></param>
        /// <param name="documento"></param>
        /// <param name="numeroTarjeta"></param>
        /// <param name="cantidadMovimientos"></param>
        /// <param name="tipomovimientotarjeta"></param>
        /// <returns></returns>
        Task<CombinedData> ConsultarMovimientos(string usuario, string clave, String documento, long numeroTarjeta, long cantidadMovimientos, int tipomovimientotarjeta);

        /// <summary>
        /// Prepara los datos necesarios para el resumen de un periodo específico y un usuario.
        /// </summary>
        /// <param name="periodoId"></param>
        /// <param name="usuarioId"></param>
        /// <returns></returns>
        Task<TempalteResumenDTO> PrepararDatosDTO(CombinedData datosMovimientos, Periodo periodo, UsuarioParaProcesarDTO usuario);

        //Task ActualizarMovimientosAsync(Usuario usuario);
        //Task ActualizarMovimientosAsyncModificado(Usuario usuario);
        //Task<JsonResult> ActualizarMovimientosAsync();
        Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model);

        /// <summary>
        /// Calcula los punitorios de una lista de cuotas a partir de una fecha de cálculo.
        /// </summary>
        /// <param name="cuotas"></param>
        /// <param name="fechaDeCalculo"></param>
        /// <returns></returns>
        Task<decimal> CalcularPunitorios(List<SolicitudDetail> cuotas);

        /// <summary>
        /// Calcula el monto de la cuotas a pagar.
        /// </summary>
        /// <param name="datosMovimientos"></param>
        /// <param name="fechaActualCuotas"></param>
        /// <returns></returns>
        Task<decimal> CalcularMontoCuota(CombinedData datosMovimientos, DateTime fechaActualCuotas);

        /// <summary>
        /// Calcula el monto de la proxima cuotas a pagar.
        /// </summary>
        /// <param name="datosMovimientos"></param>
        /// <param name="fechaActualCuotas"></param>
        /// <returns></returns>
        Task<decimal> CalcularMontoProximaCuota(CombinedData datosMovimientos, DateTime fechaActualCuotasProximo);

        /// <summary>
        /// Obtiene los ultimos movimientos de la tarjeta.
        /// </summary>
        /// <param name="datosMovimientos"></param>
        /// <param name="fechaActualCuotas"></param>
        /// <returns></returns>
        Task<List<MovimientoTarjetaDTO>> ObtieneUltimosMovimientos(CombinedData datosMovimientos, int top);
    }
}
