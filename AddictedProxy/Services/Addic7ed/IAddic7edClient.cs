﻿using AddictedProxy.Model;
using AddictedProxy.Model.Config;
using AddictedProxy.Model.Shows;
using JetBrains.Annotations;

namespace AddictedProxy.Services.Addic7ed
{
    public interface IAddic7edClient
    {
        /// <summary>
        /// Get Tv Shows
        /// </summary>
        /// <param name="creds"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        IAsyncEnumerable<TvShow> GetTvShowsAsync(Addic7edCreds creds, CancellationToken token);

        /// <summary>
        /// Get available seasons
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="show"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<Season>> GetSeasonsAsync([CanBeNull] Addic7edCreds credentials, TvShow show, CancellationToken token);

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