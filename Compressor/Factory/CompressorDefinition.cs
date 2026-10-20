namespace Compressor.Factory;

public class CompressorDefinition
{
    public ushort MagicNumberLength { get; }
    public byte[] MagicNumberByte { get; }
    public string MagicNumber { get; }
    public bool HasMagicNumber { get; }

    public CompressorDefinition(string magicNumber)
    {
        MagicNumber = magicNumber;
        MagicNumberByte = MagicNumber.Split('-').Select(x => Convert.ToByte(x, 16)).ToArray();
        MagicNumberLength = (ushort)MagicNumberByte.Length;
        HasMagicNumber = true;
    }

    internal CompressorDefinition()
    {
        MagicNumberByte = Array.Empty<byte>();
        MagicNumberLength = 0;
        MagicNumber = string.Empty;
        HasMagicNumber = false;
    }
}

public enum AlgorithmEnum
{
    Zstd,
    BrotliWithSignature,

    /// <summary>
    /// Legacy signature-less Brotli, kept for backward compatibility: used only to
    /// decompress streams stored before signatures were introduced and as the
    /// fallback when magic-number detection fails. Never use it to compress new data.
    /// </summary>
    // Not [Obsolete]: every remaining usage IS the backward-compatibility path it
    // describes, and BrotliWithSignature is not a drop-in replacement (its
    // decompressor skips the magic number, corrupting signature-less streams).
    BrotliDefault,
    ZstdSrtDict
}