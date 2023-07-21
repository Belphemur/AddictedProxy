﻿#region

using AddictedProxy.Database.Context;
using AddictedProxy.Database.EnvVar;
using AddictedProxy.Database.Repositories.Credentials;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Tools.Database.Transaction;
using InversionOfControl.Model;
using InversionOfControl.Service.EnvironmentVariable.Registration;
using Microsoft.EntityFrameworkCore;
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
        services.AddDbContext<EntityContext>(builder =>
        {
            builder.EnableSensitiveDataLogging();
            builder.UseNpgsql(configuration.GetConnectionString("Addicted"), optionsBuilder =>
            {
                optionsBuilder.EnableRetryOnFailure();
            });
        });

        services.AddScoped<ITvShowRepository, TvShowRepository>();

        services.AddScoped<ISeasonRepository, SeasonRepository>();
        services.AddScoped<IEpisodeRepository, EpisodeRepository>();
        services.AddScoped<ISubtitleRepository, SubtitleRepository>();
        services.AddScoped<IAddictedUserCredentialRepository, AddictedUserCredentialRepository>();
        services.AddScoped<ITransactionManager<EntityContext>, TransactionManager<EntityContext>>();
    }

    public EnvVarRegistration<EFCoreLicense, EFCoreLicenseParser> EnvVarRegistration => new("EFCORE");
}