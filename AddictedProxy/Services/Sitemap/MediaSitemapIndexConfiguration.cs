using System.Web;
using AddictedProxy.Controllers.Rest;
using AddictedProxy.Database.Model.Shows;
using Microsoft.AspNetCore.Mvc;
using SimpleMvcSitemap;
using SimpleMvcSitemap.StyleSheets;

namespace AddictedProxy.Services.Sitemap;

public class MediaSitemapIndexConfiguration : SitemapIndexConfiguration<TvShow>
{
    private readonly IUrlHelper _helper;
    

    public MediaSitemapIndexConfiguration(IQueryable<TvShow> dataSource, int? currentPage, IUrlHelper helper) : base(dataSource, currentPage)
    {
        _helper = helper;
        Size = 500;
        SitemapStyleSheets = new List<XmlStyleSheet> { new("https://raw.githubusercontent.com/pedroborges/xml-sitemap-stylesheet/master/sitemap.xsl") };
        SitemapIndexStyleSheets = new List<XmlStyleSheet> { new("https://raw.githubusercontent.com/pedroborges/xml-sitemap-stylesheet/master/sitemap.xsl") };
    }

    public override SitemapIndexNode CreateSitemapIndexNode(int page)
    {
        return new SitemapIndexNode(_helper.RouteUrl(Routes.MediaSitemap.ToString(), new { page }));
    }

    public override SitemapNode CreateNode(TvShow show)
    {
        return new SitemapNode($"/shows/{show.UniqueId}/{HttpUtility.UrlEncode(show.Name)}")
        {
            LastModificationDate = show.LastUpdated,
            ChangeFrequency = show.IsCompleted? ChangeFrequency.Monthly : ChangeFrequency.Daily
        };
    }
}