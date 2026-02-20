using AddictedProxy.Database.Model.Shows;
using SuperSubtitleClient.Generated;

namespace AddictedProxy.Services.Provider.SuperSubtitles;

/// <summary>
/// Extension/mapping helpers for converting SuperSubtitles protobuf types to domain types.
/// </summary>
internal static class ProtoMappingExtensions
{
    /// <summary>
    /// Converts a collection of protobuf <see cref="Quality"/> values to a <see cref="VideoQuality"/> bitmask.
    /// </summary>
    public static VideoQuality ToVideoQuality(this IEnumerable<Quality> protoQualities)
    {
        var result = VideoQuality.None;
        foreach (var q in protoQualities)
        {
            result |= q switch
            {
                Quality._360P  => VideoQuality.Q360P,
                Quality._480P  => VideoQuality.Q480P,
                Quality._720P  => VideoQuality.Q720P,
                Quality._1080P => VideoQuality.Q1080P,
                Quality._2160P => VideoQuality.Q2160P,
                _              => VideoQuality.None,
            };
        }
        return result;
    }
}
