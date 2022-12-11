namespace AddictedProxy.Culture.Model;

// ReSharper disable once InconsistentNaming
public record Culture(string EnglishName, string TwoLetterISOLanguageName, string Name)
{
    public override string ToString()
    {
        return EnglishName;
    }
};