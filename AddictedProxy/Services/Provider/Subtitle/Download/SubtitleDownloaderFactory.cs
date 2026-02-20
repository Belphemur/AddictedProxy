using AddictedProxy.Database.Model.Shows;
using InversionOfControl.Model.Factory;
using Microsoft.Extensions.DependencyInjection;

namespace AddictedProxy.Services.Provider.Subtitle.Download;

/// <summary>
/// Factory that resolves the correct <see cref="ISubtitleDownloader"/> for a given <see cref="DataSource"/>.
/// </summary>
[EnumServiceLifetime(ServiceLifetime.Scoped)]
public class SubtitleDownloaderFactory : EnumFactory<DataSource, ISubtitleDownloader>
{
    public SubtitleDownloaderFactory(IEnumerable<ISubtitleDownloader> services) : base(services)
    {
    }
}
