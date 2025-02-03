#region

using System.Reflection;
using InversionOfControl.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

#endregion

namespace InversionOfControl.Service.Bootstrap;

public static class BootstrapRegistrarExtensions
{
    private static BootstrapRegister? _register = new();

    /// <summary>
    ///     Add the different <see cref="IBootstrap" /> registration to the IoC container
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="assemblies">Where to look for <see cref="IBootstrap" /></param>
    /// <returns></returns>
    public static IHostApplicationBuilder AddBootstrap(this IHostApplicationBuilder builder, params Assembly[] assemblies)
    {
        _register!.RegisterBootstrapServices(builder.Services,builder.Configuration, builder.Logging, assemblies);
        return builder;
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