using ProxyProvider.Root.Service.Rotator;

namespace ProxyProvider.Root.Service.HttpHandler;

public class ProxyRotatorHttpHandler : HttpClientHandler
{
    private readonly IProxyRotator _proxyRotator;

    public ProxyRotatorHttpHandler(IProxyRotator proxyRotator)
    {
        _proxyRotator = proxyRotator;
        UseProxy = true;
    }
    
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Proxy = await _proxyRotator.GetNextProxyAsync(cancellationToken);
        return await base.SendAsync(request, cancellationToken);
    }
}