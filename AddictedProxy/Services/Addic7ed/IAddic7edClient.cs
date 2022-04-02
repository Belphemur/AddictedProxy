using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Model.Config;
using AddictedProxy.Model.Shows;

namespace AddictedProxy.Services.Addic7ed;

public interface IAddic7edClient
{
    /// <summary>
    ///     Get Tv Shows
    /// </summary>
    /// <param name="creds"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    IAsyncEnumerable<TvShow> GetTvShowsAsync(Addic7edCreds creds, CancellationToken token);

    /// <summary>
    ///     Get available seasons
    /// </summary>
    /// <param name="credentials"></param>
    /// <param name="show"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<IEnumerable<Season>> GetSeasonsAsync(Addic7edCreds credentials, TvShow show, CancellationToken token);

    /// <summary>
    ///     Get episode for the following season
    /// </summary>
    /// <param name="credentials"></param>
    /// <param name="show"></param>
    /// <param name="season"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<IEnumerable<Episode>> GetEpisodesAsync(Addic7edCreds credentials, TvShow show, int season, CancellationToken token);
}