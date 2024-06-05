using System;
using System.IO;

namespace TvMovieDatabaseClient.Service.Model;

public record TmdbImageMetadata(string ImagePath, DateTime LastModified, long ContentLength, string? ContentType);

public record struct TmdbImage(Stream ImageStream, TmdbImageMetadata Metadata);