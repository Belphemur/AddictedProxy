namespace ProxyScrape.Model;

public class ProxyScrapeConfig
{
    public class Creds
    {
        public string Username { get; init; }
        public string Password { get; init; }
    }

    public string AccountId { get; init; }
    public string SubUserId { get; init; }
    public Creds User { get; init; }
}