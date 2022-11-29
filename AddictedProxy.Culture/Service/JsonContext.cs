using System.Text.Json.Serialization;

namespace AddictedProxy.Culture.Service;

[JsonSerializable(typeof(Model.Culture))]
internal partial class JsonContext : JsonSerializerContext
{
    
}