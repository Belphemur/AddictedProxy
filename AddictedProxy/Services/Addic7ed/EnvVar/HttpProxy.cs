using System.Net;

namespace AddictedProxy.Services.Addic7ed.EnvVar;

public record HttpProxy(Uri Address, NetworkCredential Credentials);