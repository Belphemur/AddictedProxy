namespace AddictedProxy.Services.Provider.Merging.Model;

/// <summary>
/// Third-party IDs for matching shows across providers.
/// Used during show merging to find existing shows by TvDB, TMDB, or IMDB IDs.
/// </summary>
/// <param name="TvdbId">TvDB ID (most reliable for TV shows)</param>
/// <param name="ImdbId">IMDB ID (e.g. "tt1234567"), used as fallback via TMDB lookup</param>
/// <param name="TmdbId">TMDB ID, used as secondary match after TvDB</param>
public record ThirdPartyShowIds(int? TvdbId, string? ImdbId, int? TmdbId);
