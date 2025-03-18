namespace ProxyScrape.Model;

public class ProxyScrapeConfig
{
    public class Creds
    {
        public string Username { get; init; } = null!;
        public string Password { get; init; } = null!;
    }

    public string AccountId { get; init; } = null!;
    public string SubUserId { get; init; } = null!;
    public Creds User { get; init; } = null!;
    
    public TimeSpan ScrapeInterval { get; init; } = TimeSpan.FromSeconds(30);
    
    public bool EnableScrape { get; init; } = true;
}