using Microsoft.Extensions.Hosting;

namespace AddictedProxy.OneTimeMigration.Services;

internal class MigrationRunnerHostedService : IHostedService
{
    private readonly IMigrationsRunner _migrationsRunner;

    public MigrationRunnerHostedService(IMigrationsRunner migrationsRunner)
    {
        _migrationsRunner = migrationsRunner;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _migrationsRunner.RunMigrationAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}