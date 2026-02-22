using AddictedProxy.Database.Context;
using AddictedProxy.Database.Model;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.OneTimeMigration.Model;
using Microsoft.EntityFrameworkCore;
using TvMovieDatabaseClient.Service;

namespace AddictedProxy.Migrations.Services;

[MigrationDate(2023, 02, 29)]
public class PopulateTvDbIdsMigration : IMigration
{
    private readonly EntityContext _entityContext;
    private readonly ITMDBClient _tmdbClient;

    public PopulateTvDbIdsMigration(EntityContext entityContext, ITMDBClient tmdbClient)
    {
        _entityContext = entityContext;
        _tmdbClient = tmdbClient;
    }

    public async Task ExecuteAsync(Hangfire.Server.PerformContext context, CancellationToken token)
    {
        var count = 0;
        foreach (var tvShow in await _entityContext.TvShows.Where(show => show.TmdbId != null)
                                                   .Where(show => show.TvdbId == null)
                                                   .ToArrayAsync(token))
        {
            var externalIds = await (tvShow.Type switch
            {
                ShowType.Show  => _tmdbClient.GetShowExternalIdsAsync(tvShow.TmdbId!.Value, token),
                ShowType.Movie => _tmdbClient.GetMovieExternalIdsAsync(tvShow.TmdbId!.Value, token),
                _              => throw new ArgumentOutOfRangeException()
            });

            if (externalIds == null)
            {
                continue;
            }

            tvShow.TvdbId = externalIds.TvdbId;

            if (++count % 50 == 0)
            {
                await _entityContext.BulkSaveChangesAsync(token);
            }
        }

        await _entityContext.BulkSaveChangesAsync(token);
    }
}