using AddictedProxy.Database.Model.Shows;

namespace AddictedProxy.Services.Provider.ShowInfo;

/// <summary>
/// Resolved TMDB data for a TV show.
/// </summary>
public record ShowTmdbInfo(int TmdbId, int? TvdbId, bool IsEnded);

/// <summary>
/// Resolved TMDB data for a movie.
/// </summary>
public record MovieTmdbInfo(int TmdbId, int? TvdbId);

/// <summary>
/// Resolves TMDB and TvDB IDs for shows and movies using name-based heuristics.
/// Encapsulates the filter logic (year, country, BBC) shared across multiple jobs.
/// </summary>
public interface IShowTmdbMapper
{
    /// <summary>
    /// Try to find TMDB (and TvDB) data for a TV show.
    /// Returns <c>null</c> if no match was found.
    /// </summary>
    Task<ShowTmdbInfo?> TryResolveShowAsync(TvShow show, CancellationToken token);

    /// <summary>
    /// Try to find TMDB (and TvDB) data for a movie.
    /// Returns <c>null</c> if no match was found.
    /// </summary>
    Task<MovieTmdbInfo?> TryResolveMovieAsync(TvShow show, CancellationToken token);
}
