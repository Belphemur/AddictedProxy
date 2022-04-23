#region

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace InversionOfControl.Model;

public interface IBootstrap
{
    /// <summary>
    ///     Configure the different services
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration);
}