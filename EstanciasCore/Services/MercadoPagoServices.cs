using DAL.Data;
using DAL.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using Org.BouncyCastle.Utilities.Encoders;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;  
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
namespace EstanciasCore.Services
{
    public class MercadoPagoServices
    {
        public IConfiguration _Configuration { get; }
        public string Url = "https://api.mercadopago.com/v1/payments/";

        public MercadoPagoServices(IConfiguration configuration)
        {
            _Configuration = configuration;
        }

        public async Task<Payment> GetPago(string Id)
        {
            string toke = _Configuration.GetConnectionString("TokenMercadoPago");
            Payment paymentObject = null;
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", toke);
                string fullUrl = Url + Id;
                HttpResponseMessage response = await client.GetAsync(fullUrl);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();

                    try
                    {
                        paymentObject = JsonSerializer.Deserialize<Payment>(jsonResponse);
                    }
                    catch (JsonException jsonEx)
                    {
                        Console.WriteLine($"Error de parseo JSON: {jsonEx.Message}");
                    }
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                }
            }
            return paymentObject;
        }


        public class Payment
        {
            public string Id { get; set; }

            [JsonPropertyName("date_created")]
            public DateTime DateCreated { get; set; }

            [JsonPropertyName("date_approved")]
            public DateTime? DateApproved { get; set; }

            [JsonPropertyName("date_last_updated")]
            public DateTime DateLastUpdated { get; set; }

            [JsonPropertyName("money_release_date")]
            public DateTime MoneyReleaseDate { get; set; }

            [JsonPropertyName("payment_method_id")]
            public string PaymentMethodId { get; set; }

            [JsonPropertyName("payment_type_id")]
            public string PaymentTypeId { get; set; }

            [JsonPropertyName("status")]
            public string Status { get; set; }

            [JsonPropertyName("status_detail")]
            public string StatusDetail { get; set; }

            [JsonPropertyName("currency_id")]
            public string CurrencyId { get; set; }

            [JsonPropertyName("description")]
            public string Description { get; set; }

            [JsonPropertyName("collector_id")]
            public long CollectorId { get; set; }

            [JsonPropertyName("payer")]
            public Payer Payer { get; set; }

            [JsonPropertyName("metadata")]
            public Dictionary<string, object> Metadata { get; set; }

            [JsonPropertyName("additional_info")]
            public Dictionary<string, object> AdditionalInfo { get; set; }

            [JsonPropertyName("external_reference")]
            public string ExternalReference { get; set; }

            [JsonPropertyName("transaction_amount")]
            public decimal TransactionAmount { get; set; }

            [JsonPropertyName("transaction_amount_refunded")]
            public decimal TransactionAmountRefunded { get; set; }

            [JsonPropertyName("coupon_amount")]
            public decimal CouponAmount { get; set; }

            [JsonPropertyName("transaction_details")]
            public TransactionDetails TransactionDetails { get; set; }

            [JsonPropertyName("installments")]
            public int Installments { get; set; }

            [JsonPropertyName("card")]
            public Dictionary<string, object> Card { get; set; }
        }

        // Objeto anidado "payer"
        public class Payer
        {
            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonPropertyName("email")]
            public string Email { get; set; }

            [JsonPropertyName("identification")]
            public Identification Identification { get; set; }

            [JsonPropertyName("type")]
            public string Type { get; set; }
        }

        // Objeto anidado "identification"
        public class Identification
        {
            [JsonPropertyName("type")]
            public string Type { get; set; }

            [JsonPropertyName("number")]
            public string Number { get; set; } // Se usa string para evitar problemas con números largos
        }

        // Objeto anidado "transaction_details"
        public class TransactionDetails
        {
            [JsonPropertyName("net_received_amount")]
            public decimal NetReceivedAmount { get; set; }

            [JsonPropertyName("total_paid_amount")]
            public decimal TotalPaidAmount { get; set; }

            [JsonPropertyName("overpaid_amount")]
            public decimal OverpaidAmount { get; set; }

            [JsonPropertyName("installment_amount")]
            public decimal InstallmentAmount { get; set; }
        }


        //Respuesta

        public class PaymentNotification
        {
            [JsonPropertyName("action")]
            public string Action { get; set; }

            [JsonPropertyName("api_version")]
            public string ApiVersion { get; set; }

            [JsonPropertyName("data")]
            public PaymentData Data { get; set; }

            [JsonPropertyName("date_created")]
            public DateTimeOffset DateCreated { get; set; }

            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonPropertyName("live_mode")]
            public bool LiveMode { get; set; }

            [JsonPropertyName("type")]
            public string Type { get; set; }

            [JsonPropertyName("user_id")]
            public long UserId { get; set; }
        }

        public class PaymentData
        {
            [JsonPropertyName("id")]
            public string Id { get; set; }
            [JsonPropertyName("uat")]
            public string UAT { get; set; }
        }

    }
}