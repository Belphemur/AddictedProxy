using AddictedProxy.Culture.Service;
using AddictedProxy.Database.Context;
using AddictedProxy.Database.Model.Shows;
using Microsoft.EntityFrameworkCore;
using Sentry.Performance.Service;

namespace AddictedProxy.Services.Job.Migration;

public sealed class UpdateSubtitleLanguageJob
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
        async ValueTask UpdateSubtitleChunk(Subtitle[] subtitles, CancellationToken cancellationToken)
        {
            foreach (var subtitle in subtitles)
            {
                var culture = await _parser.FromStringAsync(subtitle.Language, cancellationToken);
                if (culture == null)
                {
                    continue;
                }

                subtitle.LanguageIsoCode = culture.Name;
            }
        }

        using var _ = _performanceTracker.BeginNestedSpan("subtitles", "update-language");
        var count = 0L;
        var total = await _entityContext.Subtitles.Where(subtitle => subtitle.LanguageIsoCode == null).CountAsync(token);
        Subtitle[] subtitles;
        do
        {
            using var __ = _performanceTracker.BeginNestedSpan("subtitles-chunk", $"chunk {count}/{total}");
            subtitles = _entityContext.Subtitles.AsNoTracking().Where(subtitle => subtitle.LanguageIsoCode == null).OrderBy(subtitle => subtitle.Id).Take(10000).ToArray();

            await Parallel.ForEachAsync(subtitles.Chunk(500), token, UpdateSubtitleChunk);

            await _entityContext.Subtitles.BulkUpdateAsync(subtitles, token);
            count += subtitles.Length;
            _logger.LogInformation("[Migration] Language: {count}/{total} subtitles updated", count, total);
        } while (subtitles.Length > 0);
    }
}