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
    private readonly ILogger<SeasonPackProvider> _logger;

    public SeasonPackProvider(
        ISeasonPackSubtitleRepository seasonPackSubtitleRepository,
        ICachedStorageProvider cachedStorageProvider,
        ISuperSubtitlesClient superSubtitlesClient,
        ILogger<SeasonPackProvider> logger)
    {
        _seasonPackSubtitleRepository = seasonPackSubtitleRepository;
        _cachedStorageProvider = cachedStorageProvider;
        _superSubtitlesClient = superSubtitlesClient;
        _logger = logger;
    }

    public Task<SeasonPackSubtitle?> GetByUniqueIdAsync(Guid uniqueId, CancellationToken token)
    {
        return _seasonPackSubtitleRepository.GetByUniqueIdAsync(uniqueId, token);
    }

    public async Task<Stream> GetSeasonPackFileAsync(SeasonPackSubtitle seasonPack, int? episode, CancellationToken token)
    {
        // For episode extraction, always download from upstream (no caching of individual episode SRTs)
        if (episode.HasValue)
        {
            return await DownloadFromUpstreamAsync(seasonPack, episode.Value, token);
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

    private async Task<Stream> DownloadFromUpstreamAsync(SeasonPackSubtitle seasonPack, int episode, CancellationToken token)
    {
        try
        {
            var response = await _superSubtitlesClient.DownloadSubtitleAsync(seasonPack.ExternalId.ToString(), episode: episode, cancellationToken: token);
            await _seasonPackSubtitleRepository.IncrementDownloadCountAsync(seasonPack, token);
            return new MemoryStream(response.Content.ToByteArray());
        }
        catch (RpcException e) when (e.StatusCode == StatusCode.Internal && e.Status.Detail.Contains(EpisodeNotFoundInZipDetail))
        {
            throw new EpisodeNotInSeasonPackException(episode, e.Status.Detail);
        }
    }

    private async Task<Stream> DownloadAndStoreAsync(SeasonPackSubtitle seasonPack, CancellationToken token)
    {
        var response = await _superSubtitlesClient.DownloadSubtitleAsync(seasonPack.ExternalId.ToString(), episode: null, cancellationToken: token);
        var blob = response.Content.ToByteArray();

        BackgroundJob.Enqueue<StoreSeasonPackJob>(job => job.ExecuteAsync(seasonPack.UniqueId, blob, null, default));

        await _seasonPackSubtitleRepository.IncrementDownloadCountAsync(seasonPack, token);
        return new MemoryStream(blob);
    }
}
