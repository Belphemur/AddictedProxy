using Compressor.Factory;
using Compressor.Model;
using Microsoft.Extensions.Options;

namespace Compressor;

public class Compressor : ICompressor
{
    private readonly CompressorFactory _factory;
    private readonly IOptions<CompressorConfig> _config;

    public Compressor(CompressorFactory factory, IOptions<CompressorConfig> config)
    {
        _factory = factory;
        _config = config;
    }
    
    public string GetFileName(string file)
    {
        //For backward compatibility, in the past I only had the brotli compressor.
        //this is to be sure I didn't need to go an rename the file in S3, technically, this extension doesn't represent anymore the real algorithm used.
        return $"{file}.brotli";
    }

    public Task<Stream> CompressAsync(Stream inputStream, CancellationToken cancellationToken)
    {
        return _factory.GetService(_config.Value.Algorithm).CompressAsync(inputStream, cancellationToken);
    }

    public async Task<Stream> DecompressAsync(Stream inputStream, CancellationToken cancellationToken)
    {
        var memoryStream = new MemoryStream();
        await inputStream.CopyToAsync(memoryStream, cancellationToken);
        try
        {
            return await (await _factory.GetServiceByMagicNumberAsync(memoryStream, cancellationToken)).DecompressAsync(memoryStream, cancellationToken);
        }
        catch (ArgumentOutOfRangeException)
        {
            return await _factory.GetService(AlgorithmEnum.BrotliDefault).DecompressAsync(memoryStream, cancellationToken);
        }
    }
}