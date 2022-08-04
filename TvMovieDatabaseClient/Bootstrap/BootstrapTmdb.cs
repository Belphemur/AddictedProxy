using InversionOfControl.Model;
using InversionOfControl.Service.EnvironmentVariable.Registration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TvMovieDatabaseClient.Bootstrap.EnvVar;

namespace TvMovieDatabaseClient.Bootstrap;

public class BootstrapTmdb : IBootstrap, IBootstrapEnvironmentVariable<TmdbConfig, TmdbConfigParser>
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        
    }

    public EnvVarRegistration<TmdbConfig, TmdbConfigParser> EnvVarRegistration { get; } = new("TMDB_APIKEY");
}