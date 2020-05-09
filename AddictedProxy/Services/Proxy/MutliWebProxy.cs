using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace AddictedProxy.Services.Proxy
{
    public class MutliWebProxy : IWebProxy
    {
        private readonly Uri[] _proxies;

        private int _proxyCount;

        public MutliWebProxy(IEnumerable<WebProxy> proxies)
        {
            _proxies = proxies.Select(proxy => proxy.Address).ToArray();
        }


        public Uri GetProxy(Uri destination)
        {
            return _proxies[_proxyCount++ % _proxies.Length];
        }

        public bool IsBypassed(Uri host)
        {
            return false;
        }

        public ICredentials Credentials { get; set; }
    }
}