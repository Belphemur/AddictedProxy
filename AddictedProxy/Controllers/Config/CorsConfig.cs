namespace AddictedProxy.Controllers.Config;

/// <summary>
/// Configuration for the CORS policy applied to the API.
/// Bound from the "Cors" configuration section.
/// </summary>
public class CorsConfig
{
    public const string SectionName = "Cors";

    /// <summary>
    /// Origins allowed to call the API. Entries may use a wildcard subdomain
    /// (e.g. "https://*.example.com"); the scheme is part of the origin.
    /// Bound from the "Cors:AllowedOrigins" configuration section; falls back
    /// to an empty list (deny-all) when unconfigured so the API fails closed
    /// rather than reflecting an unintended origin.
    /// </summary>
    public string[]? AllowedOrigins { get; set; }
}
