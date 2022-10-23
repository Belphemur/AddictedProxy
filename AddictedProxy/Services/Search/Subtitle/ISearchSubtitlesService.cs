using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Model.Search;
using Ardalis.Result;

namespace AddictedProxy.Services.Search;

public interface ISearchSubtitlesService
{
    /// <summary>
    /// Find a show
    /// </summary>
    /// <param name="showName"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<Result<TvShow>> FindShowAsync(string showName, CancellationToken token);

    /// <summary>
    /// Find subtitles
    /// </summary>
    /// <param name="request"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<Result<SubtitleFound>> FindSubtitlesAsync(SearchPayload request, CancellationToken token);
}