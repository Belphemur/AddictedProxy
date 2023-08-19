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
    BrotliDefault,
    BrotliWithSignature,
    Zstd
}