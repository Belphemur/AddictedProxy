using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AddictedProxy.OneTimeMigration.Services;

internal class MigrationRunnerHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public MigrationRunnerHostedService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<IMigrationsRunner>().RunMigrationAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}