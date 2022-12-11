#region

using System.Collections.Concurrent;
using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

#endregion

namespace AddictedProxy.Culture.Service;

public class CultureParser
{
    private readonly IDistributedCache _cache;
    private readonly ConcurrentDictionary<string, Model.Culture> _localCache = new();

    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web)
    {
        TypeInfoResolver = JsonContext.Default
    };

    public CultureParser(IDistributedCache cache)
    {
        _cache = cache;
    }

    /// <summary>
    ///     Try to find what is the culture associated with the string
    /// </summary>
    /// <param name="name"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<Model.Culture?> FromStringAsync(string name, CancellationToken token)
    {
        var nameLc = name.ToLower();
        var cacheKey = $"Culture.v2.{nameLc}";

        if (_localCache.TryGetValue(cacheKey, out var localCulture))
        {
            return localCulture;
        }

        var cachedCultureJson = await _cache.GetStringAsync(cacheKey, token: token);
        if (cachedCultureJson != null)
        {
            var cached = JsonSerializer.Deserialize<Model.Culture>(cachedCultureJson, JsonSerializerOptions)!;
            _localCache.TryAdd(cacheKey, cached);
            return cached;
        }

        var cultureInfo = nameLc switch
        {
            "portuguese (brazilian)" => CultureInfo.GetCultureInfo("pt-br"),
            "norwegian"              => CultureInfo.GetCultureInfo("no"),
            "french (canadian)"      => CultureInfo.GetCultureInfo("fr-ca"),
            "galego"                 => CultureInfo.GetCultureInfo("gl"),
            "tagalog"                => CultureInfo.GetCultureInfo("tl"),
            "cantonese"              => CultureInfo.GetCultureInfo("yue"),
            "euskera"                => CultureInfo.GetCultureInfo("eus"),
            "bengali"                => CultureInfo.GetCultureInfo("bn"),
            "català"                 => CultureInfo.GetCultureInfo("cat"),
            "klingon"                => CultureInfo.GetCultureInfo("tlh"),
            _ => CultureInfo.GetCultures(CultureTypes.AllCultures)
                            .FirstOrDefault(info => info.Name.ToLower() == nameLc
                                                    || info.DisplayName.ToLower() == nameLc
                                                    || info.EnglishName.ToLower() == nameLc
                                                    || info.ThreeLetterISOLanguageName.ToLower() == nameLc || info.TwoLetterISOLanguageName.ToLower() == nameLc)
        };

        Model.Culture? culture = null;
        if (cultureInfo != null)
        {
            culture = new Model.Culture(cultureInfo.EnglishName, cultureInfo.TwoLetterISOLanguageName, cultureInfo.Name);
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(culture, JsonSerializerOptions), new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromDays(1)
            }, token);
            _localCache.TryAdd(cacheKey, culture);
        }

        return culture;
    }
}