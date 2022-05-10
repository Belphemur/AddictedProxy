using System.Text.Json.Serialization;
using AddictedProxy.Model.Responses;

namespace AddictedProxy.Controllers.Serializer;
[JsonSerializable(typeof(TvShows.ShowSearchResponse))]
[JsonSerializable(typeof(TvShowSubtitleResponse))]
[JsonSerializable(typeof(SubtitleSearchResponse))]
internal partial class SerializationContext : JsonSerializerContext
{
    
}