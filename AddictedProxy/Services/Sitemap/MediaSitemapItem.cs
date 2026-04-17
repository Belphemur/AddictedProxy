namespace AddictedProxy.Services.Sitemap;

/// <summary>
/// Flat projection combining show-level and season-level data, used as the single
/// data source for the media sitemap so both URL types appear in the same paginated output.
/// </summary>
public sealed record MediaSitemapItem(
    Guid ShowUniqueId,
    string ShowName,
    int? SeasonNumber,
    DateTime LastModified,
    bool IsCompleted);
