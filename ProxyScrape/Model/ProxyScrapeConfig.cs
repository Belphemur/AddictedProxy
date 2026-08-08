namespace ProxyScrape.Model;

public class ProxyScrapeConfig
{
    public string AccountId { get; init; } = null!;
    public string SubUserId { get; init; } = null!;
    public string ApiToken { get; init; } = null!;
    public TimeSpan ScrapeInterval { get; init; } = TimeSpan.FromSeconds(30);
    public bool EnableScrape { get; init; } = true;
}
