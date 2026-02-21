using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Services.Provider.Merging.Model;

using SubtitleEntity = AddictedProxy.Database.Model.Shows.Subtitle;

namespace AddictedProxy.Services.Provider.Merging;

/// <summary>
/// Service responsible for merging provider data into the unified database model.
/// Handles show matching/creation, episode upserts with subtitles, and season pack ingestion.
/// Used by background jobs (bulk import and incremental refresh) to ingest data from any provider.
/// </summary>
public interface IProviderDataIngestionService
{
    /// <summary>
    /// Match an existing <see cref="TvShow"/> or create a new one based on provider data.
    /// <para>
    /// Lookup order:
    /// 1. <see cref="ShowExternalId"/> for the given source + provider ID (fast path for already-imported shows)
    /// 2. TvDB ID match
    /// 3. TMDB ID match
    /// 4. IMDB → TMDB lookup (via TMDB API), then TMDB ID match
    /// 5. Create new <see cref="TvShow"/>
    /// </para>
    /// Also upserts the <see cref="ShowExternalId"/> and backfills missing TvDB/TMDB IDs on matched shows.
    /// </summary>
    /// <param name="source">The provider source (e.g. SuperSubtitles)</param>
    /// <param name="providerExternalId">The provider-specific show ID</param>
    /// <param name="showName">The show name from the provider</param>
    /// <param name="thirdPartyIds">Optional third-party IDs (TvDB, IMDB, TMDB) for matching</param>
    /// <param name="token">Cancellation token</param>
    /// <returns>The matched or newly created <see cref="TvShow"/></returns>
    Task<TvShow> MergeShowAsync(
        DataSource source,
        string providerExternalId,
        string showName,
        ThirdPartyShowIds? thirdPartyIds,
        CancellationToken token);

    /// <summary>
    /// Merge an episode and its subtitle into the database.
    /// Creates the <see cref="Season"/> entity if it doesn't exist yet.
    /// The episode external ID (if provided) is stored in the episode's <see cref="Episode.ExternalIds"/> collection
    /// and atomically upserted alongside the episode and subtitle in a single SQL CTE.
    /// </summary>
    /// <param name="show">The parent TvShow (must have a valid Id)</param>
    /// <param name="source">The provider source</param>
    /// <param name="season">Season number</param>
    /// <param name="episodeNumber">Episode number</param>
    /// <param name="episodeTitle">Optional episode title</param>
    /// <param name="episodeExternalId">Optional provider-specific episode external ID</param>
    /// <param name="subtitle">The subtitle to attach to the episode</param>
    /// <param name="token">Cancellation token</param>
    Task MergeEpisodeSubtitleAsync(
        TvShow show,
        DataSource source,
        int season,
        int episodeNumber,
        string? episodeTitle,
        string? episodeExternalId,
        SubtitleEntity subtitle,
        CancellationToken token);

    /// <summary>
    /// Ingest a season pack subtitle into the <see cref="SeasonPackSubtitle"/> table.
    /// Season packs are not linked to individual episodes — they cover an entire season.
    /// </summary>
    /// <param name="seasonPack">The season pack subtitle to upsert</param>
    /// <param name="token">Cancellation token</param>
    Task IngestSeasonPackAsync(SeasonPackSubtitle seasonPack, CancellationToken token);

    /// <summary>
    /// Bulk merge multiple episodes (with their <see cref="Episode.Subtitles"/> and <see cref="Episode.ExternalIds"/> populated)
    /// for a show. Creates <see cref="Season"/> entities for any new season numbers encountered.
    /// </summary>
    /// <param name="show">The parent TvShow (must have a valid Id)</param>
    /// <param name="episodes">Episodes with their Subtitles and ExternalIds collections populated</param>
    /// <param name="token">Cancellation token</param>
    Task MergeEpisodesWithSubtitlesAsync(
        TvShow show,
        IEnumerable<Episode> episodes,
        CancellationToken token);

    /// <summary>
    /// Bulk ingest multiple season pack subtitles into the <see cref="SeasonPackSubtitle"/> table.
    /// </summary>
    /// <param name="seasonPacks">The season pack subtitles to upsert</param>
    /// <param name="token">Cancellation token</param>
    Task IngestSeasonPacksAsync(IEnumerable<SeasonPackSubtitle> seasonPacks, CancellationToken token);
}
