using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Model.Crendentials;

namespace AddictedProxy.Services.Provider.Seasons;

public interface ISeasonRefresher
{
    /// <summary>
    /// Refresh a specific season
    /// </summary>
    /// <param name="show"></param>
    /// <param name="seasonNumber"></param>
    /// <param name="timeBetweenChecks"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<Season?> GetRefreshSeasonAsync(TvShow show, int seasonNumber, TimeSpan timeBetweenChecks, CancellationToken token);

    /// <summary>
    /// Refresh the season of the given show
    /// </summary>
    /// <param name="show"></param>
    /// <param name="credentials"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<TvShow> RefreshSeasonsAsync(TvShow show, CredsContainer credentials, CancellationToken token);
}