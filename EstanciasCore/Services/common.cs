using System;  
using System.Data;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using DAL.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.IO.Compression;
using System.Net.Mail;
using Excel.FinancialFunctions;
using DAL.Models;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using DAL.Models.Core;
using Newtonsoft.Json.Linq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using static QRCoder.PayloadGenerator;
using Google.Protobuf.WellKnownTypes;
using MessagePack;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Database;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using OfficeOpenXml.Style;
using System.Runtime.InteropServices;
using System.Security.Policy;
using MySql.Data.MySqlClient.Memcached;
using Org.BouncyCastle.Asn1.Cmp;
using Org.BouncyCastle.Asn1.Crmf;
using RestSharp;
using Method = RestSharp.Method;
using MySqlX.XDevAPI;
using Newtonsoft.Json;
using System.Diagnostics;
using DAL.Models.ViewModels;
using Microsoft.AspNetCore.Authentication.Twitter;
using RestSharp;
using static EstanciasCore.Services.common;
using Org.BouncyCastle.Asn1.X509;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Numeric;
using OfficeOpenXml.Packaging.Ionic.Zlib;
using System.Globalization;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Org.BouncyCastle.Crmf;
using Org.BouncyCastle.Asn1.Ocsp;

namespace EstanciasCore.Services
{
    public class common
    {
        public static int DiasDelMes(int mes)
        {
            switch (mes)
            {
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12:
                    return 31;
                case 2:
                    return 28;
                default:
                    return 30;
            }
        }
        public class InMemoryFile
        {
            public string FileName { get; set; }
            public byte[] Content { get; set; }
        }
        public static int NiumeroRandom(int min, int max)
        {
            var rand = new Random();
            return rand.Next(min, max);
        }
        public static string NumeroALetras(double value)
        {
            string num2Text; value = Math.Truncate(value);
            if (value == 0) num2Text = "CERO";
            else if (value == 1) num2Text = "UNO";
            else if (value == 2) num2Text = "DOS";
            else if (value == 3) num2Text = "TRES";
            else if (value == 4) num2Text = "CUATRO";
            else if (value == 5) num2Text = "CINCO";
            else if (value == 6) num2Text = "SEIS";
            else if (value == 7) num2Text = "SIETE";
            else if (value == 8) num2Text = "OCHO";
            else if (value == 9) num2Text = "NUEVE";
            else if (value == 10) num2Text = "DIEZ";
            else if (value == 11) num2Text = "ONCE";
            else if (value == 12) num2Text = "DOCE";
            else if (value == 13) num2Text = "TRECE";
            else if (value == 14) num2Text = "CATORCE";
            else if (value == 15) num2Text = "QUINCE";
            else if (value < 20) num2Text = "DIECI" + NumeroALetras(value - 10);
            else if (value == 20) num2Text = "VEINTE";
            else if (value < 30) num2Text = "VEINTI" + NumeroALetras(value - 20);
            else if (value == 30) num2Text = "TREINTA";
            else if (value == 40) num2Text = "CUARENTA";
            else if (value == 50) num2Text = "CINCUENTA";
            else if (value == 60) num2Text = "SESENTA";
            else if (value == 70) num2Text = "SETENTA";
            else if (value == 80) num2Text = "OCHENTA";
            else if (value == 90) num2Text = "NOVENTA";
            else if (value < 100) num2Text = NumeroALetras(Math.Truncate(value / 10) * 10) + " Y " + NumeroALetras(value % 10);
            else if (value == 100) num2Text = "CIEN";
            else if (value < 200) num2Text = "CIENTO " + NumeroALetras(value - 100);
            else if ((value == 200) || (value == 300) || (value == 400) || (value == 600) || (value == 800)) num2Text = NumeroALetras(Math.Truncate(value / 100)) + "CIENTOS";
            else if (value == 500) num2Text = "QUINIENTOS";
            else if (value == 700) num2Text = "SETECIENTOS";
            else if (value == 900) num2Text = "NOVECIENTOS";
            else if (value < 1000) num2Text = NumeroALetras(Math.Truncate(value / 100) * 100) + " " + NumeroALetras(value % 100);
            else if (value == 1000) num2Text = "MIL";
            else if (value < 2000) num2Text = "MIL " + NumeroALetras(value % 1000);
            else if (value < 1000000)
            {
                num2Text = NumeroALetras(Math.Truncate(value / 1000)) + " MIL";
                if ((value % 1000) > 0)
                {
                    num2Text = num2Text + " " + NumeroALetras(value % 1000);
                }
            }
            else if (value == 1000000)
            {
                num2Text = "UN MILLON";
            }
            else if (value < 2000000)
            {
                num2Text = "UN MILLON " + NumeroALetras(value % 1000000);
            }
            else if (value < 1000000000000)
            {
                num2Text = NumeroALetras(Math.Truncate(value / 1000000)) + " MILLONES ";
                if ((value - Math.Truncate(value / 1000000) * 1000000) > 0)
                {
                    num2Text = num2Text + " " + NumeroALetras(value - Math.Truncate(value / 1000000) * 1000000);
                }
            }
            else if (value == 1000000000000) num2Text = "UN BILLON";
            else if (value < 2000000000000) num2Text = "UN BILLON " + NumeroALetras(value - Math.Truncate(value / 1000000000000) * 1000000000000);
            else
            {
                num2Text = NumeroALetras(Math.Truncate(value / 1000000000000)) + " BILLONES";
                if ((value - Math.Truncate(value / 1000000000000) * 1000000000000) > 0)
                {
                    num2Text = num2Text + " " + NumeroALetras(value - Math.Truncate(value / 1000000000000) * 1000000000000);
                }
            }
            return num2Text;
        }
        public static byte[] GetZipArchive(List<InMemoryFile> files)
        {
            byte[] archiveFile;
            using (var archiveStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(archiveStream, ZipArchiveMode.Create, true))
                {
                    foreach (var file in files)
                    {
                        var zipArchiveEntry = archive.CreateEntry(file.FileName, System.IO.Compression.CompressionLevel.Fastest);
                        using (var zipStream = zipArchiveEntry.Open())
                            zipStream.Write(file.Content, 0, file.Content.Length);
                    }
                }
                archiveFile = archiveStream.ToArray();
            }
            return archiveFile;
        }
        public static string padl0(string texto, int longitud)
        {
            string convertir = "0000000000000000" + texto.Trim();
            return convertir.Substring(convertir.Length - longitud);
        }
        public static string padl0d(decimal texto, int longitud, int decimales)
        {
            string prefijo = "";
            if (texto < 0)
            {
                prefijo = "-";
            }
            string parteentera = texto.ToString("F2").Split(".")[0];
            string partedecimal = "00";
            if (texto.ToString("F2").Split(".").Length > 1)
            {
                partedecimal = texto.ToString("F2").Split(".")[1];
            }
            string numero = "0000000000000000000" + prefijo + parteentera + partedecimal;
            return padr(numero, longitud + decimales);
        }
        public static string padr(string texto, int longitud)
        {
            string convertir = texto.Trim() + "                                                                  ";
            return convertir.Substring(0, longitud);
        }
        public class MailAPI
        {
            public string Mail { get; set; }
            public string Html { get; set; }
            public string Titulo { get; set; }
            public string Token { get; set; }
            public string Empresa { get; set; }
            public int Status { get; set; }
            public string Mensaje { get; set; }
        }
        public static Image resizeImage(Image imgToResize, Size size)
        {
            return (Image)(new Bitmap(imgToResize, size));
        }

