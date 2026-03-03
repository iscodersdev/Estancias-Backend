using Commons.Identity;
using Commons.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace DAL.Models
{
    public class ResendRequest
    {
        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("to")]
        public string To { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("html")]
        public string Html { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("cc")]
        public string Cc { get; set; }

        [JsonProperty("bcc")]
        public string Bcc { get; set; }

        [JsonProperty("reply_to")]
        public string ReplyTo { get; set; }

        [JsonProperty("attachments")]
        public List<Attachments> Attachments { get; set; }
    }

    public class Attachments
    {
        [JsonProperty("filename")]
        public string Filename { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }
    }

    public class MailConfig
    {
        public int Id { get; set; }
        public string Proveedor { get; set; } // "BREVO" o "RESEND"
        public string CodigoProveedor { get; set; } // "BREVO" o "RESEND"
        public string ApiKey { get; set; }    // Para Resend
        public string SmtpUser { get; set; }  // Para Brevo
        public string SmtpPass { get; set; }  // Para Brevo
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
        public bool Activo { get; set; }
    }
}