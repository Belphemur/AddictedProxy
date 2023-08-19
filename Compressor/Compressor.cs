using Compressor.Factory;

namespace Compressor;

public class Compressor : ICompressor
{
    private readonly CompressorFactory _factory;

    public Compressor(CompressorFactory factory)
    {
        _factory = factory;
    }
    
    [Obsolete]
    public string GetFileName(string file)
    {
        //For backward compatibility, in the past I only had the brotli compressor.
        //this is to be sure I didn't need to go an rename the file in S3, technically, this extension doesn't represent anymore the real algorithm used.
        return $"{file}.brotli";
    }

    public Task<Stream> CompressAsync(Stream inputStream, CancellationToken cancellationToken)
    {
        return _factory.GetService(CompressorTypes.Zstd).CompressAsync(inputStream, cancellationToken);
    }

    public async Task<Stream> DecompressAsync(Stream inputStream, CancellationToken cancellationToken)
    {
        using var memoryStream = new MemoryStream();
        await inputStream.CopyToAsync(memoryStream, cancellationToken);
        
        return await (await _factory.GetServiceByMagicNumberAsync(memoryStream, cancellationToken)).DecompressAsync(memoryStream, cancellationToken);
    }
}