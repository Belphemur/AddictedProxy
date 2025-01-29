using AddictedProxy.OneTimeMigration.Model;
using AddictedProxy.Services.Provider.Shows.Jobs;
using Hangfire;

namespace AddictedProxy.Migrations.Services;

[MigrationDate(2025, 01, 29)]
public class RemoveOldCheckCompletedJobMigration : IMigration
{
    public Task ExecuteAsync(CancellationToken token)
    {
        RecurringJob.RemoveIfExists(nameof(CheckCompletedTmdbJob));
        return Task.CompletedTask;
    }
}