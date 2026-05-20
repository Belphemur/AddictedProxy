using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace AddictedProxy.Services.Tests.Database;

[TestFixture]
public class EpisodeRepositoryTests
{
    [Test]
    public void BuildPendingEpisodeExternalIdsForUpsert_DeduplicatesConflictingBatchExternalIds()
    {
        var logger = Substitute.For<ILogger<EpisodeRepository>>();
        var firstEpisode = new Episode
        {
            TvShowId = 42,
            Season = 1,
            Number = 1,
            Title = "Pilot",
            TvShow = new TvShow { Name = "Breaking Bad" },
            ExternalIds =
            [
                new EpisodeExternalId
                {
                    Source = DataSource.Addic7ed,
                    ExternalId = "198540"
                }
            ]
        };
        var duplicateEpisode = new Episode
        {
            TvShowId = 42,
            Season = 1,
            Number = 2,
            Title = "Cat's in the Bag...",
            TvShow = new TvShow { Name = "Breaking Bad" },
            ExternalIds =
            [
                new EpisodeExternalId
                {
                    Source = DataSource.Addic7ed,
                    ExternalId = "198540"
                }
            ]
        };
        var originalExternalIds = new Dictionary<Episode, EpisodeExternalId[]>
        {
            [firstEpisode] = firstEpisode.ExternalIds.ToArray(),
            [duplicateEpisode] = duplicateEpisode.ExternalIds.ToArray()
        };

        var result = EpisodeRepository.BuildPendingEpisodeExternalIdsForUpsert(
            [firstEpisode, duplicateEpisode],
            originalExternalIds,
            logger);

        result.Should().ContainSingle();
        result[0].EpisodeKey.Should().Be(new EpisodeRepository.EpisodeNaturalKey(42, 1, 1));
        result[0].ExternalId.Should().Be("198540");
        result[0].Source.Should().Be(DataSource.Addic7ed);

        logger.ReceivedCalls()
            .Select(call => call.GetArguments())
            .Any(arguments => arguments[0] is LogLevel.Warning &&
                              arguments[2]?.ToString()?.Contains("Skipping duplicate episode external ID in batch upsert") == true &&
                              arguments[2]?.ToString()?.Contains("198540") == true)
            .Should()
            .BeTrue();
    }

    [Test]
    public void BuildPendingEpisodeExternalIdsForUpsert_SkipsBlankExternalIds()
    {
        var logger = Substitute.For<ILogger<EpisodeRepository>>();
        var episode = new Episode
        {
            TvShowId = 42,
            Season = 1,
            Number = 1,
            Title = "Pilot",
            TvShow = new TvShow { Name = "Breaking Bad" },
            ExternalIds =
            [
                new EpisodeExternalId
                {
                    Source = DataSource.Addic7ed,
                    ExternalId = " "
                }
            ]
        };
        var originalExternalIds = new Dictionary<Episode, EpisodeExternalId[]>
        {
            [episode] = episode.ExternalIds.ToArray()
        };

        var result = EpisodeRepository.BuildPendingEpisodeExternalIdsForUpsert(
            [episode],
            originalExternalIds,
            logger);

        result.Should().BeEmpty();

        logger.ReceivedCalls()
            .Select(call => call.GetArguments())
            .Any(arguments => arguments[0] is LogLevel.Warning &&
                              arguments[2]?.ToString()?.Contains("Skipping episode external ID with empty value during batch upsert") == true)
            .Should()
            .BeTrue();
    }
}
