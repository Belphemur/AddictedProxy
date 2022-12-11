using AddictedProxy.Culture.Service;
using AddictedProxy.Database.Context;
using Microsoft.EntityFrameworkCore;
using Sentry.Performance.Service;

namespace AddictedProxy.Services.Job.Migration;

public class UpdateSubtitleLanguageJob
{
    private readonly EntityContext _entityContext;
    private readonly CultureParser _parser;
    private readonly ILogger<UpdateSubtitleLanguageJob> _logger;
    private readonly IPerformanceTracker _performanceTracker;

    public UpdateSubtitleLanguageJob(EntityContext entityContext, CultureParser parser, ILogger<UpdateSubtitleLanguageJob> logger, IPerformanceTracker performanceTracker)
    {
        _entityContext = entityContext;
        _parser = parser;
        _logger = logger;
        _performanceTracker = performanceTracker;
    }

    public async Task ProcessAsync(CancellationToken token)
    {
        using var _ = _performanceTracker.BeginNestedSpan("subtitles", "update-language");
        var count = 0L;
        var total = await _entityContext.Subtitles.Where(subtitle => subtitle.LanguageIsoCode == null).CountAsync(token);
        var subtitles = _entityContext.Subtitles.Where(subtitle => subtitle.LanguageIsoCode == null).OrderBy(subtitle => subtitle.Id).Take(500).ToArray();
        do
        {
            using var __ = _performanceTracker.BeginNestedSpan("subtitles-chunk", $"chunk {count}/{total}");
            foreach (var subtitle in subtitles)
            {
                var culture = await _parser.FromStringAsync(subtitle.Language, token);
                if (culture == null)
                {
                    continue;
                }

                subtitle.LanguageIsoCode = culture.Name;
            }

            await _entityContext.BulkSaveChangesAsync(token);
            count += subtitles.Length;
            _logger.LogInformation("Updated {count}/{total} subtitles", count, total);

            subtitles = _entityContext.Subtitles.Where(subtitle => subtitle.LanguageIsoCode == null).OrderBy(subtitle => subtitle.Id).Take(500).ToArray();
        } while (subtitles.Length > 0);
    }
}