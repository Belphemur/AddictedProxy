namespace AntiCaptcha.Model.Config;

public class AntiCaptchaConfig
{
    public string ClientKey { get; init; } = null!;
    
    public TimeSpan ScrapeInterval { get; init; } = TimeSpan.FromMinutes(2);
}