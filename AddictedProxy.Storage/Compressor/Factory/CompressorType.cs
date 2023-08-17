namespace AddictedProxy.Storage.Compressor.Factory;

public record CompressorType(string Ext)
{
    /// <summary>
    /// Append extension to file
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public string AppendToFile(string file) => $"{file}.{Ext}";
}

public static class CompressorTypes
{
    public static CompressorType Brotli = new("brotli");
}