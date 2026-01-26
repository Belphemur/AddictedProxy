using Compressor.Utils;
using Microsoft.Extensions.FileProviders;
using ZstdSharp;

namespace Compressor.Factory.Impl;

public class ZstdSrtFileCompressor : ICompressorService, IDisposable
{
    private readonly byte[] _srtDictionary;
    public AlgorithmEnum Enum => AlgorithmEnum.ZstdSrtDict;
    public CompressorDefinition Definition { get; } = new("EF-2F-AA-A1-28-B5-2F-FD");

    public ZstdSrtFileCompressor()
    {
        var embeddedProvider = new EmbeddedFileProvider(typeof(ZstdSrtFileCompressor).Assembly);
        var fileInfo = embeddedProvider.GetFileInfo("Model/srt-dict");
        using var stream = fileInfo.CreateReadStream();
        _srtDictionary = new byte[stream.Length];
        _ = stream.Read(_srtDictionary, 0, _srtDictionary.Length);
    }

    public async Task<Stream> CompressAsync(Stream inputStream, CancellationToken cancellationToken)
    {
        var outputStream = new MemoryStream();
        {
            await outputStream.WriteAsync(Definition.MagicNumberByte.AsMemory()[..4], cancellationToken);
            await using var compressionStream = new CompressionStream(outputStream, 5);
            compressionStream.LoadDictionary(_srtDictionary);
            await inputStream.CopyToAsync(compressionStream, cancellationToken);
            await compressionStream.FlushAsync(cancellationToken);
        }
        outputStream.ResetPosition();
        return outputStream;
    }

    public Task<Stream> DecompressAsync(Stream inputStream, CancellationToken cancellationToken)
    {
        inputStream.Seek(4, SeekOrigin.Begin);
        var decompressionStream = new DecompressionStream(inputStream);
        decompressionStream.LoadDictionary(_srtDictionary);
        return Task.FromResult<Stream>(decompressionStream);
    }

    public void Dispose()
    {
    }
}