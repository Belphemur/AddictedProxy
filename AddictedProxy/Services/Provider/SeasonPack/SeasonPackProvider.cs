using System.IO.Compression;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Storage.Caching.Service;
using Grpc.Core;
using Hangfire;
using SuperSubtitleClient.Service;

namespace AddictedProxy.Services.Provider.SeasonPack;

public class SeasonPackProvider : ISeasonPackProvider
{
    public const string EpisodeNotFoundInZipDetail = "not found in season pack ZIP";
    private readonly ISeasonPackSubtitleRepository _seasonPackSubtitleRepository;
    private readonly ICachedStorageProvider _cachedStorageProvider;
    private readonly ISuperSubtitlesClient _superSubtitlesClient;
    private readonly ISeasonPackCatalogService _catalogService;
    private readonly ISeasonPackEntryRepository _entryRepository;
    private readonly ILogger<SeasonPackProvider> _logger;

    public SeasonPackProvider(
        ISeasonPackSubtitleRepository seasonPackSubtitleRepository,
        ICachedStorageProvider cachedStorageProvider,
        ISuperSubtitlesClient superSubtitlesClient,
        ISeasonPackCatalogService catalogService,
        ISeasonPackEntryRepository entryRepository,
        ILogger<SeasonPackProvider> logger)
    {
        _seasonPackSubtitleRepository = seasonPackSubtitleRepository;
        _cachedStorageProvider = cachedStorageProvider;
        _superSubtitlesClient = superSubtitlesClient;
        _catalogService = catalogService;
        _entryRepository = entryRepository;
        _logger = logger;
    }

    public Task<SeasonPackSubtitle?> GetByUniqueIdAsync(Guid uniqueId, CancellationToken token)
    {
        return _seasonPackSubtitleRepository.GetByUniqueIdAsync(uniqueId, token);
    }

    public async Task<Stream> GetSeasonPackFileAsync(SeasonPackSubtitle seasonPack, int? episode, CancellationToken token)
    {
        // For episode extraction, try self-extraction from stored ZIP first
        if (episode.HasValue)
        {
            return await GetEpisodeStreamAsync(seasonPack, episode.Value, token);
        }

        // For full ZIP, try cached storage first
        if (seasonPack.StoragePath != null)
        {
            var stream = await _cachedStorageProvider.GetSertAsync("season-pack", seasonPack.StoragePath, token);
            if (stream != null)
            {
                await _seasonPackSubtitleRepository.IncrementDownloadCountAsync(seasonPack, token);
                return stream;
            }

            _logger.LogWarning("Couldn't find season pack with path [{path}] in storage, downloading from upstream", seasonPack.StoragePath);
        }

        return await DownloadAndStoreAsync(seasonPack, token);
    }

    private async Task<Stream> GetEpisodeStreamAsync(SeasonPackSubtitle seasonPack, int episode, CancellationToken token)
    {
        // If pack is stored and cataloged, self-extract from S3 ZIP
        if (seasonPack.StoragePath != null && await _catalogService.IsCatalogedAsync(seasonPack.Id, token))
        {
            // Verify the episode exists in the catalog
            if (!await _entryRepository.HasEpisodeAsync(seasonPack.Id, episode, token))
            {
                throw new EpisodeNotInSeasonPackException(episode, $"Episode {episode} {EpisodeNotFoundInZipDetail}");
            }

            return await SelfExtractEpisodeAsync(seasonPack, episode, token);
        }

        // Fallback to upstream for uncataloged or unstored packs
        return await DownloadFromUpstreamAsync(seasonPack, episode, token);
    }

    private async Task<Stream> SelfExtractEpisodeAsync(SeasonPackSubtitle seasonPack, int episode, CancellationToken token)
    {
        var zipStream = await _cachedStorageProvider.GetSertAsync("season-pack", seasonPack.StoragePath!, token);
        if (zipStream == null)
        {
            _logger.LogWarning("Couldn't find season pack with path [{path}] in storage for self-extraction, falling back to upstream", seasonPack.StoragePath);
            return await DownloadFromUpstreamAsync(seasonPack, episode, token);
        }

        await using (zipStream)
        {
            using var archive = new ZipArchive(zipStream, ZipArchiveMode.Read);
            var entries = await _entryRepository.GetBySeasonPackAsync(seasonPack.Id, token);
            var matchingEntry = entries.FirstOrDefault(e => e.EpisodeNumber == episode);

            if (matchingEntry == null)
            {
                throw new EpisodeNotInSeasonPackException(episode, $"Episode {episode} {EpisodeNotFoundInZipDetail}");
            }

            var zipEntry = archive.GetEntry(matchingEntry.FileName);
            if (zipEntry == null)
            {
                _logger.LogWarning("Catalog entry {FileName} not found in ZIP for season pack {SeasonPackId}, falling back to upstream", matchingEntry.FileName, seasonPack.Id);
                return await DownloadFromUpstreamAsync(seasonPack, episode, token);
            }

            // Read the SRT into memory so the ZIP stream can be disposed
            var result = new MemoryStream();
            await using var entryStream = zipEntry.Open();
            await entryStream.CopyToAsync(result, token);
            result.Position = 0;

            await _seasonPackSubtitleRepository.IncrementDownloadCountAsync(seasonPack, token);
            return result;
        }
    }

    private async Task<Stream> DownloadFromUpstreamAsync(SeasonPackSubtitle seasonPack, int episode, CancellationToken token)
    {
        try
        {
            var response = await _superSubtitlesClient.DownloadSubtitleAsync(seasonPack.ExternalId.ToString(), episode: episode, cancellationToken: token);
            await _seasonPackSubtitleRepository.IncrementDownloadCountAsync(seasonPack, token);

            BackgroundJob.Enqueue<StoreSeasonPackJob>(job => job.DownloadAndStoreAsync(seasonPack.UniqueId, null!, default));

            return new MemoryStream(response.Content.ToByteArray());
        }
        catch (RpcException e) when ((e.StatusCode == StatusCode.Internal || e.StatusCode == StatusCode.NotFound) && e.Status.Detail.Contains(EpisodeNotFoundInZipDetail))
        {
            throw new EpisodeNotInSeasonPackException(episode, e.Status.Detail);
        }
    }

    private async Task<Stream> DownloadAndStoreAsync(SeasonPackSubtitle seasonPack, CancellationToken token)
    {
        var response = await _superSubtitlesClient.DownloadSubtitleAsync(seasonPack.ExternalId.ToString(), episode: null, cancellationToken: token);
        var blob = response.Content.ToByteArray();

        BackgroundJob.Enqueue<StoreSeasonPackJob>(job => job.StoreAsync(seasonPack.UniqueId, blob, null, default));

        await _seasonPackSubtitleRepository.IncrementDownloadCountAsync(seasonPack, token);
        return new MemoryStream(blob);
    }
}
