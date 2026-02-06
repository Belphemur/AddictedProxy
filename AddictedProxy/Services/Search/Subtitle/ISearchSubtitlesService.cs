using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Model.Search;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AddictedProxy.Services.Search;

public interface ISearchSubtitlesService
{
    /// <summary>
    /// Find a show
    /// </summary>
    /// <param name="showName"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<Results<Ok<TvShow>, NotFound>> FindShowAsync(string showName, CancellationToken token);

    /// <summary>
    /// Get a show by unique id
    /// </summary>
    /// <param name="showUniqueId"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<Results<Ok<TvShow>, NotFound>> GetByShowUniqueIdAsync(Guid showUniqueId, CancellationToken token);

    /// <summary>
    /// Find subtitles. Returns StatusCodeHttpResult with 423 when language is invalid (resource locked/refreshing).
    /// </summary>
    /// <param name="request"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<Results<Ok<SubtitleFound>, StatusCodeHttpResult>> FindSubtitlesAsync(SearchPayload request, CancellationToken token);
}