        public static Image ByteaImage(byte[] byteArrayIn)
        {
            using (var ms = new MemoryStream(byteArrayIn))
            {
                return Image.FromStream(ms);
            }
        }

        public static byte[] imageaByte(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            return ms.ToArray();
        }
        public static bool EnviarMail(string destinatario, string titulo, string texto, string cliente, byte[] Adjunto = null, string NombreArchivo = null)
        {
            MailAPI mail = new MailAPI();
            mail.Mail = destinatario;
            mail.Html = texto;
            mail.Titulo = titulo;
            DateTime oFec = DateTime.Now;
            var code = Encrypt(mail.Titulo + mail.Html, "SendMail"); ;
            mail.Token = code;
            //var resultado=  EnviarMailGmail(mail);
            var resultado = EnviarMailSendinBlue(mail);
            return true;
        }

        public static decimal CalculaCFT(double capital, int cantidadcuotas, double montocuota)
        {
            try
            {
                List<double> valores = new List<double>();
                valores.Add(capital * -1);
                for (int x = 0; x < cantidadcuotas; x++)
                {
                    valores.Add(montocuota);
                }
                double tir = Financial.Irr(valores);
                decimal cft = Math.Round(Convert.ToDecimal(tir * 12 * 100), 2);
                return cft;
            }
            catch
            {
                return 0;
            }
        }
        public static bool EnviarMailSendinBlue(MailAPI mail)
        {
            if (mail.Mail == null)
            {
                return false;
            }
            try
            {
                //string usuario = "novedades@estancias.org.ar";
                string usuario = "albarracin_sergio@hotmail.com";

                // string password = "BWSNmr7qGLdHYKz2";
                string password = "w2cPVg3n9Xq6C7KO";

                var origen = new MailAddress("noresponder@estancias.com.ar", "Estancias ");
                string host = "smtp-relay.sendinblue.com";
                int puerto = 587;
                bool ssl = true;
                NetworkCredential credenciales = new NetworkCredential(usuario, password);
                MailMessage correo = new MailMessage("noresponder@estancias.org.ar", mail.Mail, mail.Titulo, cuerpoHTMLGmail(mail.Titulo, mail.Html, ""));
                correo.From = origen;
                correo.IsBodyHtml = true;
                SmtpClient servicio = new SmtpClient(host, puerto);
                servicio.UseDefaultCredentials = true;
                servicio.Credentials = credenciales;
                servicio.EnableSsl = ssl;
                string token = "";
                servicio.SendAsync(correo, token);
            }
            catch
            {
                return false;
            }
            return true;

        }
        public static bool EnviarMailGmail(MailAPI mail)
        {
            if (mail.Mail == null)
            {
                return false;
            }
            try
            {
                string usuario = "mutual.vivienda.militar@gmail.com";
                string password = "Nov2021**";
                var origen = new MailAddress("mutual.vivienda.militar@gmail.com", "Estancias - " + mail.Empresa);
                string host = "smtp.gmail.com";
                int puerto = 587;
                bool ssl = true;
                NetworkCredential credenciales = new NetworkCredential(usuario, password);
                MailMessage correo = new MailMessage("mutual.vivienda.militar@gmail.com", mail.Mail, mail.Titulo, cuerpoHTMLGmail(mail.Titulo, mail.Html, ""));
                correo.From = origen;
                correo.IsBodyHtml = true;
                SmtpClient servicio = new SmtpClient(host, puerto);
                servicio.UseDefaultCredentials = true;
                servicio.Credentials = credenciales;
                servicio.EnableSsl = ssl;
                servicio.Send(correo);
            }
            catch
            {
                return false;
            }
            return true;

        }
        //public static string cuerpoHTMLGmail(string titulo, string texto, string cliente)
        //{
        //    string sHTML = "";
        //    sHTML += "<html>";
        //    sHTML += "<meta charset='UTF-8'>";
        //    sHTML += "<body>";
        //    sHTML += "<img src=http://portalestancias.com.ar/images/logoEmailhorizontal.png><br/>";
        //    sHTML += "<h3>";
        //    sHTML += titulo;
        //    sHTML += "</h3><br><br>";
        //    sHTML += "<div id='Texto' style='font-size:12px; margin-bottom:20px; '>";
        //    sHTML += texto;
        //    sHTML += "</div>";
        //    sHTML += "<div id='footer' style='font-size:12px; text-align:left;'>";
        //    sHTML += "<p style='margin-top:0px; margin-bottom:0px;'><b>(2023) Estancias</b></p>";
        //    sHTML += "</div>";
        //    sHTML += "</body>";
        //    sHTML += "</html>";
        //    return sHTML;
        //}

