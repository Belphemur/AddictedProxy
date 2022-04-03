#region

using System.Net;

#endregion

namespace AddictedProxy.Services.Addic7ed.EnvVar.Http;

public record HttpProxy(Uri Address, NetworkCredential Credentials);