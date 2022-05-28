namespace AddictedProxy.Services.Provider.Config;

public class RefreshConfig
{
    public TimeSpan SeasonRefresh { get; init; }
    public TimeSpan EpisodeRefresh { get; init; }
    
    /// <summary>
    /// How long before a download exceeded cred can be reused
    /// </summary>
    public TimeSpan DownloadExceededTimeout { get; init; }
}