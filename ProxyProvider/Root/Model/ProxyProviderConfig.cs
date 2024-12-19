namespace ProxyProvider.Root.Model;

public class ProxyProviderConfig
{
    /// <summary>
    /// How often to refresh the proxy list
    /// </summary>
    public TimeSpan RefreshInterval { get; init; } = TimeSpan.FromMinutes(2);
}