#region

using AddictedProxy.Database.Context;
using AddictedProxy.Database.Context.Interceptor;
using AddictedProxy.Database.EnvVar;
using AddictedProxy.Database.Repositories.Credentials;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Database.Repositories.State;
using AddictedProxy.Tools.Database.Transaction;
using InversionOfControl.Model;
using InversionOfControl.Service.EnvironmentVariable.Registration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

#endregion

namespace AddictedProxy.Database.Bootstrap;

public class BootstrapDatabase : IBootstrap,
                                 IBootstrapEnvironmentVariable<EFCoreLicense, EFCoreLicenseParser>
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILoggingBuilder logging)
    {
        services.AddHostedService<SetupEfCoreHostedService>();
        services.AddScoped<SoftDeleteInterceptor>();
        services.AddDbContext<EntityContext>((serviceProvider, builder) =>
        {
            builder.EnableSensitiveDataLogging();
            builder.AddInterceptors(serviceProvider.GetRequiredService<SoftDeleteInterceptor>());
            builder.UseNpgsql(configuration.GetConnectionString("Addicted"), optionsBuilder =>
            {
                optionsBuilder.EnableRetryOnFailure();
            });
        });

        services.AddScoped<ITvShowRepository, TvShowRepository>();

        services.AddScoped<ISeasonRepository, SeasonRepository>();
        services.AddScoped<IEpisodeRepository, EpisodeRepository>();
        services.AddScoped<ISubtitleRepository, SubtitleRepository>();
        services.AddScoped<IShowExternalIdRepository, ShowExternalIdRepository>();
        services.AddScoped<IEpisodeExternalIdRepository, EpisodeExternalIdRepository>();
        services.AddScoped<ISeasonPackSubtitleRepository, SeasonPackSubtitleRepository>();
        services.AddScoped<ISeasonPackEntryRepository, SeasonPackEntryRepository>();
        services.AddScoped<ISuperSubtitlesStateRepository, SuperSubtitlesStateRepository>();
        services.AddScoped<IAddictedUserCredentialRepository, AddictedUserCredentialRepository>();
        services.AddScoped<ITransactionManager<EntityContext>, TransactionManager<EntityContext>>();
    }

    public EnvVarRegistration<EFCoreLicense, EFCoreLicenseParser> EnvVarRegistration => new("EFCORE");
}