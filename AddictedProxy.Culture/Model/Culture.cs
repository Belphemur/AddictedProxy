namespace AddictedProxy.Culture.Model;

// ReSharper disable once InconsistentNaming
public record Culture(string EnglishName, string ThreeLetterISOLanguageName, string TwoLetterISOLanguageName)
{
    public override string ToString()
    {
        return EnglishName;
    }
};