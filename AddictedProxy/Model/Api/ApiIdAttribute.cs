using System.Text.Json.Serialization;

namespace AddictedProxy.Model.Api;

/// <summary>
/// Marks a property, field or route parameter as an external API identifier
/// in the canonical base62 encoding (see <see cref="Base62Id"/>) — a
/// semantic/documentation tag referenced by
/// <c>docs/2026-10-20-api-id-base62.md</c>.
///
/// On <see cref="Guid"/> DTO properties, pair it with the literal
/// <c>[JsonConverter(typeof(GuidBase62JsonConverter))]</c> attribute: a
/// pure marker cannot also derive from <see cref="JsonConverterAttribute"/>
/// (a member may carry only one such attribute), and the literal form is
/// required so the source-generated serialization context resolves the
/// converter. Route binding is handled by the <c>apiId</c> route parameter
/// policy, not by this attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.Field)]
public sealed class ApiIdAttribute : Attribute
{
    /// <summary>Tag the member as an external API identifier encoded in base62.</summary>
    public ApiIdAttribute()
    {
    }
}
