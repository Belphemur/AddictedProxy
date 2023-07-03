using Microsoft.Extensions.DependencyInjection;
using SitemapCore;
using SitemapCore.Shared;

namespace AddictedProxy.Sitemap.Logic;

internal class SitemapDynamicProvider : ISitemapProvider
{
    private readonly ILocationHelper _locationHelper;
    private readonly IServiceProvider _serviceProvider;

    public SitemapDynamicProvider(ILocationHelper locationHelper, IServiceProvider serviceProvider)
    {
        _locationHelper = locationHelper;
        _serviceProvider = serviceProvider;
    }

    public async Task<IEnumerable<SitemapCore.Sitemap>> InvokeAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var providers = scope.ServiceProvider.GetRequiredService<IEnumerable<ISitemapDynamicProvider>>();
        return await providers.ToAsyncEnumerable().SelectMany(provider => provider.GenerateAsync()).ToArrayAsync();
    }
}