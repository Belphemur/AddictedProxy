#region

using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

#endregion

namespace InversionOfControl.Model;

public interface IBootstrap
{
    /// <summary>
    /// To be used if this bootstrap depends on another assembly bootstrap(s)
    /// </summary>
    public Assembly[] Dependencies => [];

    /// <summary>
    ///     Configure the different services
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="logging"></param>
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILoggingBuilder logging);
}