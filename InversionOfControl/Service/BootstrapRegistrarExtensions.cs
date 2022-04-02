using InversionOfControl.Model;
using Microsoft.Extensions.DependencyInjection;

namespace InversionOfControl.Service;

public static class BootstrapRegistrarExtensions
{
    /// <summary>
    /// Add the different <see cref="IBootstrap"/> registration to the IoC container
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddBootstrap(this IServiceCollection services)
    {
        var bootstrapType = typeof(IBootstrap);
        foreach (var type in AppDomain.CurrentDomain.GetAssemblies()
                                      .SelectMany(s => s.GetTypes())
                                      .Where(p => p.IsClass)
                                      .Where(p => bootstrapType.IsAssignableFrom(p)))
        {
            var bootstrap = (IBootstrap)type.GetConstructor(Type.EmptyTypes)!.Invoke(Array.Empty<object>());
            bootstrap.ConfigureServices(services);
        }

        return services;
    }
}