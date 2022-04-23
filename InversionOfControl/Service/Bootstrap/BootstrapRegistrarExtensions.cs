#region

using System.Reflection;
using InversionOfControl.Extensions;
using InversionOfControl.Model;
using InversionOfControl.Service.EnvironmentVariable.Exception;
using InversionOfControl.Service.EnvironmentVariable.Parser;
using InversionOfControl.Service.EnvironmentVariable.Registration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

#endregion

namespace InversionOfControl.Service.Bootstrap;

public static class BootstrapRegistrarExtensions
{
    private static BootstrapRegister? _register = new();

    /// <summary>
    ///     Add the different <see cref="IBootstrap" /> registration to the IoC container
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="assemblies">Where to look for <see cref="IBootstrap" /></param>
    /// <returns></returns>
    public static IServiceCollection AddBootstrap(this IServiceCollection services, IConfiguration configuration, params Assembly[] assemblies)
    {
        _register!.RegisterBootstrapServices(services, configuration, assemblies);
        return services;
    }

    /// <summary>
    ///     Add the different <see cref="IBootstrapApp" /> to configure the app
    /// </summary>
    /// <param name="application">Application builder</param>
    /// <param name="assemblies">Where to look for <see cref="IBootstrap" /></param>
    /// <returns></returns>
    public static IApplicationBuilder UseBootstrap(this IApplicationBuilder application, params Assembly[] assemblies)
    {
        _register!.RegisterBootstrapApp(application, assemblies);
        _register.Dispose();
        _register = null;
        return application;
    }
}