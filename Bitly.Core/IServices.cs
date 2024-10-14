using Microsoft.Extensions.DependencyInjection;

namespace Bitly.Core;

public interface IServices
{
    public void RegisterService(IServiceCollection services);
}
