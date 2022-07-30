using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TvMovieDatabaseClient.Model;

public record Pagination<T>(
    [property: JsonPropertyName("page")] int Page,
    [property: JsonPropertyName("results")]
    IReadOnlyList<T> Results,
    [property: JsonPropertyName("total_pages")]
    int TotalPages,
    [property: JsonPropertyName("total_results")]
    int TotalResults
);