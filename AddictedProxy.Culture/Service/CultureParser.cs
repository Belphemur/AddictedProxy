#region

using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

#endregion

namespace AddictedProxy.Culture.Service;

public class CultureParser
{
    private readonly IDistributedCache _cache;

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
        var cacheKey = $"Culture.{nameLc}";
        var cachedCultureJson = await _cache.GetStringAsync(cacheKey, token: token);
        if (cachedCultureJson != null)
        {
            return JsonSerializer.Deserialize<Model.Culture>(cachedCultureJson, JsonSerializerOptions);
        }

        var cultureInfo = CultureInfo.GetCultures(CultureTypes.AllCultures)
                                     .FirstOrDefault(info => info.Name.ToLower() == nameLc
                                                             || info.DisplayName.ToLower() == nameLc
                                                             || info.EnglishName.ToLower() == nameLc
                                                             || info.ThreeLetterISOLanguageName.ToLower() == nameLc || info.TwoLetterISOLanguageName.ToLower() == nameLc);
        Model.Culture? culture = null;
        if (cultureInfo != null)
        {
            culture = new Model.Culture(cultureInfo.EnglishName, cultureInfo.ThreeLetterISOLanguageName, cultureInfo.TwoLetterISOLanguageName);
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(culture, JsonSerializerOptions), new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromDays(1)
            }, token);
        }

        return culture;
    }
}