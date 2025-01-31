using System.Web;
using AddictedProxy.Controllers.Rest;
using AddictedProxy.Culture.Extensions;
using AddictedProxy.Database.Model.Shows;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleMvcSitemap;
using SimpleMvcSitemap.StyleSheets;

namespace AddictedProxy.Services.Sitemap;

public class MediaSitemapIndexConfiguration : SitemapIndexConfiguration<TvShow>
{
    private readonly IUrlHelper _helper;


    public MediaSitemapIndexConfiguration(IQueryable<TvShow> dataSource, int? currentPage, IUrlHelper helper, IOptions<SitemapConfig> sitemapConfig) : base(dataSource, currentPage)
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

    public override SitemapNode CreateNode(TvShow show)
    {
        return new SitemapNode($"/shows/{show.UniqueId}/{HttpUtility.UrlEncode(show.Name.ToSlug())}")
        {
            LastModificationDate = show.LastUpdated,
            ChangeFrequency = show.IsCompleted ? ChangeFrequency.Monthly : ChangeFrequency.Daily
        };
    }
}