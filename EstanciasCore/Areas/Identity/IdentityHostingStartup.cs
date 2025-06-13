using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(EstanciasCore.Areas.Identity.IdentityHostingStartup))]
namespace EstanciasCore.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}