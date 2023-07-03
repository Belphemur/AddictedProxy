namespace AddictedProxy.Sitemap.Logic;

/// <summary>
/// Generate sitemap entries
/// </summary>
public interface ISitemapDynamicProvider
{
    /// <summary>
    /// Generate sitemap entries
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public IAsyncEnumerable<SitemapCore.Sitemap> GenerateAsync();
}