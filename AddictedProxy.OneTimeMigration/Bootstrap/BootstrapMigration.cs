using AddictedProxy.OneTimeMigration.Services;
using InversionOfControl.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AddictedProxy.OneTimeMigration.Bootstrap;

public class BootstrapMigration : IBootstrap
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILoggingBuilder logging)
    {
        services.AddScoped<RunnerJob>();
        services.AddScoped<IMigrationsRunner, MigrationsRunner>();
        services.AddHostedService<MigrationRunnerHostedService>();
    }
}