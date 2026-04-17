using System.Web;
using AddictedProxy.Controllers.Rest;
using AddictedProxy.Culture.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleMvcSitemap;
using SimpleMvcSitemap.StyleSheets;

namespace AddictedProxy.Services.Sitemap;

public class MediaSitemapIndexConfiguration : SitemapIndexConfiguration<MediaSitemapItem>
{
    private readonly IUrlHelper _helper;

    public MediaSitemapIndexConfiguration(IQueryable<MediaSitemapItem> dataSource, int? currentPage, IUrlHelper helper, IOptions<SitemapConfig> sitemapConfig) : base(dataSource, currentPage)
    {
        _helper = helper;
        Size = 500;
        SitemapStyleSheets = [new XmlStyleSheet($"{sitemapConfig.Value.BaseUrl}/xsl/sitemap.xsl")];
        SitemapIndexStyleSheets = [new XmlStyleSheet($"{sitemapConfig.Value.BaseUrl}/xsl/sitemap.xsl")];
    }

    public override SitemapIndexNode CreateSitemapIndexNode(int page)
    {
        return new SitemapIndexNode(_helper.RouteUrl(Routes.MediaSitemap.ToString(), new { page }));
    }

    public override SitemapNode CreateNode(MediaSitemapItem item)
    {
        var slug = HttpUtility.UrlEncode(item.ShowName.ToSlug());
        var url = item.SeasonNumber.HasValue
            ? $"/shows/{item.ShowUniqueId}/{slug}/{item.SeasonNumber}"
            : $"/shows/{item.ShowUniqueId}/{slug}";

        var frequency = item.SeasonNumber.HasValue
            ? ChangeFrequency.Weekly
            : (item.IsCompleted ? ChangeFrequency.Monthly : ChangeFrequency.Daily);

        return new SitemapNode(url)
        {
            LastModificationDate = item.LastModified,
            ChangeFrequency = frequency
        };
    }
}