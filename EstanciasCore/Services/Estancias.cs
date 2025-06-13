using System;  
using DAL.Data;
using System.Net.Http;
using System.Linq;
using DAL.Models;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using MailKit.Security;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.IO;
namespace EstanciasCore.Services
{
    public class Estancias
    {
        public static string UrlEstancias = "http://138.99.6.117/api/socios/";
        public static Clientes CompruebaUsuarioEstancias(int DNI, EstanciasContext _context)
        {
            Clientes persona = new Clientes();
            string uat = LoginEstancias();
            using (var client = new HttpClient())
            {
                ClientesDTO cliente = new ClientesDTO();
                cliente.UAT = uat;
                cliente.DNI = DNI;
                client.BaseAddress = new Uri(UrlEstancias);
                HttpResponseMessage response = client.PostAsJsonAsync("TraeSocio", cliente).Result;
                if (response.IsSuccessStatusCode)
                {
                    var readTask = response.Content.ReadAsAsync<ClientesDTO>();
                    readTask.Wait();
                    cliente = readTask.Result;
                    if (cliente.Status == 500)
                    {
                        return persona;
                    }
                    if (cliente.Activo == false)
                    {
                        return persona;
                    }
                    var empresa = _context.Empresas.Find(2);
                    persona = _context.Clientes.FirstOrDefault(x => x.Persona.NroDocumento == DNI.ToString());
                }
            }
            return persona;
        }
        public static Clientes CompruebaUsuarioEstancias20(string eMail, EstanciasContext _context)
        {
            Clientes persona = new Clientes();
            string uat = LoginEstancias();
            using (var client = new HttpClient())
            {
                ClientesDTO cliente = new ClientesDTO();
                cliente.UAT = uat;
                cliente.eMail = eMail;
                client.BaseAddress = new Uri(UrlEstancias);
                try
                {
                    HttpResponseMessage response = client.PostAsJsonAsync("TraeSocio20", cliente).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var readTask = response.Content.ReadAsAsync<ClientesDTO>();
                        readTask.Wait();
                        cliente = readTask.Result;
                        if (cliente.Status == 500)
                        {
                            return persona;
                        }
                        if (cliente.Activo == false)
                        {
                            return persona;
                        }
                        var empresa = _context.Empresas.Find(2);
                        persona = _context.Clientes.FirstOrDefault(x => x.Usuario.Mail == eMail);
                    }
                }
                catch
                {
                    return persona;
                }
            }
            return persona;
        }

        public static string LoginEstancias()
        {
            using (var client = new HttpClient())
            {
                LoginDTO login = new LoginDTO();
                login.UsuarioId = "f27c48c3-5e2a-4f44-994e-d5a9b357499a";
                login.Password = "AQAAAAEAACcQAAAAEGzU7b87CI1dA0xq9z5Uxs+H4WpmcX/VrDVVy5nJEwKOz2IIfzU5AjexKSgNU/Kv7g==";
                client.BaseAddress = new Uri(UrlEstancias);
                try
                {
                    HttpResponseMessage response = client.PostAsJsonAsync("Login", login).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var readTask = response.Content.ReadAsAsync<LoginDTO>();
                        readTask.Wait();
                        login = readTask.Result;
                        if (login.Status == 200)
                        {
                            return login.UAT;
                        }
                    }
                    return response.ToString();
                }
                catch
                {
                    return null;
                }
            }
        }
        public static string EnviarMailEmpresa(DAL.Models.Empresas empresa, string destinatario, string titulo, string texto, byte[] imagen, string remitente = null)
        {
            try
            {
                Byte [] Imagen = ResizeImagen(imagen);
                var email = new MimeMessage();
                if (remitente != null)
                {
                    email.From.Add(MailboxAddress.Parse(remitente));
                }
                else
                {
                    email.From.Add(MailboxAddress.Parse(empresa.Mail));
                }
                email.To.Add(MailboxAddress.Parse(destinatario));
                email.Subject = titulo;
                email.Body = new TextPart(TextFormat.Html) { Text = cuerpoHTMLEmpresa(titulo, texto, "", empresa, Imagen) };
                var smtp = new SmtpClient();
                smtp.Connect(empresa.UrlMail, empresa.PuertoMail, SecureSocketOptions.SslOnConnect);
                smtp.Authenticate(empresa.UsernameMail, empresa.PasswordMail);
                smtp.Send(email);
                smtp.Disconnect(true);

                return "ok";
            }
            catch
            {
                return "false";
            }

        }
        public static string cuerpoHTMLEmpresa(string titulo, string texto, string cliente, DAL.Models.Empresas empresa, byte[] imagen)
        {
            string sHTML = "";
            sHTML += "<html>";
            sHTML += "<meta charset='UTF-8'>";
            sHTML += "<body>";
            sHTML += "<img src=data:image/jpg;base64," + Convert.ToBase64String(imagen) + "><br/>";
            sHTML += "<h3>";
            sHTML += titulo;
            sHTML += "</h3><br><br>";
            sHTML += "<div id='Texto' style='font-size:12px; margin-bottom:20px; '>";
            sHTML += texto;
            sHTML += "</div>";
            sHTML += "<div id='footer' style='font-size:12px; text-align:left;'>";
            sHTML += "<p style='margin-top:0px; margin-bottom:0px;'><b>" + empresa.RazonSocial + "</b></p><br><br>";
            sHTML += "<p style='margin-top:0px; margin-bottom:0px;'><b>(2023) Buenos Aires Fintech</b></p>";
            sHTML += "</div>";
            sHTML += "</body>";
            sHTML += "</html>";
            return sHTML;
        }
        public static byte[] ResizeImagen(byte[] original, int alto = 0, int ancho = 0)
        {
            if (original == null) return null;

            Image imagen = Image.Load(original);
            imagen.Mutate(i => i.Resize(ancho == 0 ? ConstImagenes.AnchoReporte : ancho, alto == 0 ? ConstImagenes.AltoReporte : alto));
            var salida = new MemoryStream();
            imagen.SaveAsPng(salida);

            return salida?.ToArray();
        }
    }
    public static class ConstImagenes
    {
        public static int AltoReporte = 500;
        public static int AnchoReporte = 500;
    }
}