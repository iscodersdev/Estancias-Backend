using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using DAL.Models.API;
using EstanciasCore.Controllers;
using Microsoft.AspNetCore.Http;

namespace EstanciasCore.Services
{
    public class NotificacionAPIService
    {
        public NotificacionAPIService(){}

        public HttpStatusCode Envia_Push(string push_id, string subtitulo, string texto)
        {
            var httpClient = new HttpClient();
            Push push = new Push();
            //Actualizar app_id para Billetera
            push.app_id = "ZTBiYzY1NDAtN2RmNC00ZDM3LThkZTYtYjlmYjgxYTZkNTEx";
            List<string> id = new List<string> { push_id };
            push.include_player_ids = id;
            Idiomas idiomaheading = new Idiomas();
            idiomaheading.en = subtitulo;
            idiomaheading.es = subtitulo;
            push.headings = idiomaheading;
            Idiomas idiomacontent = new Idiomas();
            idiomacontent.en = texto;
            idiomacontent.es = texto;
            push.contents = idiomacontent;
            push.android_group = push_id;
            HttpResponseMessage result = httpClient.PostAsync("https://onesignal.com/api/v1/notifications", push.AsJson()).Result;
            return result.StatusCode;
        }

    }
}