using System.Text.Json.Serialization;
using AddictedProxy.Model.Dto;
using AddictedProxy.Model.Responses;

namespace AddictedProxy.Controllers.Rest.Serializer;
[JsonSerializable(typeof(TvShowsController.ShowSearchResponse))]
[JsonSerializable(typeof(TvShowSubtitleResponse))]
[JsonSerializable(typeof(SubtitleSearchResponse))]
[JsonSerializable(typeof(ErrorResponse))]
[JsonSerializable(typeof(TopShowDto[]))]
[JsonSerializable(typeof(ApplicationController.ApplicationInfoDto))]
[JsonSerializable(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails))]
internal partial class SerializationContext : JsonSerializerContext
{
    
}