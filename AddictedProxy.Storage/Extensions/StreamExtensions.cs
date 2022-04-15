namespace AddictedProxy.Storage.Extensions;

public static class StreamExtensions
{
    /// <summary>
    /// Reset the position of the stream
    /// </summary>
    /// <param name="stream"></param>
    public static void ResetPosition(this Stream stream)
    {
        stream.Position = 0;
    }
}