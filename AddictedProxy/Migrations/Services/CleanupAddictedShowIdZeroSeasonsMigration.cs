using AddictedProxy.Database.Context;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.OneTimeMigration.Model;
using AddictedProxy.Services.Provider.Shows.Jobs;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AddictedProxy.Migrations.Services;

/// <summary>
/// One-time migration that removes bogus seasons/episodes created by the wrong-show-ID bug
/// (fixed in commit e6a44578 "fix: Not refreshing the right show ID"): the legacy
/// <see cref="TvShow.ExternalId" /> was sent to Addic7ed instead of the provider-mapped
/// <see cref="ShowExternalId" />. For shows whose legacy ExternalId is 0 (shows that were
/// only mapped via <see cref="ShowExternalId" />), Addic7ed's showID=0 endpoint returned
/// seasons 0, 3, 5, 8 and 9 — so those bogus seasons and their episodes were attached to
/// every impacted show.
///
/// Targets shows with <see cref="TvShow.ExternalId" /> = 0 that own an Addic7ed
/// <see cref="ShowExternalId" />. Deletes their seasons 0/3/5/8/9 and the episodes of
/// those seasons, but only episodes holding exclusively Addic7ed
/// <see cref="EpisodeExternalId" />s — episodes known to another provider are real data:
/// kept along with their season row. Seasons holding season packs are also preserved,
/// matching <see cref="CleanupEmptySeasonsMigrationAgain" /> semantics.
///
/// Finally nulls <see cref="TvShow.LastSeasonRefreshed" /> for the impacted shows and
/// enqueues a <see cref="RefreshSingleShowJob" /> for each so seasons and episodes are
/// re-fetched with the correct Addic7ed show ID.
/// </summary>
[MigrationDate(2026, 8, 10)]
public class CleanupAddictedShowIdZeroSeasonsMigration : IMigration
{
    private const int Addic7edSource = (int)DataSource.Addic7ed;

    /// <summary>
    /// Season numbers Addic7ed returns for showID=0
    /// (from the qsiSeason select: 0, 3, 5, 8, 9, skipping the "Season" placeholder).
    /// </summary>
    private static readonly int[] PollutedSeasonNumbers = [0, 3, 5, 8, 9];

    private static readonly string PollutedSeasonNumbersSql = string.Join(", ", PollutedSeasonNumbers);

    private readonly EntityContext _entityContext;
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly ILogger<CleanupAddictedShowIdZeroSeasonsMigration> _logger;

    public CleanupAddictedShowIdZeroSeasonsMigration(
        EntityContext entityContext,
        IBackgroundJobClient backgroundJobClient,
        ILogger<CleanupAddictedShowIdZeroSeasonsMigration> logger)
    {
        _entityContext = entityContext;
        _backgroundJobClient = backgroundJobClient;
        _logger = logger;
    }

