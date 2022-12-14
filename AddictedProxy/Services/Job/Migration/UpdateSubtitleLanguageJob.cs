﻿using AddictedProxy.Culture.Service;
using AddictedProxy.Database.Context;
using AddictedProxy.Database.Model.Shows;
using Microsoft.EntityFrameworkCore;
using Sentry.Performance.Service;

namespace AddictedProxy.Services.Job.Migration;

public class UpdateSubtitleLanguageJob
{
    private readonly EntityContext _entityContext;
    private readonly ICultureParser _parser;
    private readonly ILogger<UpdateSubtitleLanguageJob> _logger;
    private readonly IPerformanceTracker _performanceTracker;
    private readonly IServiceProvider _provider;

    public UpdateSubtitleLanguageJob(EntityContext entityContext, ICultureParser parser, ILogger<UpdateSubtitleLanguageJob> logger, IPerformanceTracker performanceTracker, IServiceProvider provider)
    {
        _entityContext = entityContext;
        _parser = parser;
        _logger = logger;
        _performanceTracker = performanceTracker;
        _provider = provider;
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
            await using var scope = _provider.CreateAsyncScope();
            await using var db = scope.ServiceProvider.GetRequiredService<EntityContext>();
            subtitles = db.Subtitles.Where(subtitle => subtitle.LanguageIsoCode == null).OrderBy(subtitle => subtitle.Id).Take(10000).ToArray();
            
            await Parallel.ForEachAsync(subtitles.Chunk(500), token, UpdateSubtitleChunk);

            await db.SaveChangesAsync(token);
            count += subtitles.Length;
            _logger.LogInformation("[Migration] Language: {count}/{total} subtitles updated", count, total);
        } while (subtitles.Length > 0);
    }
}