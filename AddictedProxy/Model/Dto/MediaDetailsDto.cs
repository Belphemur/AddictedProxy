﻿namespace AddictedProxy.Model.Dto;

public record struct MediaDetailsDto(ShowDto Media, MediaDetailsDto.DetailsDto? Details)
{
    public enum Type
    {
        Show,
        Movie
    }
    public record DetailsDto(string PosterPath, string Overview, string OriginalName, Type Type);

    /// <summary>
    /// Show data
    /// </summary>
    public ShowDto Media { get; } = Media;

    /// <summary>
    /// Details of the show
    /// </summary>
    public DetailsDto? Details { get; } = Details;
}