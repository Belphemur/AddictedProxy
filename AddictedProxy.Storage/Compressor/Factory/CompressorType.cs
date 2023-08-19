namespace AddictedProxy.Storage.Compressor.Factory;

public record CompressorType(string Ext, string MagicNumber, ushort MagicNumberLength = 4);

public static class CompressorTypes
{
    public static CompressorType Brotli = new("brotli", "CE-B2-2F-FD");
}