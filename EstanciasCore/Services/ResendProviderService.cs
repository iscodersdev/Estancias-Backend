using DAL.Models;
using EstanciasCore.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static EstanciasCore.Services.common;

public class ResendProviderService : IMailProvider
{
    private readonly HttpClient _httpClient;
    public ResendProviderService(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<bool> EnviarAsync(MailAPI mail, MailConfig config, byte[] adjunto = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, config.SmtpHost);
        request.Headers.Add("Authorization", $"Bearer {config.ApiKey}");

        var body = new ResendRequest
        {
            From = $"{config.SenderName} <{config.SenderEmail}>",
            To = mail.Mail,
            Subject = mail.Titulo,
            Html = mail.Html,   
        };

        if (adjunto!=null)
        {
            body.Attachments = new List<Attachments>
            {
                new Attachments
                {
                    Filename = "Resumen.pdf",
                    Content = Convert.ToBase64String(adjunto)
                }
            };
        }

        request.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var errorDetail = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine(errorDetail);
        }
        return response.IsSuccessStatusCode;
    }
}