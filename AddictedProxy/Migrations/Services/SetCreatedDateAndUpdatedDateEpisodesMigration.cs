using AddictedProxy.Database.Context;
using AddictedProxy.OneTimeMigration.Model;
using Microsoft.EntityFrameworkCore;

namespace AddictedProxy.Migrations.Services;

[MigrationDate(2023, 09, 16)]
public class SetCreatedDateAndUpdatedDateEpisodesMigration : IMigration
{
    private readonly EntityContext _entityContext;
    private readonly ILogger<SetCreatedDateAndUpdatedDateEpisodesMigration> _logger;

    public SetCreatedDateAndUpdatedDateEpisodesMigration(EntityContext entityContext, ILogger<SetCreatedDateAndUpdatedDateEpisodesMigration> logger)
    {
        _entityContext = entityContext;
        _logger = logger;
    }

    public async Task ExecuteAsync(Hangfire.Server.PerformContext context, CancellationToken token)
    {
        try
        {
            _entityContext.Database.SetCommandTimeout(TimeSpan.FromMinutes(5));
            _logger.LogInformation("Migrating Episodes");
            await _entityContext.Database.ExecuteSqlRawAsync("""UPDATE "Episodes" SET "CreatedAt" = "Discovered", "UpdatedAt" = NOW() at time zone 'utc'""", cancellationToken: token);
            _logger.LogInformation("Episodes Migrated");
        }
        finally
        {
            _entityContext.Database.SetCommandTimeout(TimeSpan.FromSeconds(30));
        }
    }
}