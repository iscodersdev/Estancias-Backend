using DAL.Models;
using System.Threading.Tasks;
using static EstanciasCore.Services.common;

namespace EstanciasCore.Interface
{

    public interface IMailProvider
    {
        Task<bool> EnviarAsync(MailAPI mail, MailConfig config, byte[] adjunto = null);
    }

    public interface IMailService
    {
        Task<bool> EnviarAsync(MailAPI mail, byte[] adjunto = null);
    }
}
