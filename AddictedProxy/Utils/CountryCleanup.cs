namespace AddictedProxy.Utils;

public static class CountryCleanup
{
    /// <summary>
    /// Clean a country for Addicted and Tmdb
    /// </summary>
    /// <param name="country"></param>
    /// <returns></returns>
    public static string AddictedCountryToTmdb(string country) => country switch
    {
        "UK"  => "GB",
        "USA" => "US",
        _     => country
    };
}