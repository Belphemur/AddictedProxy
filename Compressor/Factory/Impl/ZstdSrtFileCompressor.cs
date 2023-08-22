using System.IO.Compression;
using System.Reflection;
using Compressor.Utils;
using Microsoft.Extensions.FileProviders;
using ZstdNet;

namespace Compressor.Factory.Impl;

public class ZstdSrtFileCompressor : ICompressorService, IDisposable
{
    private readonly CompressionOptions _compressionOptions;
    private readonly DecompressionOptions _decompressionOptions;
    public AlgorithmEnum Enum => AlgorithmEnum.ZstdSrtDict;
    public CompressorDefinition Definition { get; } = new("EF-2F-AA-A1-28-B5-2F-FD");

    public ZstdSrtFileCompressor()
    {
        var embeddedProvider = new EmbeddedFileProvider(typeof(ZstdSrtFileCompressor).Assembly);
        var fileInfo = embeddedProvider.GetFileInfo("Model\\srt-dict");
        using var stream = fileInfo.CreateReadStream();
        var srtDictionary = new byte[stream.Length];
        _ = stream.Read(srtDictionary, 0, srtDictionary.Length);
        _compressionOptions = new CompressionOptions(srtDictionary, 5);
        _decompressionOptions = new DecompressionOptions(srtDictionary);
    }

    public async Task<Stream> CompressAsync(Stream inputStream, CancellationToken cancellationToken)
    {
        var outputStream = new MemoryStream();
        {
            await outputStream.WriteAsync(Definition.MagicNumberByte[..4], cancellationToken);
            await using var compressionStream = new CompressionStream(outputStream, _compressionOptions);
            await inputStream.CopyToAsync(compressionStream, cancellationToken);
            await compressionStream.FlushAsync(cancellationToken);
        }
        outputStream.ResetPosition();
        return outputStream;
    }

    public Task<Stream> DecompressAsync(Stream inputStream, CancellationToken cancellationToken)
    {
        inputStream.Seek(4, SeekOrigin.Begin);
        var decompressionStream = new DecompressionStream(inputStream, _decompressionOptions);
        return Task.FromResult<Stream>(decompressionStream);
    }

    public void Dispose()
    {
        _compressionOptions.Dispose();
        _decompressionOptions.Dispose();
    }
}