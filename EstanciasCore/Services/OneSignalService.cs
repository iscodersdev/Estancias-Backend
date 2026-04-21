using DAL.Models.Core;
using EstanciasCore.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EstanciasCore.Services
{
    public class OneSignalService : IPushService // Asegúrate de renombrar la interfaz si es necesario
    {
        private readonly IConfiguration _configuration;
        private static readonly HttpClient _httpClient = new HttpClient();

        // Estos valores deberían ir en tu appsettings.json idealmente
        private const string APP_ID = "f1f5c4f1-87d1-4a48-a6a6-d31e27bb7e28";
        private const string REST_API_KEY = "os_v2_app_6h24j4mh2fferjvg2mpcpo36faepge4m7j6uvzfpql7zi2gjfyccyg6tuhkttqibhgpqufilygqywbeu2rl6iwxudymzeo5qj736iva";
        private const string API_URL = "https://onesignal.com/api/v1/notifications";

        public OneSignalService(IConfiguration configuration)
        {
            _configuration = configuration;

            // Configuración básica del cliente
            if (_httpClient.DefaultRequestHeaders.Authorization == null)
            {
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", REST_API_KEY);
            }
        }

        public async Task<bool> EnviarNotificacionGeneral(NotificacionViewModelDTO notificacion)
        {
            var payload = new
            {
                app_id = APP_ID,
                included_segments = new[] { "All" }, // Equivalente a @ALL de WonderPush
                headings = new { en = notificacion.Titulo, es = notificacion.Titulo },
                contents = new { en = notificacion.Mensaje, es = notificacion.Mensaje },
                url = notificacion.DeepLink,
                // Imagen principal (Banner)
                big_picture = notificacion.ImagenUrl,
                // Icono para Android (Small icon / Large icon)
                large_icon = notificacion.ImagenIcon,
                // Para iOS
                ios_attachments = !string.IsNullOrEmpty(notificacion.ImagenUrl)
                    ? new { id1 = notificacion.ImagenUrl }
                    : null
            };

            return await EjecutarEnvio(payload);
        }

        public async Task<bool> EnviarNotificacionAIds(NotificacionViewModelDTO notificacion, List<string> deviceIds)
        {
            if (deviceIds == null || !deviceIds.Any()) return false;

            // FILTRO: Solo dejamos los que parecen UUID válidos para OneSignal
            var validIds = deviceIds.Where(id => Guid.TryParse(id, out _)).ToList();

            if (!validIds.Any())
            {
                // Si no hay IDs válidos, no disparamos la API para evitar el error 400
                return false;
            }

            var payload = new
            {
                app_id = APP_ID,
                include_player_ids = validIds, // Usamos la lista filtrada
                headings = new { en = notificacion.Titulo, es = notificacion.Titulo },
                contents = new { en = notificacion.Mensaje, es = notificacion.Mensaje },
                url = notificacion.DeepLink,
                big_picture = notificacion.ImagenUrl,
                large_icon = notificacion.ImagenIcon,
                ios_attachments = !string.IsNullOrEmpty(notificacion.ImagenUrl)
                    ? new { id1 = notificacion.ImagenUrl }
                    : null,
                priority = 10
            };

            return await EjecutarEnvio(payload);
        }

        private async Task<bool> EjecutarEnvio(object payload)
        {
            try
            {
                var json = JsonConvert.SerializeObject(payload, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Aseguramos que el header de Auth esté presente (por si cambió el API KEY)
                var request = new HttpRequestMessage(HttpMethod.Post, API_URL);
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", REST_API_KEY);
                request.Content = content;

                var response = await _httpClient.SendAsync(request);
                var responseString = await response.Content.ReadAsStringAsync();

                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                // Aquí podrías usar un ILogger para registrar el error
                return false;
            }
        }

        public async Task<bool> EnviarNotificacionPorCumpleanios()
        {
            // Lógica pendiente de implementación según tu necesidad de negocio
            return await Task.FromResult(true);
        }
    }
}