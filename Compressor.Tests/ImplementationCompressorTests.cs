#region

using System.Collections;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Compressor.Bootstrap;
using Compressor.Factory;
using FluentAssertions;
using InversionOfControl.Service.Bootstrap;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

#endregion

namespace AddictedProxy.Storage.Tests;

public class ImplementationCompressorTests
{
    public class CompressorProvider : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            var serviceCollection = new ServiceCollection();
            var hostedAppBuilder = Substitute.For<IHostApplicationBuilder>();
            hostedAppBuilder.Logging.Returns(Substitute.For<ILoggingBuilder>());
            hostedAppBuilder.Services.Returns(serviceCollection);
            hostedAppBuilder.Configuration.Returns(Substitute.For<IConfigurationManager>());
            hostedAppBuilder.AddBootstrap(typeof(BootstrapCompressor).Assembly);
            
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var factory = serviceProvider.GetRequiredService<CompressorFactory>();
            foreach (var service in factory.Services)
            {
                yield return new object[] { service };
            }
        }
    }

    [TestCaseSource(typeof(CompressorProvider))]
    public async Task CompressDecompressUsingImplementation(ICompressorService compressor)
    {
        const string text = "Hello World World World World World";
        var buffer = Encoding.UTF8.GetBytes(text);
        await using var memory = new MemoryStream(buffer);
        await using var compressed = await compressor.CompressAsync(memory, CancellationToken.None);

        await using var decompressed = await compressor.DecompressAsync(compressed, CancellationToken.None);
        await using var memoryResult = new MemoryStream();
        await decompressed.CopyToAsync(memoryResult);

        var decompressedBytes = memoryResult.ToArray();
        Encoding.UTF8.GetString(decompressedBytes).Should().Be(text);
    }
}