using AddictedProxy.Database.Context;
using AddictedProxy.OneTimeMigration.Model;
using Microsoft.EntityFrameworkCore;

namespace AddictedProxy.Migrations.Services;

[MigrationDate(2023, 09, 16)]
public class SetCreatedDateAndUpdatedDateSubtitlesMigration : IMigration
{
    private readonly EntityContext _entityContext;
    private readonly ILogger<SetCreatedDateAndUpdatedDateSubtitlesMigration> _logger;

    public SetCreatedDateAndUpdatedDateSubtitlesMigration(EntityContext entityContext, ILogger<SetCreatedDateAndUpdatedDateSubtitlesMigration> logger)
    {
        _entityContext = entityContext;
        _logger = logger;
    }

    public async Task ExecuteAsync(Hangfire.Server.PerformContext context, CancellationToken token)
    {
        try
        {
            _entityContext.Database.SetCommandTimeout(TimeSpan.FromMinutes(5));
            _logger.LogInformation("Migrating Subtitles");
            await _entityContext.Database.ExecuteSqlRawAsync("""UPDATE "Subtitles" SET "CreatedAt" = "Discovered", "UpdatedAt" = NOW() at time zone 'utc'""", cancellationToken: token);
            _logger.LogInformation("Subtitles Migrated");
        }
        finally
        {
            _entityContext.Database.SetCommandTimeout(TimeSpan.FromSeconds(30));
        }
    }
}