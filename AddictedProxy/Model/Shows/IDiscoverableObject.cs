namespace AddictedProxy.Model.Shows;

public interface IDiscoverableObject
{
    /// <summary>
    ///     When was the object discovered
    /// </summary>
    public DateTime Discovered { get; set; }
}