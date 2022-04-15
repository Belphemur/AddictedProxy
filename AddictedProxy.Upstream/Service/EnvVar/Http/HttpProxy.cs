#region

using System.Net;

#endregion

namespace AddictedProxy.Upstream.Service.EnvVar.Http;

public record HttpProxy(Uri Address, NetworkCredential Credentials);