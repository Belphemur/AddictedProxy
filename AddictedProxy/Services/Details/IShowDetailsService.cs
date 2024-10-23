using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Model.Dto;

namespace AddictedProxy.Services.Details;

/// <summary>
/// Interface for the Show Details Service.
/// </summary>
public interface IShowDetailsService
{
    /// <summary>
    /// Retrieves the cached details of a TV show.
    /// </summary>
    /// <param name="show">The TV show for which details are to be retrieved.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the details of the TV show.</returns>
    Task<MediaDetailsDto.DetailsDto?> GetDetailsDtoCachedAsync(TvShow show, CancellationToken cancellationToken);

    /// <summary>
    /// Updates the path and vote details of a media details DTO.
    /// </summary>
    /// <param name="detailsDto">The media details DTO to be updated.</param>
    /// <returns>The updated media details DTO.</returns>
    MediaDetailsDto.DetailsDto? UpdatePathAndVoteDetailsDto(MediaDetailsDto.DetailsDto? detailsDto);
}