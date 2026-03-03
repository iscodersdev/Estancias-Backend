using DAL.Models;
using EstanciasCore.Interface;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using static EstanciasCore.Services.common;

public class BrevoSmtpProviderService : IMailProvider
{
    public async Task<bool> EnviarAsync(MailAPI mail, MailConfig config, byte[] adjunto = null)
    {
        try
        {
            using (var smtp = new SmtpClient(config.SmtpHost, config.SmtpPort))
            {
                smtp.Credentials = new NetworkCredential(config.SmtpUser, config.SmtpPass);
                smtp.EnableSsl = true;

                var message = new MailMessage
                {
                    From = new MailAddress(config.SenderEmail, config.SenderName),
                    Subject = mail.Titulo,
                    Body = mail.Html,
                    IsBodyHtml = true
                };
                message.To.Add(mail.Mail);

                await smtp.SendMailAsync(message);
                return true;
            }
        }
        catch { return false; }
    }
}