using AddictedProxy.Migrations.Services;
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
        services.AddScoped<IMigration, MigrateExternalIdsToNewTableMigration>();
        services.AddScoped<IMigration, MigrateSubtitleExternalIdMigration>();
        services.AddScoped<IMigration, BackportHdToQualitiesMigration>();
        services.AddScoped<IMigration, CleanSuperSubtitlesDataMigration>();
    }
}