        public static string cuerpoHTMLGmail(string titulo, string texto, string cliente)
        {

            string sHTML = "";
            //sHTML += "<!doctype html><html><head><meta charset =\"utf-8\"><style amp4email - boilerplate > body{ visibility: hidden}</style><script async src ='https://cdn.ampproject.org/v0.js' ></script><style amp - custom > u + .body img ~div div { display: none; } span.MsoHyperlnk, span.MsoHyperlinkFollowed { color: inherit; } a.es - button { text - decoration:none; }.es - desk - hidden { display: none; float:left; overflow: hidden; width: 0; max - height:0; line - height:0; }.es - button - border:hover > a.es - button {color:#FFFFFF;}body { width:100%; height:100%;}table { border-collapse:collapse; border-spacing:0px;}table td, body, .es-wrapper { padding:0; Margin:0;}.es-content, .es-header, .es-footer { width:100%; table-layout:fixed;}p, hr { Margin:0;}h1, h2, h3, h4, h5, h6 { Margin:0; font-family:arial, \"helvetica neue\", helvetica, sans-serif; letter-spacing:0;}.es-left { float:left;}.es-right { float:right;}.es-menu td { border:0;}s { text-decoration:line-through;}ul, ol { font-family:arial, \"helvetica neue\", helvetica, sans-serif; padding:0px 0px 0px 40px; margin:15px 0px;}ul li { color:#333333;}ol li { color:#333333;}li { margin:0px 0px 15px; font-size:14px;}a { text-decoration:underline;}.es-menu td a { font-family:arial, \"helvetica neue\", helvetica, sans-serif; text-decoration:none; display:block;}.es-wrapper { width:100%; height:100%;}.es-wrapper-color, .es-wrapper { background-color:#FAFAFA;}.es-content-body p, .es-footer-body p, .es-header-body p, .es-infoblock p { font-family:arial, \"helvetica neue\", helvetica, sans-serif; line-height:150%; letter-spacing:0;}.es-header { background-color:transparent;}.es-header-body { background-color:transparent;}.es-header-body p { color:#333333; font-size:14px;}.es-header-body a { color:#666666; font-size:14px;}.es-content-body { background-color:#FFFFFF;}.es-content-body a { color:#5C68E2; font-size:14px;}.es-footer { background-color:transparent;}.es-footer-body { background-color:#FFFFFF;}.es-footer-body p { color:#333333; font-size:12px;}.es-footer-body a { color:#333333; font-size:12px;}.es-content-body p { color:#333333; font-size:14px;}.es-infoblock p { font-size:12px; color:#CCCCCC;}.es-infoblock a { font-size:12px; color:#CCCCCC;}h1 { font-size:46px; font-style:normal; font-weight:bold; line-height:120%; color:#333333;}h2 { font-size:26px; font-style:normal; font-weight:bold; line-height:120%; color:#333333;}h3 { font-size:20px; font-style:normal; font-weight:bold; line-height:120%; color:#333333;}.es-header-body h1 a, .es-content-body h1 a, .es-footer-body h1 a { font-size:46px;}.es-header-body h2 a, .es-content-body h2 a, .es-footer-body h2 a { font-size:26px;}.es-header-body h3 a, .es-content-body h3 a, .es-footer-body h3 a { font-size:20px;}h4 { font-size:24px; font-style:normal; font-weight:normal; line-height:120%; color:#333333;}h5 { font-size:20px; font-style:normal; font-weight:normal; line-height:120%; color:#333333;}h6 { font-size:16px; font-style:normal; font-weight:normal; line-height:120%; color:#333333;}.es-header-body h4 a, .es-content-body h4 a, .es-footer-body h4 a { font-size:24px;}.es-header-body h5 a, .es-content-body h5 a, .es-footer-body h5 a { font-size:20px;}.es-header-body h6 a, .es-content-body h6 a, .es-footer-body h6 a { font-size:16px;}a.es-button, button.es-button { padding:10px 30px 10px 30px; display:inline-block; background:#5C68E2; border-radius:5px 5px 5px 5px; font-size:20px; font-family:arial, \"helvetica neue\", helvetica, sans-serif; font-weight:normal; font-style:normal; line-height:120%; color:#FFFFFF; text-decoration:none; width:auto; text-align:center; letter-spacing:0;}.es-button-border { border-style:solid; border-color:#2CB543 #2CB543 #2CB543 #2CB543; background:#5C68E2; border-width:0px 0px 0px 0px; display:inline-block; border-radius:5px 5px 5px 5px; width:auto;}.es-button img { display:inline-block; vertical-align:middle;}.es-fw, .es-fw .es-button { display:block;}.es-il, .es-il .es-button { display:inline-block;}.es-p20 { padding:20px;}.es-p10t { padding-top:10px;}.es-p20r { padding-right:20px;}.es-p10b { padding-bottom:10px;}.es-p20l { padding-left:20px;}.es-p20b { padding-bottom:20px;}.es-p20t { padding-top:20px;}.es-p5t { padding-top:5px;}.es-p5b { padding-bottom:5px;}.es-p15t { padding-top:15px;}.es-p15b { padding-bottom:15px;}.es-p40r { padding-right:40px;}.es-p35b { padding-bottom:35px;}ul li, ol li { margin-left:0;}.es-menu amp-img, .es-button amp-img { vertical-align:middle;}@media only screen and (max-width:600px) {h1 { font-size:36px; text-align:left } h2 { font-size:26px; text-align:left } h3 { font-size:20px; text-align:left } *[class=\"gmail-fix\"] { display:none } p, a { line-height:150% } h1, h1 a { line-height:120% } h2, h2 a { line-height:120% } h3, h3 a { line-height:120% } h4, h4 a { line-height:120% } h5, h5 a { line-height:120% } h6, h6 a { line-height:120% } h4 { font-size:24px; text-align:left } h5 { font-size:20px; text-align:left } h6 { font-size:16px; text-align:left } .es-header-body h1 a, .es-content-body h1 a, .es-footer-body h1 a { font-size:36px } .es-header-body h2 a, .es-content-body h2 a, .es-footer-body h2 a { font-size:26px } .es-header-body h3 a, .es-content-body h3 a, .es-footer-body h3 a { font-size:20px } .es-header-body h4 a, .es-content-body h4 a, .es-footer-body h4 a { font-size:24px } .es-header-body h5 a, .es-content-body h5 a, .es-footer-body h5 a { font-size:20px } .es-header-body h6 a, .es-content-body h6 a, .es-footer-body h6 a { font-size:16px } .es-menu td a { font-size:12px } .es-header-body p, .es-header-body a { font-size:14px } .es-content-body p, .es-content-body a { font-size:14px } .es-footer-body p, .es-footer-body a { font-size:14px } .es-infoblock p, .es-infoblock a { font-size:12px } .es-m-txt-c, .es-m-txt-c h1, .es-m-txt-c h2, .es-m-txt-c h3, .es-m-txt-c h4, .es-m-txt-c h5, .es-m-txt-c h6 { text-align:center } .es-m-txt-r, .es-m-txt-r h1, .es-m-txt-r h2, .es-m-txt-r h3, .es-m-txt-r h4, .es-m-txt-r h5, .es-m-txt-r h6 { text-align:right } .es-m-txt-j, .es-m-txt-j h1, .es-m-txt-j h2, .es-m-txt-j h3, .es-m-txt-j h4, .es-m-txt-j h5, .es-m-txt-j h6 { text-align:justify } .es-m-txt-l, .es-m-txt-l h1, .es-m-txt-l h2, .es-m-txt-l h3, .es-m-txt-l h4, .es-m-txt-l h5, .es-m-txt-l h6 { text-align:left } .es-m-txt-r amp-img { float:right } .es-m-txt-c amp-img { margin:0 auto } .es-m-txt-l amp-img { float:left } .es-m-txt-r .rollover:hover .rollover-second, .es-m-txt-c .rollover:hover .rollover-second, .es-m-txt-l .rollover:hover .rollover-second { display:inline } .es-m-txt-r .rollover div, .es-m-txt-c .rollover div, .es-m-txt-l .rollover div { line-height:0; font-size:0 } .es-spacer { display:inline-table } a.es-button, button.es-button { font-size:20px } a.es-button, button.es-button { display:inline-block } .es-button-border { display:inline-block } .es-m-fw, .es-m-fw.es-fw, .es-m-fw .es-button { display:block } .es-m-il, .es-m-il .es-button, .es-social, .es-social td, .es-menu { display:inline-block } .es-adaptive table, .es-left, .es-right { width:100% } .es-content table, .es-header table, .es-footer table, .es-content, .es-footer, .es-header { width:100%; max-width:600px } .adapt-img { width:100%; height:auto } .es-mobile-hidden, .es-hidden { display:none } .es-desk-hidden { width:auto; overflow:visible; float:none; max-height:inherit; line-height:inherit; display:table-row } tr.es-desk-hidden { display:table-row } table.es-desk-hidden { display:table } td.es-desk-menu-hidden { display:table-cell } .es-menu td { width:1% } table.es-table-not-adapt, .esd-block-html table { width:auto } .es-social td { padding-bottom:10px } .h-auto { height:auto } }</style></head><body class=\"body\"><div style=\"width: 41%; margin: 0px auto;\" ><img src=http://portalestancias.com.ar/images/logoEmailhorizontal.png></div><div class=\"es-wrapper-color\"> <!--[if gte mso 9]><v:background xmlns:v=\"urn:schemas-microsoft-com:vml\" fill=\"t\"> <v:fill type=\"tile\" color=\"#fafafa\"></v:fill> </v:background><![endif]--><table class=\"es-wrapper\" width=\"100%\" cellspacing=\"0\" cellpadding=\"0\"><tr><td valign=\"top\"><table cellpadding=\"0\" cellspacing=\"0\" class=\"es-content\" align=\"center\"><tr><td class=\"es-info-area\" align=\"center\"><table class=\"es-content-body\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" width=\"600\" style=\"background-color: transparent\" bgcolor=\"rgba(0, 0, 0, 0)\"><tr><td class=\"es-p20\" align=\"left\"><table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\"><tr><td width=\"560\" align=\"center\" valign=\"top\"><table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\"><tr><td align=\"center\" class=\"es-infoblock\"></td></tr></table></td></tr></table></td></tr></table></td></tr></table><table cellpadding =\"0\" cellspacing=\"0\" class=\"es-header\" align=\"center\"><tr><td align=\"center\"><table bgcolor=\"#ffffff\" class=\"es-header-body\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" width=\"600\"><tr><td class=\"es-p10t es-p10b es-p20r es-p20l\" align=\"left\"><table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\"><tr><td width=\"560\" class=\"es-m-p0r\" valign=\"top\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\"><tr><td align=\"center\" class=\"es-p20b\" style=\"font-size: 0px\"><amp-img src=\"https://fbbvzmf.stripocdn.email/content/guids/CABINET_dea32a35cfb59390d097649b58045d3a14ee17ccf28c94b2d9f0dec4fd6fe32e/images/logo.png\" alt=\"Logo\" style=\"display:block;font-size:12px\" width=\"200\" title=\"Logo\" height=\"18\"></amp-img></td></tr></table></td></tr></table></td></tr></table></td></tr></table><table cellpadding =\"0\" cellspacing=\"0\" class=\"es-content\" align=\"center\"><tr><td align=\"center\"><table bgcolor=\"#ffffff\" class=\"es-content-body\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" width=\"600\"><tr><td class=\"es-p20t es-p10b es-p20r es-p20l\" align=\"left\"><table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\"><tr><td width=\"560\" align=\"center\" valign=\"top\"><table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\"><tr><td align=\"center\" class=\"es-p10t es-p10b\" style=\"font-size: 0px\"><amp-img class=\"adapt-img\" src=\"https://fbbvzmf.stripocdn.email/content/guids/CABINET_dea32a35cfb59390d097649b58045d3a14ee17ccf28c94b2d9f0dec4fd6fe32e/images/clave.png\" alt style=\"display:block\" width=\"300\" height=\"300\" layout=\"responsive\"></amp-img></td></tr><tr><td align=\"center\" class=\"es-p20t es-p10b es-m-txt-c\"><h1 style=\"font-size: 46px;line-height: 46px\">" + titulo + "</h1></td></tr><tr><td align =\"center\" class=\"es-p5t es-p5b\"><p>Debajo podes encontrar el token para verificar tu usuario en nuestra APP.<br></p></td></tr></table></td></tr></table></td></tr><tr><td class=\"es-p10t es-p10b es-p20r es-p20l\" align=\"left\"> <!--[if mso]><table width=\"560\" cellpadding=\"0\" cellspacing=\"0\"><tr><td width=\"125\" valign=\"top\"><![endif]--><table cellpadding=\"0\" cellspacing=\"0\" align=\"left\" class=\"es-left\"><tr class=\"es-mobile-hidden\"><td width=\"105\" class=\"es-m-p20b\" align=\"center\" valign=\"top\"><table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"border-radius: 5px;border-collapse: separate\" role=\"presentation\"><tr><td align=\"center\" height=\"40\"></td></tr></table></td><td class=\"es-hidden\" width=\"20\"></td></tr></table> <!--[if mso]></td><td width =\"310\" valign=\"top\"><![endif]--><table cellpadding=\"0\" cellspacing=\"0\" class=\"es-left\" align=\"left\"><tr><td width=\"310\" class=\"es-m-p20b\" align=\"center\" valign=\"top\"><table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"border-radius: 5px;border-collapse: separate\" role=\"presentation\"><tr><td align=\"center\" class=\"es-p10b es-p20r es-p20l es-m-txt-c\"><h2>Token</h2></td></tr></table></td></tr><tr><td width=\"310\" class=\"es-m-p20b\" align=\"center\" valign=\"top\"><table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"border-left:2px dashed #cccccc;border-right:2px dashed #cccccc;border-top:2px dashed #cccccc;border-bottom:2px dashed #cccccc;border-radius: 5px;border-collapse: separate\" role=\"presentation\"><tr><td align=\"center\" class=\"es-p10t es-p10b es-p20r es-p20l es-m-txt-c\"><h1 style=\"font-size: 40px;color: #00000\"><strong>" + texto + "</strong></h1></td></tr></table></td></tr></table> <!--[if mso]></td><td width=\"20\"></td><td width =\"105\" valign=\"top\"><![endif]--><table cellpadding=\"0\" cellspacing=\"0\" class=\"es-right\" align=\"right\"><tr class=\"es-mobile-hidden\"><td width=\"105\" align=\"left\"><table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\"><tr><td align=\"center\" height=\"40\"></td></tr></table></td></tr></table> <!--[if mso]></td></tr></table><![endif]--></td></tr><tr><td class=\"es-p20t es-p20r es-p20l\" align=\"left\"><table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\"><tr><td width=\"560\" align=\"center\" valign=\"top\"><table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\"><tr><td align=\"center\" class=\"es-p15t es-p5b es-p20r es-p20l es-m-txt-c\"><h2>Beneficios de nuestra App</h2></td></tr><tr><td align=\"center\" class=\"es-p5t es-p5b es-p20r es-p20l\"><p></p></td></tr></table></td></tr></table></td></tr><tr><td class=\"esdev-adapt-off es-p10t es-p20r es-p20l\" align=\"left\"><table width=\"560\" cellpadding=\"0\" cellspacing=\"0\" class=\"esdev-mso-table\"><tr><td class=\"esdev-mso-td\" valign=\"top\"><table cellpadding=\"0\" cellspacing=\"0\" align=\"left\" class=\"es-left\"><tr><td width=\"45\" class=\"es-m-p0r\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\"><tr><td align=\"center\" style=\"font-size: 0px\"><amp-img src=\"https://fbbvzmf.stripocdn.email/content/guids/CABINET_887f48b6a2f22ad4fb67bc2a58c0956b/images/2851617878322771.png\" alt style=\"display: block\" width=\"35\" height=\"38\"></amp-img></td></tr></table></td></tr></table></td><td width=\"20\"></td><td class=\"esdev-mso-td\" valign=\"top\"><table cellpadding=\"0\" cellspacing=\"0\" class=\"es-right\" align=\"right\"><tr><td width=\"495\" class=\"es-m-p0r\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\"><tr><td align=\"left\" class=\"es-p10t es-p10b es-m-p5t es-m-txt-l\"><p>Consultar y saldar tu estado de la cuenta de manera fácil y rápido</p></td></tr></table></td></tr></table></td></tr></table></td></tr><tr><td class=\"esdev-adapt-off es-p10t es-p20r es-p20l\" align=\"left\"><table width=\"560\" cellpadding=\"0\" cellspacing=\"0\" class=\"esdev-mso-table\"><tr><td class=\"esdev-mso-td\" valign=\"top\"><table cellpadding=\"0\" cellspacing=\"0\" align=\"left\" class=\"es-left\"><tr><td width=\"45\" class=\"es-m-p0r\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\"><tr><td align=\"center\" style=\"font-size: 0px\"><amp-img src=\"https://fbbvzmf.stripocdn.email/content/guids/CABINET_887f48b6a2f22ad4fb67bc2a58c0956b/images/2851617878322771.png\" alt style=\"display: block\" width=\"35\" height=\"38\"></amp-img></td></tr></table></td></tr></table></td><td width=\"20\"></td><td class=\"esdev-mso-td\" valign=\"top\"><table cellpadding=\"0\" cellspacing=\"0\" class=\"es-right\" align=\"right\"><tr><td width=\"495\" class=\"es-m-p0r\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\"><tr><td align=\"left\" class=\"es-p10t es-p10b es-m-p5t es-m-txt-l\"><p>Promociones Exclusivas en nuestros productos y locales</p></td></tr></table></td></tr></table></td></tr></table></td></tr><tr><td class=\"es-p10t es-p20b es-p20r es-p20l esdev-adapt-off\" align=\"left\"><table width=\"560\" cellpadding=\"0\" cellspacing=\"0\" class=\"esdev-mso-table\"><tr><td class=\"esdev-mso-td\" valign=\"top\"><table cellpadding=\"0\" cellspacing=\"0\" align=\"left\" class=\"es-left\"><tr><td width=\"45\" class=\"es-m-p0r\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\"><tr><td align=\"center\" style=\"font-size: 0px\"><amp-img src=\"https://fbbvzmf.stripocdn.email/content/guids/CABINET_887f48b6a2f22ad4fb67bc2a58c0956b/images/2851617878322771.png\" alt style=\"display: block\" width=\"35\" height=\"38\"></amp-img></td></tr></table></td></tr></table></td><td width=\"20\"></td><td class=\"esdev-mso-td\" valign=\"top\"><table cellpadding=\"0\" cellspacing=\"0\" class=\"es-right\" align=\"right\"><tr><td width=\"495\" class=\"es-m-p0r\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\"><tr><td align=\"left\" class=\"es-p10t es-p10b es-m-p5t es-m-txt-l\"><p>Enterate de nuestros ultimos lanzamientos en el momento</p></td></tr></table></td></tr></table></td></tr></table></td></tr></table></td></tr></table><table cellpadding =\"0\" cellspacing=\"0\" class=\"es-footer\" align=\"center\"><tr><td align=\"center\"><table class=\"es-footer-body\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" width=\"640\" style=\"background-color: transparent\"><tr><td class=\"es-p20t es-p20b es-p20r es-p20l\" align=\"left\"><table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\"><tr><td width=\"600\" align=\"left\"><table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\"><tr><td align=\"center\" class=\"es-p15t es-p15b\" style=\"font-size:0\"><table cellpadding=\"0\" cellspacing=\"0\" class=\"es-table-not-adapt es-social\" role=\"presentation\"><tr><td align=\"center\" valign=\"top\" class=\"es-p40r\"><amp-img title=\"Facebook\" src=\"https://fbbvzmf.stripocdn.email/content/assets/img/social-icons/logo-black/facebook-logo-black.png\" alt=\"Fb\" width=\"32\" height=\"32\"></amp-img></td><td align =\"center\" valign=\"top\" class=\"es-p40r\"><a target=\"_blank\" href=\"https://www.instagram.com/estanciaschiripa/\"><amp-img title=\"Instagram\" src=\"https://fbbvzmf.stripocdn.email/content/assets/img/social-icons/logo-black/instagram-logo-black.png\" alt=\"Inst\" width=\"32\" height=\"32\"></amp-img></a></td><td align=\"center\" valign=\"top\"><a target=\"_blank\" href=\"https://www.youtube.com/channel/UChSg5cXgmbFlr1XsJVscffQ\"><amp-img title=\"Youtube\" src=\"https://fbbvzmf.stripocdn.email/content/assets/img/social-icons/logo-black/youtube-logo-black.png\" alt=\"Yt\" width=\"32\" height=\"32\"></amp-img></a></td></tr></table></td></tr><tr><td align=\"center\" class=\"es-p35b\"><p><span>© 2023 ESTANCIAS Todos los derechos reservados.</span></p></td></tr></table></td></tr></table></td></tr></table></td></tr></table><table cellpadding =\"0\" cellspacing=\"0\" class=\"es-content\" align=\"center\"><tr><td class=\"es-info-area\" align=\"center\"><table class=\"es-content-body\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" width=\"600\" style=\"background-color: transparent\" bgcolor=\"rgba(0, 0, 0, 0)\"><tr><td class=\"es-p20\" align=\"left\"><table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\"><tr><td width=\"560\" align=\"center\" valign=\"top\"><table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\"><tr><td align=\"center\" class=\"es-infoblock\"></td></tr></table></td></tr></table></td></tr></table></td></tr></table></td></tr></table></div></body></html>";
            sHTML += texto;
            return sHTML;

        }
        public static bool ejecutaodbc(string strSQL, System.Data.OleDb.OleDbConnection dbf)
        {
            System.Data.OleDb.OleDbCommand comando = new System.Data.OleDb.OleDbCommand(strSQL);
            comando.Connection = dbf;
            comando.CommandTimeout = 0;
            comando.ExecuteNonQuery();
            return true;
        }
        public static DataTable leeodbc(string strSQL, System.Data.OleDb.OleDbConnection dbf)
        {
            DataSet ds = new DataSet();
            System.Data.OleDb.OleDbDataAdapter dataAdapter = new System.Data.OleDb.OleDbDataAdapter(strSQL,dbf);
            dataAdapter.Fill(ds);
            return ds.Tables[0];
        }

