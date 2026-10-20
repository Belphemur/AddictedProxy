using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using AddictedProxy.Controllers.Rest.Serializer;
using AddictedProxy.Model.Api;
using AddictedProxy.Model.Dto;
using FluentAssertions;

namespace AddictedProxy.Services.Tests.Model;

[TestFixture]
public class ApiIdJsonConverterTests
{
    private static ShowDto CreateShow(Guid id) => new(id, "Wellington Paranormal", 1, [1], 344280, 80475);

    private static readonly Guid Sample = new("E9C1FA23-55AF-4711-8E34-3B31E2A75533");
    private const string SampleBase62 = "15tKKYmsRZNNgPJoOScXfn";

    [Test]
    public void Serialize_SourceGenerationMetadata_WritesBase62()
    {
        // Arrange — the exact resolver bootstrap uses, but through the
        // source-generated metadata to prove the property-attached converter
        // survives the generated fast path.
        var typeInfo = (JsonTypeInfo<ShowDto>)SerializationContext.Default.GetTypeInfo(typeof(ShowDto))!;

        // Act
        var json = JsonSerializer.Serialize(CreateShow(Sample), typeInfo);

        // Assert
        json.Should().Contain($"\"Id\":\"{SampleBase62}\"");
    }

    [Test]
    public void Serialize_CombinedResolver_WritesBase62()
    {
        // Arrange — replication of the MVC options in BootstrapController.
        var options = new JsonSerializerOptions
        {
            TypeInfoResolver = JsonTypeInfoResolver.Combine(SerializationContext.Default, new DefaultJsonTypeInfoResolver()),
        };

        // Act
        var json = JsonSerializer.Serialize(CreateShow(Sample), options);

        // Assert
        json.Should().Contain($"\"Id\":\"{SampleBase62}\"");
    }

    [Test]
    public void Serialize_ReflectionOptions_WritesBase62WithoutDashes()
    {
        // Arrange
        var options = new JsonSerializerOptions();

        // Act
        var json = JsonSerializer.Serialize(CreateShow(Sample), options);

        // Assert
        json.Should().Contain($"\"Id\":\"{SampleBase62}\"");
    }

    [Test]
    public void Deserialize_CanonicalBase62_BuildsOriginalGuid()
    {
        // Arrange
        var options = new JsonSerializerOptions();
        const string json = """{"Id":"15tKKYmsRZNNgPJoOScXfn","Name":"Wellington Paranormal","NbSeasons":1,"Seasons":[1]}""";

        // Act
        var dto = JsonSerializer.Deserialize<ShowDto>(json, options);

        // Assert
        dto.Should().NotBeNull();
        dto!.Id.Should().Be(Sample);
    }

    [TestCase("15tKKYmsRZNNgPJoOScXf")] // wrong length
    [TestCase("E9C1FA23-55AF-4711-8E34-3B31E2A75533")] // legacy dashed Guid fails closed
    public void Deserialize_IllegalIdentifier_ThrowsJsonException(string invalidId)
    {
        // Arrange
        var options = new JsonSerializerOptions();
        var json = $$"""{"Id":"{{invalidId}}","Name":"Wellington Paranormal","NbSeasons":1,"Seasons":[1]}""";

        // Act
        var act = () => JsonSerializer.Deserialize<ShowDto>(json, options);

        // Assert
        act.Should().Throw<JsonException>();
    }
}
