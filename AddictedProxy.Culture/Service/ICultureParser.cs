namespace AddictedProxy.Culture.Service;

public interface ICultureParser
{
    /// <summary>
    ///     Try to find what is the culture associated with the string
    /// </summary>
    /// <param name="name"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<Model.Culture?> FromStringAsync(string name, CancellationToken token);
}