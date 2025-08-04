using DAL.DTOs.Servicios;
using DAL.DTOs.Servicios.DatosTarjeta;
using DAL.Models;
using EstanciasCore.Services;
using Microsoft.AspNetCore.Mvc;
using System;
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

        Task<CombinedData> ConsultarMovimientos(string usuario, string clave, String documento, long numeroTarjeta, long cantidadMovimientos, int tipomovimientotarjeta);

        //Task<DatosParaResumenDTO> PrepararDatosDTO(int periodoId, string usuarioId);
        //Task ActualizarMovimientosAsync(Usuario usuario);
        //Task ActualizarMovimientosAsyncModificado(Usuario usuario);
        //Task<JsonResult> ActualizarMovimientosAsync();
        //Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model);
        //Task<ObtenerCreditosResponse> ObtenerCreditosAsync(string login, string clave, int PersonaId);
        //Task<LoginUsuarioResponse> LoginApiLoanAsync(string usuario, string clave);
    }
}
