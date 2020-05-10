using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
        /// Get nb of season the show has
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="show"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<int> GetNbSeasonsAsync([CanBeNull] Addic7edCreds credentials, TvShow show, CancellationToken token);

        /// <summary>
        /// Get episode for the following season
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="show"></param>
        /// <param name="season"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<Episode>> GetEpisodesAsync([CanBeNull] Addic7edCreds credentials, TvShow show, int season, CancellationToken token);
    }
}