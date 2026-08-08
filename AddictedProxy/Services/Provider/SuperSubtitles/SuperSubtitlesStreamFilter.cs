using AddictedProxy.Database.Model.Shows;
using Microsoft.Extensions.Logging;

using ProtoSubtitle = SuperSubtitleClient.Generated.Subtitle;

namespace AddictedProxy.Services.Provider.SuperSubtitles;

/// <summary>
/// Filters SuperSubtitles subtitle streams against the show's known TMDB season count,
/// dropping subtitles for seasons that cannot exist (polluted upstream data).
/// </summary>
public static class SuperSubtitlesStreamFilter
{
    /// <summary>
    /// Drop subtitles whose season exceeds the show's TMDB season count.
    /// Season 0 (specials) is always kept. When the season count is unknown, everything is kept.
    /// </summary>
    public static IReadOnlyList<ProtoSubtitle> DropInvalidSeasons(TvShow show, IEnumerable<ProtoSubtitle> subtitles, ILogger logger)
    {
        if (!show.NumberOfSeasons.HasValue)
        {
            return subtitles.ToList();
        }

        var maxSeason = show.NumberOfSeasons.Value;
        var kept = new List<ProtoSubtitle>();
        var droppedSeasons = new SortedSet<int>();
        var droppedCount = 0;

        foreach (var subtitle in subtitles)
        {
            if (subtitle.Season > 0 && subtitle.Season > maxSeason)
            {
                droppedSeasons.Add(subtitle.Season);
                droppedCount++;
                continue;
            }

            kept.Add(subtitle);
        }

        if (droppedCount > 0)
        {
            logger.LogWarning(
                "Dropped {Count} SuperSubtitles subtitles for {ShowName}: invalid seasons [{Seasons}] exceed TMDB season count {Max}",
                droppedCount, show.Name, string.Join(", ", droppedSeasons), maxSeason);
        }

        return kept;
    }
}
