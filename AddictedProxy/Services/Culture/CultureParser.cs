using System.Globalization;

namespace AddictedProxy.Services.Culture;

public class CultureParser
{
    /// <summary>
    ///     Try to find what is the culture associated with the string
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public CultureInfo? FromString(string name)
    {
        var nameLc = name.ToLowerInvariant();

        return CultureInfo.GetCultures(CultureTypes.AllCultures)
                          .FirstOrDefault(info => info.Name.ToLowerInvariant() == nameLc
                                                  || info.DisplayName.ToLowerInvariant() == nameLc
                                                  || info.EnglishName.ToLowerInvariant() == nameLc
                                                  || info.ThreeLetterISOLanguageName.ToLowerInvariant() == nameLc
                                                  || info.TwoLetterISOLanguageName.ToLowerInvariant() == nameLc);
    }
}