    public async Task ExecuteAsync(PerformContext context, CancellationToken token)
    {
        context.WriteLine("Cleaning up seasons 0/3/5/8/9 fetched from Addic7ed showID=0...");
        _logger.LogInformation("Cleaning up seasons fetched from Addic7ed showID=0...");

        var impactedShowIds = await _entityContext.TvShows
            .AsNoTracking()
            .Where(show => show.ExternalId == 0)
            .Where(show => show.ExternalIds.Any(extId => extId.Source == DataSource.Addic7ed))
            .Where(show => show.Seasons.Any(season => PollutedSeasonNumbers.Contains(season.Number)))
            .Select(show => show.Id)
            .ToListAsync(token);

        if (impactedShowIds.Count == 0)
        {
            context.WriteLine("No impacted shows found, nothing to do.");
            _logger.LogInformation("No impacted shows found, nothing to do.");
            return;
        }

        context.WriteLine($"Found {impactedShowIds.Count} impacted show(s) with bogus seasons from Addic7ed showID=0");
        _logger.LogInformation("Found {Count} impacted show(s) with bogus seasons from Addic7ed showID=0", impactedShowIds.Count);

        var showsReset = await _entityContext.Database.ExecuteSqlAsync(
            $"""
             UPDATE "TvShows"
             SET "LastSeasonRefreshed" = NULL
             WHERE "ExternalId" = 0
               AND EXISTS (
                   SELECT 1 FROM "ShowExternalIds" se
                   WHERE se."TvShowId" = "TvShows"."Id" AND se."Source" = {Addic7edSource}
               )
               AND EXISTS (
                   SELECT 1 FROM "Seasons" s
                   WHERE s."TvShowId" = "TvShows"."Id" AND s."Number" IN ({PollutedSeasonNumbersSql})
               )
             """, token);

        context.WriteLine($"Nulled LastSeasonRefreshed on {showsReset} show(s)");
        _logger.LogInformation("Nulled LastSeasonRefreshed on {Count} show(s)", showsReset);

        var episodesDeleted = await _entityContext.Database.ExecuteSqlAsync(
            $"""
             DELETE FROM "Episodes" e
             WHERE e."Season" IN ({PollutedSeasonNumbersSql})
               AND EXISTS (
                   SELECT 1 FROM "TvShows" t
                   WHERE t."Id" = e."TvShowId"
                     AND t."ExternalId" = 0
                     AND EXISTS (
                         SELECT 1 FROM "ShowExternalIds" se
                         WHERE se."TvShowId" = t."Id" AND se."Source" = {Addic7edSource}
                     )
               )
               AND NOT EXISTS (
                   SELECT 1 FROM "EpisodeExternalIds" ee
                   WHERE ee."EpisodeId" = e."Id" AND ee."Source" <> {Addic7edSource}
               )
             """, token);

        context.WriteLine($"Deleted {episodesDeleted} bogus episode(s) (addicted-only external IDs)");
        _logger.LogInformation("Deleted {Count} bogus episode(s) (addicted-only external IDs)", episodesDeleted);

        var seasonsDeleted = await _entityContext.Database.ExecuteSqlAsync(
            $"""
             DELETE FROM "Seasons" s
             WHERE s."Number" IN ({PollutedSeasonNumbersSql})
               AND EXISTS (
                   SELECT 1 FROM "TvShows" t
                   WHERE t."Id" = s."TvShowId"
                     AND t."ExternalId" = 0
                     AND EXISTS (
                         SELECT 1 FROM "ShowExternalIds" se
                         WHERE se."TvShowId" = t."Id" AND se."Source" = {Addic7edSource}
                     )
               )
               AND NOT EXISTS (
                   SELECT 1 FROM "Episodes" e
                   WHERE e."TvShowId" = s."TvShowId" AND e."Season" = s."Number"
               )
               AND NOT EXISTS (
                   SELECT 1 FROM "SeasonPackSubtitles" sp
                   WHERE sp."TvShowId" = s."TvShowId" AND sp."Season" = s."Number"
               )
             """, token);

        context.WriteLine($"Deleted {seasonsDeleted} bogus season(s) (no remaining episodes or season packs)");
        _logger.LogInformation("Deleted {Count} bogus season(s) (no remaining episodes or season packs)", seasonsDeleted);

        context.WriteLine($"Enqueuing {impactedShowIds.Count} RefreshSingleShowJob(s)...");
        _logger.LogInformation("Enqueuing {Count} RefreshSingleShowJob(s)...", impactedShowIds.Count);

        foreach (var showId in impactedShowIds)
        {
            _backgroundJobClient.Enqueue<RefreshSingleShowJob>(job =>
                job.ExecuteAsync(showId, null!, default));
        }

        context.WriteLine($"Enqueued {impactedShowIds.Count} show-refresh job(s). Done.");
        _logger.LogInformation("Enqueued {Count} show-refresh job(s). Done.", impactedShowIds.Count);
    }
}
