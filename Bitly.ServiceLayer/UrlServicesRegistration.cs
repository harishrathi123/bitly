using Bitly.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Bitly.UrlServices;

public class UrlServicesRegistration : IServices
{
    public void RegisterService(IServiceCollection services)
    {
        services.AddSingleton<ICodeGeneration, CodeGeneration>();
    }
}
