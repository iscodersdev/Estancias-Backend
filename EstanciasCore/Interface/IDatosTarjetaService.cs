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
    public interface IDatosTarjetaService
    {
        /// <summary>
        /// Obtiene los datos de una persona a partir de su número de documento.
        /// </summary>
        /// <param name="documento"></param>
        /// <returns></returns>
        Task<ApiResponseObtenerPersonaDTO> ObtenerPersonaAsync(string documento);

        /// <summary>
        /// Obtiene los créditos asociados a una persona.
        /// </summary>
        /// <param name="PersonaId"></param>
        /// <returns></returns>
        Task<ApiResponseObtenerCreditos> ObtenerCreditosAsync(string PersonaId);

        /// <summary>
        /// Obtiene los detalles de un crédito específico a partir de su ID de solicitud.
        /// </summary>
        /// <param name="SolicitudId"></param>
        /// <returns></returns>
        Task<ApiResponseCreditoDetalles> ObtenerCreditoDetallesAsync(int SolicitudId);

        /// <summary>
        /// obtiene la deuda de una operación específica a partir del número de documento.
        /// </summary>
        /// <param name="documento"></param>
        /// <returns></returns>
        Task<DeudaApiResponseLOAN> ObtenerDeudaOperacionAsync(string documento);

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
        decimal CalcularPunitorios(List<SolicitudDetail> cuotas);

        //Task<ObtenerCreditosResponse> ObtenerCreditosAsync(string login, string clave, int PersonaId);
        //Task<LoginUsuarioResponse> LoginApiLoanAsync(string usuario, string clave);
    }
}
