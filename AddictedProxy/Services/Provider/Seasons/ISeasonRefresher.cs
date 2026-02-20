using AddictedProxy.Database.Model.Shows;

namespace AddictedProxy.Services.Provider.Seasons;

public interface ISeasonRefresher
{
    /// <summary>
    /// Refresh a specific season
    /// </summary>
    /// <param name="show"></param>
    /// <param name="seasonNumber"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<Season?> GetRefreshSeasonAsync(TvShow show, int seasonNumber, CancellationToken token);

    /// <summary>
    /// Refresh the season of the given show
    /// </summary>
    /// <param name="show"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task RefreshSeasonsAsync(TvShow show, CancellationToken token = default);

    /// <summary>
    /// Does the show need to have its seasons refreshed from any registered provider
    /// </summary>
    /// <param name="show"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<bool> IsShowNeedsRefreshAsync(TvShow show, CancellationToken token);
}