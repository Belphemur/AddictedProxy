﻿using AddictedProxy.Migrations.Services;
using AddictedProxy.OneTimeMigration.Model;
using InversionOfControl.Model;

namespace AddictedProxy.Migrations.Bootstrap;

public class BootstrapMigration : IBootstrap
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILoggingBuilder logging)
    {
        services.AddScoped<IMigration, PopulateTvDbIdsMigration>();
        services.AddScoped<IMigration, SetCreatedDateAndUpdatedDateEpisodesMigration>();
        services.AddScoped<IMigration, SetCreatedDateAndUpdatedDateSubtitlesMigration>();
        services.AddScoped<IMigration, CleanUpInboxUsersMigration>();
        services.AddScoped<IMigration, RemoveOldCheckCompletedJobMigration>();


    }
}