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
    /// </summary>
    public string[] AllowedOrigins { get; set; } =
    [
        "https://gestdown.info",
        "https://*.gestdown.info",
        "https://addictedproxy.pages.dev",
        "https://*.addictedproxy.pages.dev",
        "https://subvault.tv",
        "https://*.subvault.tv"
    ];
}
