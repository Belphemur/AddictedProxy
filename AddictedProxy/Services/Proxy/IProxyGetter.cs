using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AddictedProxy.Services.Proxy
{
    public interface IProxyGetter
    {
        /// <summary>
        /// Get list of web proxies
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEnumerable<WebProxy>> GetWebProxiesAsync(CancellationToken cancellationToken);
    }
}