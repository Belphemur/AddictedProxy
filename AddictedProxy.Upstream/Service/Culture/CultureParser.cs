#region

using System.Globalization;
using AngleSharp.Common;

#endregion

namespace AddictedProxy.Upstream.Service.Culture;

public class CultureParser
{
    private readonly Dictionary<string, CultureInfo> _cache = new();

    /// <summary>
    ///     Try to find what is the culture associated with the string
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public CultureInfo? FromString(string name)
    {
        var nameLc = name.ToLower();
        var cultureInfo = _cache.GetOrDefault(nameLc, null);
        if (cultureInfo != null)
        {
            return cultureInfo;
        }

        cultureInfo = CultureInfo.GetCultures(CultureTypes.AllCultures)
                                 .FirstOrDefault(info => info.Name.ToLower() == nameLc
                                                         || info.DisplayName.ToLower() == nameLc
                                                         || info.EnglishName.ToLower() == nameLc
                                                         || info.ThreeLetterISOLanguageName.ToLower() == nameLc
                                                         || info.TwoLetterISOLanguageName.ToLower() == nameLc);
        if (cultureInfo != null)
        {
            _cache.Add(nameLc, cultureInfo);
        }

        return cultureInfo;
    }
}