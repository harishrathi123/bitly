using Bitly.Core;

namespace Bitly.Configurators;

public static class ModuleServicesConfigurator
{
    // TODO: use scrutor to scan & register IServices
    public static void RegisterAllServices(this IServiceCollection services)
    {
        var serviceType = typeof(IServices);
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.GetName().Name?.StartsWith("Bitly") == true);

        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes()
                .Where(t => t is { IsInterface: false, IsAbstract: false })
                .Where(serviceType.IsAssignableFrom);

            foreach (var type in types)
            {
                var instance = (IServices)Activator.CreateInstance(type)!;
                instance.RegisterService(services);
            }
        }
    }
}
