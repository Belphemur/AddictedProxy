namespace AddictedProxy.Storage.Compressor.Factory;

public record CompressorType(string MagicNumber, ushort MagicNumberLength = 4);

public static class CompressorTypes
{
    public static readonly CompressorType Brotli = new("CE-B2-2F-FD");
    public static readonly CompressorType Zstd = new("28-B5-2F-FD");

}