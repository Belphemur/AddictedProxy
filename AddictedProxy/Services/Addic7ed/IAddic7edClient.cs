using AddictedProxy.Model;
using AddictedProxy.Model.Config;
using JetBrains.Annotations;

namespace AddictedProxy.Services.Addic7ed
{
    public interface IAddic7edClient
    {
        /// <summary>
        /// Get Tv Shows
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<TvShow>> GetTvShowsAsync([CanBeNull]Addic7edCreds credentials, CancellationToken token);

        /// <summary>
        /// Get available seasons
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="show"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<int>> GetSeasonsAsync([CanBeNull] Addic7edCreds credentials, TvShow show, CancellationToken token);

        /// <summary>
        /// Get episode for the following season
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="show"></param>
        /// <param name="season"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<Episode>> GetEpisodesAsync([CanBeNull] Addic7edCreds credentials, TvShow show, int season, CancellationToken token);

        /// <summary>
        /// Download the given subtitle
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="lang"></param>
        /// <param name="id"></param>
        /// <param name="version"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<Stream> DownloadSubtitle(Addic7edCreds credentials, int lang, int id, int version, CancellationToken token);
    }
}