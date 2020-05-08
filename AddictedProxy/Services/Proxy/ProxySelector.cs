using System.Net.Http;

namespace AddictedProxy.Services.Proxy
{
    public class ProxySelector
    {
        private readonly HttpClient _httpClient;

        public ProxySelector(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
    }
}