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
    private readonly ISeasonPackSubtitleRepository _seasonPackSubtitleRepository;
    private readonly ICachedStorageProvider _cachedStorageProvider;
    private readonly ISuperSubtitlesClient _superSubtitlesClient;
    private readonly ISeasonPackEntryRepository _entryRepository;
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly ILogger<SeasonPackProvider> _logger;

    public SeasonPackProvider(
        ISeasonPackSubtitleRepository seasonPackSubtitleRepository,
        ICachedStorageProvider cachedStorageProvider,
        ISuperSubtitlesClient superSubtitlesClient,
        ISeasonPackEntryRepository entryRepository,
        IBackgroundJobClient backgroundJobClient,
        ILogger<SeasonPackProvider> logger)
    {
        _seasonPackSubtitleRepository = seasonPackSubtitleRepository;
        _cachedStorageProvider = cachedStorageProvider;
        _superSubtitlesClient = superSubtitlesClient;
        _entryRepository = entryRepository;
        _backgroundJobClient = backgroundJobClient;
        _logger = logger;
    }

    public Task<SeasonPackSubtitle?> GetByUniqueIdAsync(Guid uniqueId, CancellationToken token)
    {
        return _seasonPackSubtitleRepository.GetByUniqueIdAsync(uniqueId, token);
    }

    public Task<SeasonPackEntry?> GetEntryByUniqueIdAsync(Guid uniqueId, CancellationToken token)
    {
        return _entryRepository.FindByUniqueIdAsync(uniqueId, token);
    }

    public async Task<Stream> GetSeasonPackZipAsync(SeasonPackSubtitle seasonPack, CancellationToken token)
    {
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

        return await DownloadAndStoreFullZipAsync(seasonPack, token);
    }

    public async Task<Stream> GetEntryFileAsync(SeasonPackSubtitle seasonPack, SeasonPackEntry entry, CancellationToken token)
    {
        if (seasonPack.StoragePath == null)
        {
            _logger.LogWarning("Season pack {SeasonPackId} has no storage path, falling back to upstream for episode {Episode}", seasonPack.Id, entry.EpisodeNumber);
            return await DownloadFromUpstreamAsync(seasonPack, entry.EpisodeNumber, token);
        }

        var zipStream = await _cachedStorageProvider.GetSertAsync("season-pack", seasonPack.StoragePath, token);
        if (zipStream == null)
        {
            _logger.LogWarning("Couldn't find season pack with path [{path}] in storage for self-extraction, falling back to upstream", seasonPack.StoragePath);
            return await DownloadFromUpstreamAsync(seasonPack, entry.EpisodeNumber, token);
        }

        await using (zipStream)
        {
            using var archive = new ZipArchive(zipStream, ZipArchiveMode.Read, leaveOpen: true);
            var zipEntry = archive.GetEntry(entry.FileName);
            if (zipEntry == null)
            {
                _logger.LogWarning("Catalog entry {FileName} not found in ZIP for season pack {SeasonPackId}, falling back to upstream", entry.FileName, seasonPack.Id);
                return await DownloadFromUpstreamAsync(seasonPack, entry.EpisodeNumber, token);
            }

            var result = new MemoryStream();
            await using var entryStream = zipEntry.Open();
            await entryStream.CopyToAsync(result, token);
            result.Position = 0;

            await _seasonPackSubtitleRepository.IncrementDownloadCountAsync(seasonPack, token);
            return result;
        }
    }

    public async Task<Stream> GetEpisodeFromUpstreamAsync(SeasonPackSubtitle seasonPack, int episode, CancellationToken token)
    {
        if (seasonPack.RangeStart.HasValue && episode < seasonPack.RangeStart.Value)
        {
            throw new EpisodeNotInSeasonPackException(episode, $"Episode {episode} is before season pack range start ({seasonPack.RangeStart.Value})");
        }

        if (seasonPack.RangeEnd.HasValue && episode > seasonPack.RangeEnd.Value)
        {
            throw new EpisodeNotInSeasonPackException(episode, $"Episode {episode} is after season pack range end ({seasonPack.RangeEnd.Value})");
        }

        return await DownloadFromUpstreamAsync(seasonPack, episode, token);
    }

    private async Task<Stream> DownloadFromUpstreamAsync(SeasonPackSubtitle seasonPack, int episode, CancellationToken token)
    {
        try
        {
            var response = await _superSubtitlesClient.DownloadSubtitleAsync(seasonPack.ExternalId.ToString(), episode: episode, cancellationToken: token);
            await _seasonPackSubtitleRepository.IncrementDownloadCountAsync(seasonPack, token);

            return new MemoryStream(response.Content.ToByteArray());
        }
        catch (RpcException e) when (e.StatusCode is StatusCode.NotFound or StatusCode.FailedPrecondition)
        {
            throw new EpisodeNotInSeasonPackException(episode, e.Status.Detail);
        }
        finally
        {
            if(seasonPack.StoragePath == null)
            {
                _backgroundJobClient.Enqueue<StoreSeasonPackJob>(job => job.DownloadAndStoreAsync(seasonPack.UniqueId, null!, default));
            }
        }
    }

    private async Task<Stream> DownloadAndStoreFullZipAsync(SeasonPackSubtitle seasonPack, CancellationToken token)
    {
        var response = await _superSubtitlesClient.DownloadSubtitleAsync(seasonPack.ExternalId.ToString(), episode: null, cancellationToken: token);
        var blob = response.Content.ToByteArray();

        _backgroundJobClient.Enqueue<StoreSeasonPackJob>(job => job.StoreAsync(seasonPack.UniqueId, blob, null, default));

        await _seasonPackSubtitleRepository.IncrementDownloadCountAsync(seasonPack, token);
        return new MemoryStream(blob);
    }
}
