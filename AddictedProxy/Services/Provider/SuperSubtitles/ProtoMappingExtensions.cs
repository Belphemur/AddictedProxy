using AddictedProxy.Database.Model.Shows;
using SuperSubtitleClient.Generated;

using ProtoSubtitle = SuperSubtitleClient.Generated.Subtitle;
using SubtitleEntity = AddictedProxy.Database.Model.Shows.Subtitle;

namespace AddictedProxy.Services.Provider.SuperSubtitles;

/// <summary>
/// Extension/mapping helpers for converting SuperSubtitles protobuf types to domain types.
/// </summary>
internal static class ProtoMappingExtensions
{
    /// <summary>
    /// Converts a collection of protobuf <see cref="Quality"/> values to a <see cref="VideoQuality"/> bitmask.
    /// </summary>
    public static VideoQuality ToVideoQuality(this IEnumerable<Quality> protoQualities)
    {
        var result = VideoQuality.None;
        foreach (var q in protoQualities)
        {
            result |= q switch
            {
                Quality._360P  => VideoQuality.Q360P,
                Quality._480P  => VideoQuality.Q480P,
                Quality._720P  => VideoQuality.Q720P,
                Quality._1080P => VideoQuality.Q1080P,
                Quality._2160P => VideoQuality.Q2160P,
                _              => VideoQuality.None,
            };
        }
        return result;
    }

    /// <summary>
    /// Converts a proto <see cref="ProtoSubtitle"/> to a <see cref="SubtitleEntity"/> domain entity.
    /// </summary>
    /// <param name="subtitle">The proto subtitle</param>
    /// <param name="languageIsoCode">Pre-resolved ISO language code (e.g. "en"), or null if the language could not be parsed</param>
    public static SubtitleEntity ToSubtitleEntity(this ProtoSubtitle subtitle, string? languageIsoCode)
    {
        if (string.IsNullOrEmpty(subtitle.DownloadUrl))
            throw new ArgumentException($"Subtitle {subtitle.Id} has no download URL", nameof(subtitle));

        return new SubtitleEntity
        {
            Scene = string.Join(", ", subtitle.ReleaseGroups),
            Version = 0,
            Completed = true,
            CompletionPct = 100.0,
            HearingImpaired = false,
            Corrected = false,
            Qualities = subtitle.Qualities.ToVideoQuality(),
            Release = string.IsNullOrEmpty(subtitle.Release) ? null : subtitle.Release,
            DownloadUri = new Uri(subtitle.DownloadUrl),
            Language = subtitle.Language,
            LanguageIsoCode = languageIsoCode,
            Discovered = DateTime.UtcNow,
            Source = DataSource.SuperSubtitles,
            ExternalId = subtitle.Id.ToString()
        };
    }

    /// <summary>
    /// Converts a proto <see cref="ProtoSubtitle"/> (with <c>IsSeasonPack = true</c>) to a <see cref="SeasonPackSubtitle"/> domain entity.
    /// </summary>
    /// <param name="subtitle">The proto subtitle (must be a season pack)</param>
    /// <param name="tvShowId">The database TvShow ID to link to</param>
    /// <param name="languageIsoCode">Pre-resolved ISO language code (e.g. "en"), or null if the language could not be parsed</param>
    public static SeasonPackSubtitle ToSeasonPackSubtitle(this ProtoSubtitle subtitle, long tvShowId, string? languageIsoCode)
    {
        return new SeasonPackSubtitle
        {
            TvShowId = tvShowId,
            Season = subtitle.Season,
            Source = DataSource.SuperSubtitles,
            ExternalId = subtitle.Id,
            Filename = subtitle.Filename,
            Language = subtitle.Language,
            LanguageIsoCode = languageIsoCode,
            Release = string.IsNullOrEmpty(subtitle.Release) ? null : subtitle.Release,
            Uploader = string.IsNullOrEmpty(subtitle.Uploader) ? null : subtitle.Uploader,
            UploadedAt = subtitle.UploadedAt?.ToDateTime(),
            Qualities = subtitle.Qualities.ToVideoQuality(),
            ReleaseGroups = subtitle.ReleaseGroups.Count > 0
                ? string.Join(",", subtitle.ReleaseGroups)
                : null,
            Discovered = DateTime.UtcNow
        };
    }
}
