namespace Compressor.Factory;

public record CompressorType(string MagicNumber, ushort MagicNumberLength = 4)
{
    public byte[] MagicNumberByte => MagicNumber.Split('-').Select(x => Convert.ToByte(x, 16)).ToArray();
}

public static class CompressorTypes
{
    public static readonly CompressorType BrotliDefault = new("");
    public static readonly CompressorType BrotliWithSignature = new("CE-B2-CF-81");
    public static readonly CompressorType Zstd = new("28-B5-2F-FD");
}