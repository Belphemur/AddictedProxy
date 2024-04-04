namespace AddictedProxy.Culture.Extensions;

using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

public static partial class StringExtensions
{
    /// <summary>
    /// Generate url safe slug for a specific string
    /// </summary>
    /// <param name="phrase"></param>
    /// <returns></returns>
    public static string ToSlug(this string phrase)
    {
        var str = phrase.RemoveAccent().ToLower();
        // Remove invalid characters
        str = NonTextRegex().Replace(str, "");
        // Trim leading/trailing spaces and convert multiple spaces to single hyphen
        str = SpaceRegex().Replace(str, "-").Trim('-');
        // Truncate slug to 50 characters
        str = str[..(str.Length <= 50 ? str.Length : 50)];
        // Trim trailing hyphens
        str = str.Trim('-');

        return str;
    }

    private static string RemoveAccent(this string text)
    {
        var sb = new StringBuilder();
        foreach (var c in text.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark))
        {
            sb.Append(c);
        }

        return sb.ToString().Normalize(NormalizationForm.FormD);
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex SpaceRegex();
    [GeneratedRegex(@"[^a-z0-9\s-]")]
    private static partial Regex NonTextRegex();
}