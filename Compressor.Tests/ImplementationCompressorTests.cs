#region

using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Compressor.Factory.Impl;
using FluentAssertions;
using NUnit.Framework;

#endregion

namespace AddictedProxy.Storage.Tests;

public class ImplementationCompressorTests
{
    [Test]
    public async Task CompressDecompressBrotliSigned()
    {
        const string text = "Hello World World World World World";
        var buffer = Encoding.UTF8.GetBytes(text);
        await using var memory = new MemoryStream(buffer);
        BrotliSignedCompressor compressor = new();
        await using var compressed = await compressor.CompressAsync(memory, CancellationToken.None);

        await using var decompressed = await compressor.DecompressAsync(compressed, CancellationToken.None);
        await using var memoryResult = new MemoryStream();
        await decompressed.CopyToAsync(memoryResult);

        var decompressedBytes = memoryResult.ToArray();
        Encoding.UTF8.GetString(decompressedBytes).Should().Be(text);
    }
    
    [Test]
    public async Task CompressDecompressZstd()
    {
        const string text = "Hello World World World World World";
        var buffer = Encoding.UTF8.GetBytes(text);
        await using var memory = new MemoryStream(buffer);
        ZstdCompressor compressor = new();
        await using var compressed = await compressor.CompressAsync(memory, CancellationToken.None);

        await using var decompressed = await compressor.DecompressAsync(compressed, CancellationToken.None);
        await using var memoryResult = new MemoryStream();
        await decompressed.CopyToAsync(memoryResult);

        var decompressedBytes = memoryResult.ToArray();
        Encoding.UTF8.GetString(decompressedBytes).Should().Be(text);
    }
    
    [Test]
    public async Task CompressDecompressBrotli()
    {
        const string text = "Hello World World World World World";
        var buffer = Encoding.UTF8.GetBytes(text);
        await using var memory = new MemoryStream(buffer);
        BrotliCompressor compressor = new();
        await using var compressed = await compressor.CompressAsync(memory, CancellationToken.None);

        await using var decompressed = await compressor.DecompressAsync(compressed, CancellationToken.None);
        await using var memoryResult = new MemoryStream();
        await decompressed.CopyToAsync(memoryResult);

        var decompressedBytes = memoryResult.ToArray();
        Encoding.UTF8.GetString(decompressedBytes).Should().Be(text);
    }
}