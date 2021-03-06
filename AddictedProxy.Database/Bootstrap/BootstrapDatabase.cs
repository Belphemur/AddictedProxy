#region

using AddictedProxy.Database.Context;
using AddictedProxy.Database.EnvVar;
using AddictedProxy.Database.Repositories.Credentials;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Database.Transaction;
using InversionOfControl.Model;
using InversionOfControl.Service.EnvironmentVariable.Registration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace AddictedProxy.Database.Bootstrap;

public class BootstrapDatabase : IBootstrap,
                                 IBootstrapEnvironmentVariable<EFCoreLicense, EFCoreLicenseParser>
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHostedService<SetupEfCoreHostedService>();
        services.AddDbContext<EntityContext>(builder => builder.EnableSensitiveDataLogging());

        services.AddScoped<ITvShowRepository, TvShowRepository>();

        services.AddScoped<ISeasonRepository, SeasonRepository>();
        services.AddScoped<IEpisodeRepository, EpisodeRepository>();
        services.AddScoped<ISubtitleRepository, SubtitleRepository>();
        services.AddScoped<IAddictedUserCredentialRepository, AddictedUserCredentialRepository>();
        services.AddScoped<ITransactionManager, TransactionManager<EntityContext>>();
    }

    public EnvVarRegistration<EFCoreLicense, EFCoreLicenseParser> EnvVarRegistration => new("EFCORE");
}