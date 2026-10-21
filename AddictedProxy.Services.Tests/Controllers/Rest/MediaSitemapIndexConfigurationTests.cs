using AddictedProxy.Controllers.Rest;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Model.Api;
using AddictedProxy.Services.Sitemap;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;
using SimpleMvcSitemap;

namespace AddictedProxy.Services.Tests.Controllers.Rest;

[TestFixture]
public class MediaSitemapIndexConfigurationTests
{
    private const string VectorGuid = "1086727A-EB71-4B24-A209-7CF22374574D";
    private const string VectorBase62 = "0VBJpBmUXk2twqNJdYbHmH";

    private static MediaSitemapIndexConfiguration CreateConfiguration(params TvShow[] shows)
    {
        return new MediaSitemapIndexConfiguration(
            shows.AsQueryable(),
            0,
            Substitute.For<IUrlHelper>(),
            Options.Create(new SitemapConfig { BaseUrl = "https://localhost" }));
    }

    private static TvShow Show(Guid uniqueId, string name = "Test [Show]!") => new()
    {
        UniqueId = uniqueId,
        Name = name,
        LastUpdated = DateTime.UtcNow,
        IsCompleted = false,
    };

    [Test]
    public void CreateNode_EmitsBase62ShowId_SoTheFrontendShowLinksStayValid()
    {
        // Arrange — sitemap URLs feed the frontend show pages, whose apiId
        // routes fail closed on dashed Guids.
        var configuration = CreateConfiguration(Show(new Guid(VectorGuid)));

        // Act
        var node = configuration.CreateNode(Show(new Guid(VectorGuid)));

        // Assert
        node.Url.Should().StartWith($"/shows/{VectorBase62}/", "dashed Guids are rejected by the apiId route constraint");
        node.Url.Should().Contain("/test-show", "the slug halves of the URL are unchanged");
    }

    [Test]
    public void CreateNode_EncodedBase62IsRebuildableServerSide()
    {
        // The round-trip invariant of the API id contract: whatever the
        // sitemap emits must decode back to the show's Guid.
        var guid = new Guid(VectorGuid);
        var configuration = CreateConfiguration(Show(guid));

        var node = configuration.CreateNode(Show(guid));
        var segment = node.Url.Split('/')[2];

        Base62Id.TryDecode(segment, out var decoded).Should().BeTrue($"'{segment}' must decode");
        decoded.Should().Be(guid);
    }

    [Test]
    public void CreateNode_SlugsUnsafeCharacters()
    {
        var configuration = CreateConfiguration(Show(Guid.NewGuid()));

        var node = configuration.CreateNode(Show(Guid.NewGuid()));

        node.Url.Should().Contain("test-show");
    }
}
