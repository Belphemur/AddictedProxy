#region

using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AddictedProxy.Storage.Compressor;
using NUnit.Framework;

#endregion

namespace AddictedProxy.Storage.Tests;

public class CompressorTests
{
    private readonly BrotliCompressor _compressor;

    public CompressorTests()
    {
        _compressor = new BrotliCompressor();
    }

    [Test]
    public async Task CompressDecompress()
    {
        const string text = "Hello World World World World World";
        var buffer = Encoding.UTF8.GetBytes(text);
        await using var memory = new MemoryStream(buffer);
        await using var compressed = await _compressor.CompressAsync(memory, CancellationToken.None);

        await using var decompressed = await _compressor.DecompressAsync(compressed, CancellationToken.None);
        await using var memoryResult = new MemoryStream();
        await decompressed.CopyToAsync(memoryResult);

        var decompressedBytes = memoryResult.ToArray();
        Assert.AreEqual(text, Encoding.UTF8.GetString(decompressedBytes));
    }
}