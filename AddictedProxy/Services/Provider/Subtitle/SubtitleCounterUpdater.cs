using AddictedProxy.Database.Repositories.Shows;

namespace AddictedProxy.Services.Provider.Subtitle;

internal class SubtitleCounterUpdater
{
    private readonly ISubtitleRepository _subtitleRepository;

    public SubtitleCounterUpdater(ISubtitleRepository subtitleRepository)
    {
        _subtitleRepository = subtitleRepository;
    }

    /// <summary>
    /// Increment and save the subtitle count
    /// </summary>
    /// <param name="subtitle"></param>
    /// <param name="token"></param>
    public Task IncrementSubtitleCountAsync(Database.Model.Shows.Subtitle subtitle, CancellationToken token)
    {
        return _subtitleRepository.IncrementDownloadCountAsync(subtitle, token);
    }
}