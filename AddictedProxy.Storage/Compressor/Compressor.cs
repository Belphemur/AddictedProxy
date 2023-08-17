using AddictedProxy.Storage.Compressor.Factory;
using InversionOfControl.Model.Factory;

namespace AddictedProxy.Storage.Compressor;

public class Compressor : ICompressor
{
    private readonly EnumFactory<CompressorType, ICompressorService> _factory;

    public Compressor(EnumFactory<CompressorType, ICompressorService> factory)
    {
        _factory = factory;
    }
    
    public string GetFileName(string file)
    {
        return _factory.GetService(CompressorTypes.Brotli).GetFileName(file);
    }

    public Task<Stream> CompressAsync(Stream inputStream, CancellationToken cancellationToken)
    {
        return _factory.GetService(CompressorTypes.Brotli).CompressAsync(inputStream, cancellationToken);
    }

    public Task<Stream> DecompressAsync(Stream inputStream, CancellationToken cancellationToken)
    {
        return _factory.GetService(CompressorTypes.Brotli).DecompressAsync(inputStream, cancellationToken);
    }
}