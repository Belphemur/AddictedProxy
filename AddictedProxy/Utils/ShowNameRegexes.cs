using System.Text.RegularExpressions;

namespace AddictedProxy.Utils;

public static partial class ShowNameRegexes
{
    [GeneratedRegex(@"\((\d{4})\)", RegexOptions.Compiled)]
    public static partial Regex ReleaseYearRegex();

    [GeneratedRegex(@"\(([A-Z]{2,3})\)", RegexOptions.Compiled)]
    public static partial Regex CountryRegex();
}