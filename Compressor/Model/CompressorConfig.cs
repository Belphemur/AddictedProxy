using Compressor.Factory;

namespace Compressor.Model;

public class CompressorConfig
{
    /// <summary>
    /// What algorithm to use for compression
    /// </summary>
    public AlgorithmEnum Algorithm { get; set; } = AlgorithmEnum.ZstdSrtDict;
}