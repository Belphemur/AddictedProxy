using AddictedProxy.OneTimeMigration.Services;
using InversionOfControl.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AddictedProxy.OneTimeMigration.Bootstrap;

public class BootstrapMigration : IBootstrap
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<RunnerJob>();
        services.AddScoped<IMigrationsRunner, MigrationsRunner>();
        services.AddHostedService<MigrationRunnerHostedService>();
    }
}