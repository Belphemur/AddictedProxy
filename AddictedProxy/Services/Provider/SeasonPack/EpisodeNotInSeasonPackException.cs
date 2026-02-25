namespace AddictedProxy.Services.Provider.SeasonPack;

/// <summary>
/// Thrown when a specific episode is not found in a season pack ZIP archive.
/// </summary>
public class EpisodeNotInSeasonPackException : Exception
{
    public int Episode { get; }

    public EpisodeNotInSeasonPackException(int episode, string message) : base(message)
    {
        Episode = episode;
    }
}
