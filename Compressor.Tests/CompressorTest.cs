using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Compressor;
using Compressor.Bootstrap;
using Compressor.Factory.Impl;
using InversionOfControl.Service.Bootstrap;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;

namespace AddictedProxy.Storage.Tests;

public class CompressorTest
{

    private ICompressor? _compressor;
    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddBootstrap(Substitute.For<IConfiguration>(), typeof(BootstrapCompressor).Assembly);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        _compressor = serviceProvider.GetRequiredService<ICompressor>();
    }
    [Test]
    public async Task CompressDecompressWithZstd()
    {
        const string text = "Hello World World World World World";
        var buffer = Encoding.UTF8.GetBytes(text);
        await using var memory = new MemoryStream(buffer);
        ZstdCompressor compressor = new();
        await using var compressed = await compressor.CompressAsync(memory, CancellationToken.None);

        await using var decompressed = await _compressor!.DecompressAsync(compressed, CancellationToken.None);
        await using var memoryResult = new MemoryStream();
        await decompressed.CopyToAsync(memoryResult);

        var decompressedBytes = memoryResult.ToArray();
        Assert.AreEqual(text, Encoding.UTF8.GetString(decompressedBytes));
    }
    
    [Test]
    public async Task CompressDecompressWithBrotli()
    {
        const string text = "Hello World World World World World";
        var buffer = Encoding.UTF8.GetBytes(text);
        await using var memory = new MemoryStream(buffer);
        BrotliSignedCompressor signedCompressor = new();
        await using var compressed = await signedCompressor.CompressAsync(memory, CancellationToken.None);

        await using var decompressed = await _compressor!.DecompressAsync(compressed, CancellationToken.None);
        await using var memoryResult = new MemoryStream();
        await decompressed.CopyToAsync(memoryResult);

        var decompressedBytes = memoryResult.ToArray();
        Assert.AreEqual(text, Encoding.UTF8.GetString(decompressedBytes));
    }
}