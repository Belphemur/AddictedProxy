using AddictedProxy.Database.Context;
using AddictedProxy.Database.Model.Migration;
using AddictedProxy.OneTimeMigration.Model;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Performance.Model;
using Performance.Service;

namespace AddictedProxy.OneTimeMigration.Services;

internal class RunnerJob
{
    private readonly EntityContext _entityContext;
    private readonly IPerformanceTracker _performanceTracker;
    private readonly IEnumerable<IMigration> _migrations;
    private readonly ILogger<RunnerJob> _logger;

    public RunnerJob(EntityContext entityContext, IPerformanceTracker performanceTracker, IEnumerable<IMigration> migrations, ILogger<RunnerJob> logger)
    {
        _entityContext = entityContext;
        _performanceTracker = performanceTracker;
        _migrations = migrations;
        _logger = logger;
    }

    public async Task RunMigrationAsync(string migrationType, PerformContext context, CancellationToken token)
    {
        context.WriteLine($"Starting migration: {migrationType}");
        using var span = _performanceTracker.BeginNestedSpan("migration");
        span.SetTag("migration.type", migrationType);

        using var scope = _logger.BeginScope(migrationType);
        _logger.LogInformation("Starting migration");
        var migration = _migrations.Single(mig => mig.MigrationType == migrationType);
        var migrationEntry = await _entityContext.OneTimeMigrationRelease.FirstOrDefaultAsync(release => release.MigrationType == migration.MigrationType, cancellationToken: token);
        if (migrationEntry == null)
        {
            migrationEntry = new OneTimeMigrationRelease
            {
                MigrationType = migration.MigrationType
            };
            await _entityContext.OneTimeMigrationRelease.AddAsync(migrationEntry, token);
        }

        migrationEntry.State = OneTimeMigrationRelease.MigrationState.Running;

        await _entityContext.SaveChangesAsync(token);

        try
        {
            await migration.ExecuteAsync(token);
            migrationEntry.State = OneTimeMigrationRelease.MigrationState.Success;
            _logger.LogInformation("Successfully migrated");
            context.WriteLine($"Successfully completed migration: {migrationType}");
        }
        catch (Exception e)
        {
            migrationEntry.State = OneTimeMigrationRelease.MigrationState.Fail;
            _logger.LogError(e, "Couldn't migrate");
            context.WriteLine($"Error: Migration {migrationType} failed: {e.Message}");
            span.Finish(e, Status.InternalError);
            throw;
        }
        finally
        {
            await _entityContext.SaveChangesAsync(token);
        }
    }
}