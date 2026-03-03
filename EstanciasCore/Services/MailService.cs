using DAL.Data;
using EstanciasCore.Interface;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using static EstanciasCore.Services.common;

public class MailService : IMailService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly EstanciasContext _context;

    public MailService(IServiceProvider serviceProvider, EstanciasContext context)
    {
        _serviceProvider = serviceProvider;
        _context = context;
    }

    public async Task<bool> EnviarAsync(MailAPI mail, byte[] adjunto = null)
    {
        var config = _context.MailConfig.FirstOrDefault(c => c.Activo);
        if (config == null) return false;

        IMailProvider provider;
        if (config.CodigoProveedor.ToUpper() == "RESEND")
            provider = _serviceProvider.GetService<ResendProviderService>();
        else
            provider = _serviceProvider.GetService<BrevoSmtpProviderService>();

        if (provider == null) return false;

        return await provider.EnviarAsync(mail, config, adjunto);
    }
}