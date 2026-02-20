using AddictedProxy.Database.Context;
using AddictedProxy.Database.Model.Migration;
using AddictedProxy.OneTimeMigration.Model;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Performance.Service;
using System.Collections.Frozen;

namespace AddictedProxy.OneTimeMigration.Services;

internal class MigrationsRunner : IMigrationsRunner
{
    private readonly EntityContext _entityContext;
    private readonly IEnumerable<IMigration> _migrations;
    private readonly IPerformanceTracker _performanceTracker;

    public MigrationsRunner(EntityContext entityContext, IEnumerable<IMigration> migrations, IPerformanceTracker performanceTracker)
    {
        _entityContext = entityContext;
        _migrations = migrations;
        _performanceTracker = performanceTracker;
    }

    public async Task RunMigrationAsync(CancellationToken token)
    {
        using var span = _performanceTracker.BeginNestedSpan("running-migration", "Running one time migrations");
        var migrations = _migrations.ToDictionary(migration => migration.MigrationType);
        var success = await _entityContext.OneTimeMigrationRelease
            .Where(release => migrations.Keys.Contains(release.MigrationType))
            .Where(release => release.State == OneTimeMigrationRelease.MigrationState.Success)
            .ToDictionaryAsync(release => release.MigrationType, cancellationToken: token);

        foreach (var migrationType in migrations.Keys.Except(success.Keys))
        {
            BackgroundJob.Enqueue<RunnerJob>(job => job.RunMigrationAsync(migrationType, null, default));
        }
    }
}