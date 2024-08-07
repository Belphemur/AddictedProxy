﻿#region

using AddictedProxy.Database.Model.Credentials;

#endregion

namespace AddictedProxy.Upstream.Service;

public class HttpUtils
{
    private enum Browser
    {
        Chrome,
        Edge,
        Firefox,
        Safari
    }

    private readonly Dictionary<Browser, string[]> _userAgents = new()
    {
        {
            Browser.Chrome, [
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/125.0.0.0 Safari/537.36",
                "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36",
                "Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36",
                "Mozilla/5.0 (Macintosh; Intel Mac OS X 12_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36",
                "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36",
                "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36",]
        },
        {
            Browser.Firefox, [
                "Mozilla/5.0 (X11; Linux x86_64; rv:102.0) Gecko/20100101 Firefox/102.0",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:102.0) Gecko/20100101 Firefox/102.0",
                "Mozilla/5.0 (Macintosh; Intel Mac OS X 12.4; rv:102.0) Gecko/20100101 Firefox/102.0",
                "Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:102.0) Gecko/20100101 Firefox/102.0",
                "Mozilla/5.0 (X11; Fedora; Linux x86_64; rv:102.0) Gecko/20100101 Firefox/102.0",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:125.0) Gecko/20100101 Firefox/125.0"
            ]
        },
        {
            Browser.Edge, [
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36 Edg/126.0.1264.49",
                "Mozilla/5.0 (Macintosh; Intel Mac OS X 12_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36 Edg/126.0.1264.51",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/125.0.0.0 Safari/537.36 Edg/125.0.1418.56"
            ]
        },
        {
            Browser.Safari,
            [
                "Mozilla/5.0 (Macintosh; Intel Mac OS X 12_4) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/15.4 Safari/605.1.15",
                "Mozilla/5.0 (iPhone; CPU iPhone OS 15_5 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/15.4 Mobile/15E148 Safari/604.1",
                "Mozilla/5.0 (iPad; CPU OS 15_5 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/15.4 Mobile/15E148 Safari/604.1"
            ]
        }
    };

    private string GetUserAgent()
    {
        var random = new Random();
        var percentage = random.Next(0, 100);
        var browser = percentage switch
        {
            <= 70 => Browser.Chrome,
            <= 76 => Browser.Edge,
            <= 90 => Browser.Safari,
            _ => Browser.Firefox
        };
        var userAgents = _userAgents[browser];

        return userAgents[percentage % userAgents.Length];
    }

    public HttpRequestMessage PrepareRequest(AddictedUserCredentials? credentials, string url, HttpMethod method)
    {
        var userAgent = GetUserAgent();
        var request = new HttpRequestMessage(method, url)
        {
            Headers =
            {
                { "Referer", "https://www.addic7ed.com/" },
                { "User-Agent", userAgent },
            }
        };
        if (credentials != null)
        {
            request.Headers.Add("Cookie", credentials.Cookie);
        }

        return request;
    }
}