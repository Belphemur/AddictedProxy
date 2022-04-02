using Microsoft.Extensions.DependencyInjection;

namespace InversionOfControl.Model;

public interface IBootstrap
{
    /// <summary>
    ///     Configure the different services
    /// </summary>
    /// <param name="services"></param>
    public void ConfigureServices(IServiceCollection services);
}