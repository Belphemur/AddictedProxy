using System.Text.Json.Serialization;
using AddictedProxy.Model.Responses;

namespace AddictedProxy.Controllers.Rest.Serializer;
[JsonSerializable(typeof(TvShowsController.ShowSearchResponse))]
[JsonSerializable(typeof(TvShowSubtitleResponse))]
[JsonSerializable(typeof(SubtitleSearchResponse))]
internal partial class SerializationContext : JsonSerializerContext
{
    
}