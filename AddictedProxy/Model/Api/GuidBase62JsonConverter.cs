using System.Text.Json;
using System.Text.Json.Serialization;

namespace AddictedProxy.Model.Api;

/// <summary>
/// Serializes <see cref="Guid"/> API identifiers as canonical base62 strings
/// (see <see cref="Base62Id"/>). Deserialization is strict: anything that is
/// not the canonical encoding — including legacy dashed GUIDs — raises
/// <see cref="JsonException"/> and fails closed.
/// </summary>
public sealed class GuidBase62JsonConverter : JsonConverter<Guid>
{
    /// <inheritdoc />
    public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected a base62 string API identifier, got {reader.TokenType}.");
        }

        var value = reader.GetString();
        if (!Base62Id.TryDecode(value, out var id))
        {
            throw new JsonException($"Value '{value}' is not a canonical base62 API identifier; expected {Base62Id.Length} characters over the alphabet '{Base62Id.Alphabet}'.");
        }
        return id;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(Base62Id.Encode(value));
    }
}
