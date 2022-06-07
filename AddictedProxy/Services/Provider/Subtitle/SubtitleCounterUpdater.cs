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
    public async Task IncrementSubtitleCountAsync(Database.Model.Shows.Subtitle subtitle, CancellationToken token)
    {
        subtitle.DownloadCount++;
        await _subtitleRepository.SaveChangeAsync(token);
    }
}