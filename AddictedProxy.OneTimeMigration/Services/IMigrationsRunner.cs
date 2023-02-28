namespace AddictedProxy.OneTimeMigration.Services;

internal interface IMigrationsRunner
{
    Task RunMigrationAsync(CancellationToken token);
}