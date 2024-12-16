using AntiCaptcha.Model.Task.Turnstile;

namespace ProxyScrape.Model;

public record struct LoginExtraData(string PhpSessionId, string UserAgent);