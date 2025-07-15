using DAL.Models;
using EstanciasCore.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EstanciasCore.Interface
{
    public interface IDatosTarjetaService
    {
        Task<CombinedData> ConsultarMovimientos(string usuario, string clave, String documento, long numeroTarjeta, long cantidadMovimientos, int tipomovimientotarjeta);
        Task<DatosParaResumenDTO> PrepararDatosDTO(int periodoId, string usuarioId);
        Task ActualizarMovimientosAsync(Usuario usuario);
        Task<JsonResult> ActualizarMovimientosAsync();
        Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model);
    }
}
