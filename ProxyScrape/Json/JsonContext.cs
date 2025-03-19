using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using ProxyScrape.Model;

namespace ProxyScrape.Json;

[JsonSerializable(typeof(AuthResponse))]
[JsonSerializable(typeof(ProxyStatistics))]
internal partial class JsonContext : JsonSerializerContext
{
    internal static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web)
    {
        TypeInfoResolver = JsonTypeInfoResolver.Combine(Default, new DefaultJsonTypeInfoResolver())
    };
}