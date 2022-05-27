using AddictedProxy.Database.Model.Shows;

namespace AddictedProxy.Model.Dto;

/// <summary>
/// Episode information
/// </summary>
public class EpisodeDto
{
    internal EpisodeDto(int season, int number, string show)
    {
        Season = season;
        Number = number;
        Title = "N/A";
        Show = show;
        Discovered = DateTime.UtcNow;
    }

    public EpisodeDto(Episode episode)
    {
        Season = episode.Season;
        Number = episode.Number;
        Title = episode.Title;
        Discovered = episode.Discovered;
        Show = episode.TvShow.Name;
    }

    /// <summary>
    /// Season of the episode
    /// </summary>
    /// <example>1</example>
    public int Season { get; }

    /// <summary>
    /// Number of the episode
    /// </summary>
    /// <example>1</example>
    public int Number { get; }

    /// <summary>
    /// Title of the episode
    /// </summary>
    /// <example>Demon Girl</example>
    public string Title { get; }

    /// <summary>
    /// For which show
    /// </summary>
    /// <example>Wellington Paranormal</example>
    public string Show { get; }

    /// <summary>
    ///     When was the Episode discovered
    /// </summary>
    /// <example>2022-04-02T05:16:45.3996669</example>
    public DateTime Discovered { get; }
}