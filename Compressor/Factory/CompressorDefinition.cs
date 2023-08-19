namespace Compressor.Factory;

public class CompressorDefinition
{
    public ushort MagicNumberLength { get; }
    public byte[] MagicNumberByte { get; }
    public string MagicNumber { get; }

    public CompressorDefinition(string magicNumber)
    {
        MagicNumber = magicNumber;
        MagicNumberByte = MagicNumber.Split('-').Select(x => Convert.ToByte(x, 16)).ToArray();
        MagicNumberLength = (ushort)MagicNumberByte.Length;
    }
}

public enum AlgorithmEnum
{
    BrotliDefault,
    BrotliWithSignature,
    Zstd
}