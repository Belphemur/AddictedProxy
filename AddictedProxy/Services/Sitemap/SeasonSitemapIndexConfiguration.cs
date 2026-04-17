using System.Web;
using AddictedProxy.Controllers.Rest;
using AddictedProxy.Culture.Extensions;
using AddictedProxy.Database.Model.Shows;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleMvcSitemap;
using SimpleMvcSitemap.StyleSheets;

namespace AddictedProxy.Services.Sitemap;

public class SeasonSitemapIndexConfiguration : SitemapIndexConfiguration<Season>
{
    private readonly IUrlHelper _helper;

    public SeasonSitemapIndexConfiguration(IQueryable<Season> dataSource, int? currentPage, IUrlHelper helper, IOptions<SitemapConfig> sitemapConfig) : base(dataSource, currentPage)
    {
        _helper = helper;
        Size = 500;
        SitemapStyleSheets = [new XmlStyleSheet($"{sitemapConfig.Value.BaseUrl}/xsl/sitemap.xsl")];
        SitemapIndexStyleSheets = [new XmlStyleSheet($"{sitemapConfig.Value.BaseUrl}/xsl/sitemap.xsl")];
    }

    public override SitemapIndexNode CreateSitemapIndexNode(int page)
    {
        return new SitemapIndexNode(_helper.RouteUrl(Routes.SeasonSitemap.ToString(), new { page }));
    }

    public override SitemapNode CreateNode(Season season)
    {
        var slug = HttpUtility.UrlEncode(season.TvShow.Name.ToSlug());
        return new SitemapNode($"/shows/{season.TvShow.UniqueId}/{slug}/{season.Number}")
        {
            LastModificationDate = season.LastRefreshed ?? season.TvShow.LastUpdated,
            ChangeFrequency = ChangeFrequency.Weekly
        };
    }
}