        public static System.Data.OleDb.OleDbConnection abreodbc(string carpeta)
        {
            string coneccion = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + carpeta + ";Extended Properties=dBASE IV;";
            //            string coneccion = "Provider=VFPOLEDB.1;Data Source=" + carpeta + ";Collating Sequence=general;";
            System.Data.OleDb.OleDbConnection dbf = new System.Data.OleDb.OleDbConnection(coneccion);
            dbf.Open();
            return dbf;
        }
        public static bool cierraodbc (System.Data.OleDb.OleDbConnection dbf)
        {
            dbf.Close();
            dbf.Dispose();
            return true;
        }
        public static string Encrypt(string toEncrypt, string secretKey)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);
            var md5Serv = System.Security.Cryptography.MD5.Create();
            keyArray = md5Serv.ComputeHash(UTF8Encoding.UTF8.GetBytes(secretKey));
            md5Serv.Dispose();
            var tdes = System.Security.Cryptography.TripleDES.Create();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray =
              cTransform.TransformFinalBlock(toEncryptArray, 0,
              toEncryptArray.Length);
            tdes.Dispose();
            return sin_simbolos(Convert.ToBase64String(resultArray, 0, resultArray.Length));
        }
        public static string sin_simbolos(String cadena)
        {
            try
            {
                return Regex.Replace(cadena, @"[^a-zA-Z0-9 ñÑ]", "",
                                     RegexOptions.None, TimeSpan.FromSeconds(1.5));
            }
            catch (RegexMatchTimeoutException)
            {
                return String.Empty;
            }
        }
        public static string cuerpoHTML(string titulo, string texto, string cliente)
        {
            string sHTML = "";
            sHTML += "<html>";
            sHTML += "<meta charset='UTF-8'>";
            sHTML += "<body>";
            sHTML += "<img src=https://www.cge.mil.ar/cgewebsite/images/bannercge.png><br/>";
            sHTML += "<div id='TituloNombre' style='font-size: 14px; color: #14456b; font-weight: bold; border-style: solid; border-top: 0px; border-left: 0px; border-right: 0px; margin-bottom: 10px;'>";
            sHTML += titulo;
            sHTML += "</div>";
            sHTML += "<div id='cuerpoMail' style='font-size:12px; margin-bottom:20px; color:#14456b;'>";
            sHTML += texto;
            sHTML += "</div>";
            sHTML += "<div id='footerMail' style='font-size:10px; text-align:left;'>";
            sHTML += "<p style='margin-top:0px; margin-bottom:0px;'><b>Contaduria General del Ejército.</b></p>";
            sHTML += "<p style='margin-top:0px; margin-bottom:0px;'><b>Somos el Ejército.</b></p>";
            sHTML += "<p style='margin-top:0px; margin-bottom:0px;'><a href='https://www.cge.mil.ar'></a></p>";
            sHTML += "</div>";
            sHTML += "</body>";
            sHTML += "</html>";
            return sHTML;
        }

        public static byte[] ConvertFromBase64String(string input)
        {
            if (String.IsNullOrWhiteSpace(input)) return null;
            try
            {
                string working = input.Replace('-', '+').Replace('_', '/'); ;
                while (working.Length % 4 != 0)
                {
                    working += '=';
                }
                return Convert.FromBase64String(working);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static byte[] Reporting(string reporte, string parametros, string formato, EstanciasContext Context)
        {
            var cabecera = Context.DatosEstructura.FirstOrDefault();
            System.Net.NetworkCredential credencial = new System.Net.NetworkCredential(cabecera.UsuarioReportes,cabecera.CredencialReportes);
            WebClient client = new WebClient();
            client.Credentials = credencial;
            string reportURL = cabecera.URLReportes  + reporte + "&rs:Command=Render" + parametros + "&rs:Format=" + formato;
            return client.DownloadData(reportURL);
        }

        public static IRestResponse EnviaWhatsAppTexto(string telefono, string texto, string LineaId, string token)
        {
            string instance = LineaId;
            var url = $"https://api.ultramsg.com/{instance}/messages/chat";
            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("token", token);
            request.AddParameter("to", telefono);
            request.AddParameter("body", texto);

            IRestResponse response = client.Execute(request);
            return response;
        }

        public static void EnviaWhatsAppImagen(string telefono, string texto, string imagenurl, string LineaId, string token)
        {
            string instance = LineaId;
            var client = new RestClient("https://api.ultramsg.com/" + instance + "/messages/image");
            var request = new RestRequest("", Method.POST);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("undefined", "&token=" + token + "&to=" + telefono + "&image=" + imagenurl + "&caption=" + texto + "&priority=1&referenceId=", ParameterType.RequestBody);
            RestResponse response = (RestResponse)client.Execute(request);
            return;
        }

        public static HttpStatusCode EnviaNotificationWonderPushAll(string title, string message)
        {
            HttpClient _httpClient = new HttpClient();
            var accessToken = "ZDdmZTQ3YzQxZDI5YmNhYTUyMGEwOGVhZjI4YmQwMWMxZjg4ZDc1Mjk5NjVmOGVkMzE5YjIwYzcwNzBhZTE1NQ";
            var url = "https://management-api.wonderpush.com/v1/deliveries?accessToken=" + accessToken;
            var targetSegmentIds = "@ALL";

            var notification = new NotificationWonderPush
            {
                Alert = new AlertWonderPush
                {
                    Title = title,
                    Text = message
                }
            };

            var notificationJson = JsonConvert.SerializeObject(notification);
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("targetSegmentIds", targetSegmentIds),
                new KeyValuePair<string, string>("notification", notificationJson)
            });
            var response = _httpClient.PostAsync(url, content);
            var status = response.Result.StatusCode;
            return status;

        }   
        
        public static HttpStatusCode EnviaNotificationWonderPushTestUser(string title, string message)
        {
            HttpClient _httpClient = new HttpClient();
            var accessToken = "ZDdmZTQ3YzQxZDI5YmNhYTUyMGEwOGVhZjI4YmQwMWMxZjg4ZDc1Mjk5NjVmOGVkMzE5YjIwYzcwNzBhZTE1NQ";
            var url = "https://management-api.wonderpush.com/v1/deliveries?accessToken=" + accessToken;
            var targetSegmentIds = "01hvf7n5tnuj29id:@TEST";

            var notification = new NotificationWonderPush
            {
                Alert = new AlertWonderPush
                {
                    Title = title,
                    Text = message
                }
            };

            var notificationJson = JsonConvert.SerializeObject(notification);
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("targetSegmentIds", targetSegmentIds),
                new KeyValuePair<string, string>("notification", notificationJson)
            });
            var response = _httpClient.PostAsync(url, content);
            var status = response.Result.StatusCode;
            return status;

        }

        private static void SetearCultureInfoES()
        {
            CultureInfo cultura = new CultureInfo("es-ES");
            CultureInfo.CurrentCulture = cultura;
            CultureInfo.CurrentUICulture = cultura;
        }

        public static DateTime ConvertirFecha(string fecha)
        {
            SetearCultureInfoES();
            DateTime fechaIngresada;
            if (DateTime.TryParse(fecha, out fechaIngresada))
            {
                return fechaIngresada;
            }
            else
            {
                return fechaIngresada;
            }
        }

        /// <summary>
        /// Calcula una fecha formateada según una regla específica.
        /// - Si el día de la fecha de entrada es 15 o menor, devuelve esa misma fecha.
        /// - Si el día es mayor a 15, devuelve la fecha correspondiente al mes siguiente.
        /// El formato de salida es siempre "dd/MM/yyyy".
        /// </summary>
        /// <param name="fecha">La fecha de entrada para el cálculo.</param>
        /// <returns>Un string con la fecha formateada.</returns>
        public static string ObtenerFechaCalculada(DateTime fecha)
        {
            var fechaMesActualCuotas = DateTime.Now;
            DateTime fechaResultadoPunitorios;

            if (fechaMesActualCuotas.Day>15)
            {
                fechaResultadoPunitorios = new DateTime(fechaMesActualCuotas.Year, fechaMesActualCuotas.Month, 31);
            }
            else
            {
                fechaResultadoPunitorios = new DateTime(fechaMesActualCuotas.Year, fechaMesActualCuotas.Month, 15);
            }
            return fechaResultadoPunitorios.ToString("dd/MM/yyyy");
        }

        //public static HttpStatusCode EnviaNotificationWonderPushId(string title, string message, string[] deviceId)
        //{
        //	HttpClient _httpClient = new HttpClient();
        //          string iconPath = "https://cdn.by.wonderpush.com/upload/01hvf7n5tnuj29id/bfd983f284b4b409a187529e60a1f38da1750d97/v1/small";
        //          var accessToken = "ZDdmZTQ3YzQxZDI5YmNhYTUyMGEwOGVhZjI4YmQwMWMxZjg4ZDc1Mjk5NjVmOGVkMzE5YjIwYzcwNzBhZTE1NQ";
        //	var url = "https://management-api.wonderpush.com/v1/deliveries?accessToken=" + accessToken;

        //          var notification = new
        //          {
        //              targetDeviceIds = deviceId,
        //              notification = new
        //              {
        //                  alert = new
        //                  {
        //                      title = title,
        //                      text = message,
        //                      icon = iconPath, // URL del icono
        //                      ios = new
        //                      {
        //                          attachments = new[]
        //                          {
        //                              new
        //                              {
        //                                  url = iconPath
        //                              }
        //                          }
        //                      },
        //                      android = new
        //                      {
        //                          smallIcon = iconPath
        //                      },
        //                      web = new
        //                      {
        //                          icon = iconPath
        //                      }
        //                  }
        //              }
        //          };
        //          var notificationJson = JsonConvert.SerializeObject(notification);

        //          var content = new StringContent(notificationJson, Encoding.UTF8, "application/json");

        //          var response = _httpClient.PostAsync(url, content).Result;
        //          return response.StatusCode;
        //}

        public static HttpStatusCode EnviaNotificationWonderPushId(string title, string message, string[] deviceId, byte[] imgPath = null)
        {
            try
            {
                using (HttpClient _httpClient = new HttpClient())
                {
                    string iconPath = "https://cdn.by.wonderpush.com/upload/01hvf7n5tnuj29id/bfd983f284b4b409a187529e60a1f38da1750d97/v1/small";
                    if (imgPath!=null)
                    {
                        string base64Image = Convert.ToBase64String(imgPath);
                        iconPath = $"data:image/png;base64,{base64Image}";
                        
                    }
                    var accessToken = "ZDdmZTQ3YzQxZDI5YmNhYTUyMGEwOGVhZjI4YmQwMWMxZjg4ZDc1Mjk5NjVmOGVkMzE5YjIwYzcwNzBhZTE1NQ";
                    var url = $"https://management-api.wonderpush.com/v1/deliveries?accessToken={accessToken}";

                    var notification = new
                    {
                        targetDeviceIds = deviceId,
                        notification = new
                        {
                            alert = new
                            {
                                title = title,
                                text = message,
                                icon = iconPath, // URL del icono
                                ios = new
                                {
                                    attachments = new[]
                                    {
                                new
                                {
                                    url = iconPath
                                }
                            }
                                },
                                android = new
                                {
                                    smallIcon = iconPath,
                                    largeIcon = iconPath
                                },
                                web = new
                                {
                                    icon = iconPath,
                                }
                            }
                        }
                    };

                    var notificationJson = JsonConvert.SerializeObject(notification);
                    var content = new StringContent(notificationJson, Encoding.UTF8, "application/json");

                    var response = _httpClient.PostAsync(url, content).Result;
                    return response.StatusCode;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending notification: {ex.Message}");
                return HttpStatusCode.InternalServerError;
            }
        }

        ////Obtener Nuevo Push Token
        //public static async Task<HttpStatusCode> HandleWebhook()
        //{
        //    var accessToken = "ZDdmZTQ3YzQxZDI5YmNhYTUyMGEwOGVhZjI4YmQwMWMxZjg4ZDc1Mjk5NjVmOGVkMzE5YjIwYzcwNzBhZTE1NQ";
        //    string webhookContent;
        //    using (var reader = new StreamReader(Request.Body))
        //    {
        //        webhookContent = await reader.ReadToEndAsync();
        //        Request.Body.Position = 0; // Rebobina el stream al inicio
        //    }
        //    var webhookData = JsonConvert.DeserializeObject<dynamic>(webhookContent);
        //    HttpClient _httpClient = new HttpClient();

        //    if (webhookData.eventType == "Push token Invalidated")
        //    {
        //        string invalidToken = webhookData.invalidToken;

        //        // Elimina el token usando la API de WonderPush
        //        var request = new HttpRequestMessage(HttpMethod.Delete, $"https://management-api.wonderpush.com/v1/installations/{invalidToken}");
        //        request.Headers.Add("Authorization", $"Bearer {accessToken}");

        //        var response = await _httpClient.SendAsync(request);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            // El token se eliminó correctamente de WonderPush
        //        }
        //        else
        //        {
        //            // Hubo un error al eliminar el token, maneja el error apropiadamente
        //        }

        //        // (Opcional) Puedes mantener la consulta a tu base de datos para tus propios registros, pero ya no es estrictamente necesaria para WonderPush
        //    }

        //    return HttpStatusCode.OK;
        //}

        public class AlertWonderPush
        {
            public string Title { get; set; }
            public string Text { get; set; }
        }

        public class NotificationWonderPush
        {
            public AlertWonderPush Alert { get; set; }
            public string ImageUrl { get; set; }
        }

              
        public async Task<HttpStatusCode> ResubscribeUser([FromBody] ResubscribeRequest request)
        {
            var result = await UpdateWonderPushToken(request.UserId, request.NewToken);
            if (result)
            {
                return HttpStatusCode.Accepted;
            }
            else
            {
                return HttpStatusCode.BadRequest;
            }
        }

        private async Task<bool> UpdateWonderPushToken(string userId, string newToken)
        {
            using (HttpClient client = new HttpClient())
            {
                var url = $"https://management-api.wonderpush.com/v1/installations/{userId}";
                var accessToken = "ZDdmZTQ3YzQxZDI5YmNhYTUyMGEwOGVhZjI4YmQwMWMxZjg4ZDc1Mjk5NjVmOGVkMzE5YjIwYzcwNzBhZTE1NQ";
                var payload = new
                {
                    pushToken = newToken
                };

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var jsonPayload = JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await client.PutAsync(url, content);
                return response.IsSuccessStatusCode;
            }
        }
        

        public class ResubscribeRequest
        {
            public string UserId { get; set; }
            public string NewToken { get; set; }
        }



        public static void EnviaWhatsApp(string telefono, string LineaId, string token, string texto = null, string imagenurl = null, string fileurl = null, string Link = null)
        {
            string instance = LineaId;
            if (Link!=null)
            {
                texto = texto + " - " + Link;
            }
            if (imagenurl!=null)
            {
                var client = new RestClient("https://api.ultramsg.com/" + instance + "/messages/image");
                var request = new RestRequest("", Method.POST);
                request.AddHeader("content-type", "application/x-www-form-urlencoded");
                request.AddParameter("undefined", "&token=" + token + "&to=" + telefono +"&image=" +imagenurl+"&caption="+texto+ "&priority=1&referenceId=", ParameterType.RequestBody);
                RestResponse response = (RestResponse)client.Execute(request);
            }
            else
            {
                //var client = new RestClient("https://api.ultramsg.com/" + instance + "/messages/chat");
                //var request = new RestRequest("", Method.POST);
                //request.AddHeader("content-type", "application/x-www-form-urlencoded");
                //request.AddParameter("undefined", "&token=" + token + "&to=" + telefono + "&body=" + texto + "&priority=1&referenceId=", ParameterType.RequestBody);
                //RestResponse response = (RestResponse)client.Execute(request);

                var url = $"https://api.ultramsg.com/{instance}/messages/chat";
                var client = new RestClient(url);

                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/x-www-form-urlencoded");
                request.AddParameter("token", token);
                request.AddParameter("to", telefono);
                request.AddParameter("body", "WhatsApp API on UltraMsg.com works good");

                IRestResponse response = client.Execute(request);




            }
            if (fileurl!=null)
            {
                var client = new RestClient("https://api.ultramsg.com/" + instance + "/messages/document");
                var request = new RestRequest("", Method.POST);
                request.AddHeader("content-type", "application/x-www-form-urlencoded");
                request.AddParameter("undefined", "&token=" + token + "&to=" + telefono + "&document=" + fileurl + "&filename=documento.pdf&priority=1&referenceId=", ParameterType.RequestBody);
                RestResponse response = (RestResponse)client.Execute(request);
            }
            return;
        }
    }

    public class BaseDataAccess
    {
        protected string ConnectionString { get; set; }

        private SqlConnection GetConnection()
        {
            SqlConnection connection = new SqlConnection(this.ConnectionString);
            if (connection.State != ConnectionState.Open)
                connection.Open();
            return connection;
        }

        protected DbCommand GetCommand(DbConnection connection, string commandText, CommandType commandType)
        {
            SqlCommand command = new SqlCommand(commandText, connection as SqlConnection);
            command.CommandType = commandType;
            return command;
        }

        public Array GetParameter(string parameter1 = null, object value1 = null, string parameter2 = null, object value2 = null, string parameter3 = null, object value3 = null, string parameter4 = null, object value4 = null, string parameter5 = null, object value5 = null, string parameter6 = null, object value6 = null)
        {
            if (parameter1 == null)
                return null;
            else if (parameter2 == null)
            {
                SqlParameter[] parametros = new SqlParameter[1];
                SqlParameter parametro1 = new SqlParameter(parameter1, value1 != null ? value1 : DBNull.Value);
                parametro1.Direction = ParameterDirection.Input;
                parametros[0] = parametro1;
                return parametros;
            }
            else if (parameter3 == null)
            {
                SqlParameter[] parametros = new SqlParameter[2];
                SqlParameter parametro1 = new SqlParameter(parameter1, value1 != null ? value1 : DBNull.Value);
                parametro1.Direction = ParameterDirection.Input;
                parametros[0] = parametro1;
                SqlParameter parametro2 = new SqlParameter(parameter2, value2 != null ? value2 : DBNull.Value);
                parametro2.Direction = ParameterDirection.Input;
                parametros[1] = parametro2;
                return parametros;
            }
            else if (parameter4 == null)
            {
                SqlParameter[] parametros = new SqlParameter[3];
                SqlParameter parametro1 = new SqlParameter(parameter1, value1 != null ? value1 : DBNull.Value);
                parametro1.Direction = ParameterDirection.Input;
                parametros[0] = parametro1;
                SqlParameter parametro2 = new SqlParameter(parameter2, value2 != null ? value2 : DBNull.Value);
                parametro2.Direction = ParameterDirection.Input;
                parametros[1] = parametro2;
                SqlParameter parametro3 = new SqlParameter(parameter3, value3 != null ? value3 : DBNull.Value);
                parametro3.Direction = ParameterDirection.Input;
                parametros[2] = parametro3;
                return parametros;
            }
            else if (parameter5 == null)
            {
                SqlParameter[] parametros = new SqlParameter[4];
                SqlParameter parametro1 = new SqlParameter(parameter1, value1 != null ? value1 : DBNull.Value);
                parametro1.Direction = ParameterDirection.Input;
                parametros[0] = parametro1;
                SqlParameter parametro2 = new SqlParameter(parameter2, value2 != null ? value2 : DBNull.Value);
                parametro2.Direction = ParameterDirection.Input;
                parametros[1] = parametro2;
                SqlParameter parametro3 = new SqlParameter(parameter3, value3 != null ? value3 : DBNull.Value);
                parametro3.Direction = ParameterDirection.Input;
                parametros[2] = parametro3;
                SqlParameter parametro4 = new SqlParameter(parameter4, value4 != null ? value4 : DBNull.Value);
                parametro4.Direction = ParameterDirection.Input;
                parametros[3] = parametro4;
                return parametros;
            }
            else if (parameter6 == null)
            {
                SqlParameter[] parametros = new SqlParameter[5];
                SqlParameter parametro1 = new SqlParameter(parameter1, value1 != null ? value1 : DBNull.Value);
                parametro1.Direction = ParameterDirection.Input;
                parametros[0] = parametro1;
                SqlParameter parametro2 = new SqlParameter(parameter2, value2 != null ? value2 : DBNull.Value);
                parametro2.Direction = ParameterDirection.Input;
                parametros[1] = parametro2;
                SqlParameter parametro3 = new SqlParameter(parameter3, value3 != null ? value3 : DBNull.Value);
                parametro3.Direction = ParameterDirection.Input;
                parametros[2] = parametro3;
                SqlParameter parametro4 = new SqlParameter(parameter4, value4 != null ? value4 : DBNull.Value);
                parametro4.Direction = ParameterDirection.Input;
                parametros[3] = parametro4;
                SqlParameter parametro5 = new SqlParameter(parameter5, value5 != null ? value5 : DBNull.Value);
                parametro5.Direction = ParameterDirection.Input;
                parametros[4] = parametro5;
                return parametros;
            }
            else
            {
                SqlParameter[] parametros = new SqlParameter[6];
                SqlParameter parametro1 = new SqlParameter(parameter1, value1 != null ? value1 : DBNull.Value);
                parametro1.Direction = ParameterDirection.Input;
                parametros[0] = parametro1;
                SqlParameter parametro2 = new SqlParameter(parameter2, value2 != null ? value2 : DBNull.Value);
                parametro2.Direction = ParameterDirection.Input;
                parametros[1] = parametro2;
                SqlParameter parametro3 = new SqlParameter(parameter3, value3 != null ? value3 : DBNull.Value);
                parametro3.Direction = ParameterDirection.Input;
                parametros[2] = parametro3;
                SqlParameter parametro4 = new SqlParameter(parameter4, value4 != null ? value4 : DBNull.Value);
                parametro4.Direction = ParameterDirection.Input;
                parametros[3] = parametro4;
                SqlParameter parametro5 = new SqlParameter(parameter5, value5 != null ? value5 : DBNull.Value);
                parametro5.Direction = ParameterDirection.Input;
                parametros[4] = parametro5;
                SqlParameter parametro6 = new SqlParameter(parameter6, value5 != null ? value6 : DBNull.Value);
                parametro6.Direction = ParameterDirection.Input;
                parametros[5] = parametro6;
                return parametros;
            }
        }
        protected SqlParameter GetParameterOut(string parameter, SqlDbType type, object value = null, ParameterDirection parameterDirection = ParameterDirection.InputOutput)
        {
            SqlParameter parameterObject = new SqlParameter(parameter, type); ;

            if (type == SqlDbType.NVarChar || type == SqlDbType.VarChar || type == SqlDbType.NText || type == SqlDbType.Text)
            {
                parameterObject.Size = -1;
            }

            parameterObject.Direction = parameterDirection;

            if (value != null)
            {
                parameterObject.Value = value;
            }
            else
            {
                parameterObject.Value = DBNull.Value;
            }

            return parameterObject;
        }


        protected object ExecuteScalar(string procedureName, List<SqlParameter> parameters)
        {
            object returnValue = null;

            try
            {
                using (DbConnection connection = this.GetConnection())
                {
                    DbCommand cmd = this.GetCommand(connection, procedureName, CommandType.StoredProcedure);

                    if (parameters != null && parameters.Count > 0)
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());
                    }

                    returnValue = cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                //LogException("Failed to ExecuteScalar for " + procedureName, ex, parameters);
                throw;
            }

            return returnValue;
        }

        public DataTable GetDataTable(string procedureName, Array parameters)
        {
            DataSet ds = new DataSet();
            SqlConnection connection = new SqlConnection("Server=rey;Database=haberes;user=sa;password=Cofranjalud1406;MultipleActiveResultSets=true;Connection Timeout=30");
            {
                connection.Open();
                SqlDataAdapter sqa = new SqlDataAdapter(procedureName, connection);
                if (parameters != null)
                {
                    sqa.SelectCommand.Parameters.AddRange(parameters);
                }
                sqa.Fill(ds);
                try
                {
                    connection.Close();
                }
                catch
                {
                    connection.Close();
                }
            }
            return ds.Tables[0];
        }
        public int ExecuteNonQuery(string procedureName, Array parameters)
        {
            int returnValue = -1;
            SqlConnection connection = new SqlConnection("Server=rey;Database=haberes;user=sa;password=Cofranjalud1406;MultipleActiveResultSets=true;Connection Timeout=0");
            {
                connection.Open();
                SqlCommand command = new SqlCommand(procedureName, connection);
                command.CommandTimeout = 0;
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                returnValue = command.ExecuteNonQuery();
                try
                {
                    connection.Close();
                }
                catch
                {
                    connection.Close();
                }
            }
            return returnValue;
        }

        
    }
}