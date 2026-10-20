using AddictedProxy.Controllers.Rest;
using AddictedProxy.Model.Api;
using FluentAssertions;
using Microsoft.AspNetCore.Routing;

namespace AddictedProxy.Services.Tests.Controllers.Rest;

[TestFixture]
public class ApiIdParameterPolicyTests
{
    private const string Key = "showId";
    private static readonly Guid Sample = new("E9C1FA23-55AF-4711-8E34-3B31E2A75533");
    private const string SampleBase62 = "15tKKYmsRZNNgPJoOScXfn";

    private static readonly ApiIdParameterPolicy Policy = new();

    private static bool Match(RouteValueDictionary values, RouteDirection direction) =>
        Policy.Match(httpContext: null, route: null, routeKey: Key, values: values, routeDirection: direction);

    [Test]
    public void Match_IncomingUrl_CanonicalBase62_PassesConstraint()
    {
        Match(new RouteValueDictionary { [Key] = SampleBase62 }, RouteDirection.IncomingRequest).Should().BeTrue();
    }

    [TestCase("E9C1FA23-55AF-4711-8E34-3B31E2A75533")] // legacy dashed Guid rejected
    [TestCase("15tKKYmsRZNNgPJoOScXf")] // wrong length
    [TestCase("zzzzzzzzzzzzzzzzzzzzzz")] // overflow beyond 128 bits
    [TestCase("")]
    public void Match_IncomingUrl_NonCanonical_FailsClosed(string invalid)
    {
        Match(new RouteValueDictionary { [Key] = invalid }, RouteDirection.IncomingRequest).Should().BeFalse($"'{invalid}' must not match");
    }

    [Test]
    public void Match_LinkGeneration_TypedGuidValue_PassesConstraint()
    {
        // Outbound link generation evaluates constraints on route values before transformers run.
        Match(new RouteValueDictionary { [Key] = Sample }, RouteDirection.UrlGeneration).Should().BeTrue();
    }

    [Test]
    public void Match_MissingSegment_Fails()
    {
        Match(new RouteValueDictionary(), RouteDirection.IncomingRequest).Should().BeFalse();
    }

    [Test]
    public void TransformOutbound_Guid_EncodesToBase62()
    {
        Policy.TransformOutbound(Sample).Should().Be(SampleBase62);
    }

    [Test]
    public void TransformOutbound_AlreadyEncodedString_PassesThroughUnchanged()
    {
        // Composite ids (sp_{pack}_ep_{N}) arrive as strings and must not be double-encoded.
        const string composite = "sp_3j3OsqLGMsYABUdr7n9XsD_ep_3";
        Policy.TransformOutbound(composite).Should().Be(composite);
    }

    [Test]
    public void TransformOutbound_Null_ReturnsNull()
    {
        Policy.TransformOutbound(null).Should().BeNull();
    }

    [Test]
    public void GuidRouteValues_EncodeAndDecodeThroughTheSameFunnel()
    {
        // The wire contract: encode on the way out, decode on the way in.
        // Combine the route-link output with the download endpoint parser semantics.
        for (var i = 0; i < 64; i++)
        {
            var guid = Guid.NewGuid();
            var encoded = (string)Policy.TransformOutbound(guid)!;

            Base62Id.TryDecode(encoded, out var decoded).Should().BeTrue();
            decoded.Should().Be(guid);
        }
    }
}
