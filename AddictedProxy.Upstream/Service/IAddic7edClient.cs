#region

using AddictedProxy.Database.Model.Credentials;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Upstream.Model;

#endregion

namespace AddictedProxy.Upstream.Service;

public interface IAddic7edClient
{
    /// <summary>
    ///     Get Tv Shows
    /// </summary>
    /// <param name="creds"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    IAsyncEnumerable<TvShow> GetTvShowsAsync(AddictedUserCredentials creds, CancellationToken token);

    /// <summary>
    ///     Get available seasons
    /// </summary>
    /// <param name="credentials"></param>
    /// <param name="show"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<IEnumerable<Season>> GetSeasonsAsync(AddictedUserCredentials credentials, TvShow show, CancellationToken token);

    /// <summary>
    ///     Get episode for the following season
    /// </summary>
    /// <param name="credentials"></param>
    /// <param name="show"></param>
    /// <param name="season"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<IEnumerable<Episode>> GetEpisodesAsync(AddictedUserCredentials credentials, TvShow show, int season, CancellationToken token);

    /// <summary>
    /// Get the download usage of a specific credential
    /// </summary>
    /// <param name="credentials"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<DownloadUsage> GetDownloadUsageAsync(AddictedUserCredentials credentials, CancellationToken token);

    /// <summary>
    /// Clean up the inbox for the given account
    /// </summary>
    /// <param name="creds"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<bool> CleanupInbox(AddictedUserCredentials creds, CancellationToken token);
}