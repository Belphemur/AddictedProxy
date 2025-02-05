using System.Collections;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Compressor;
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

namespace AddictedProxy.Storage.Tests;



public class CompressorTest
{
    public class CompressorProvider : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            var hostedAppBuilder = Substitute.For<IHostApplicationBuilder>();
            hostedAppBuilder.Logging.Returns(Substitute.For<ILoggingBuilder>());
            hostedAppBuilder.Services.Returns(serviceCollection);
            hostedAppBuilder.Configuration.Returns(Substitute.For<IConfigurationManager>());
            hostedAppBuilder.AddBootstrap(typeof(BootstrapCompressor).Assembly);
            
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var factory = serviceProvider.GetRequiredService<CompressorFactory>();
            var compressor = serviceProvider.GetRequiredService<ICompressor>();
            foreach (var service in factory.Services)
            {
                yield return new object[] { service , compressor};
            }
        }
    }


    [TestCaseSource(typeof(CompressorProvider))]
    public async Task CompressAndDecompressUsingMainCompressor(ICompressorService compressorService, ICompressor compressor)
    {
        #region srt file

        const string text = @"1
                              00:00:10,510 --> 00:00:12,044
                              Quel genre de personne es-tu ?
                              
                              2
                              00:00:12,078 --> 00:00:13,879
                              Comment peux-tu 
                              vouloir le garder secret ?
                              
                              3
                              00:00:13,914 --> 00:00:15,714
                              Calme-toi ! 
                              Pense aux voisins !
                              
                              4
                              00:00:15,749 --> 00:00:17,650
                              Les voisins ?! Mais je me 
                              fous de ces saletés de voisins !
                              
                              5
                              00:00:17,684 --> 00:00:18,901
                              Je  ne peux pas te parler 
                              quand t'es comme ça !
                              
                              6
                              00:00:18,902 --> 00:00:20,119
                              Tu as perdu la tête !
                              
                              7
                              00:00:20,153 --> 00:00:21,854
                              Tu es un lâche ! Je vais 
                              le dire à tout le monde !
                              
                              8
                              00:00:21,888 --> 00:00:24,089
                              C'est arrivé ! 
                              Que veux-tu que j'y fasse ?!
                              
                              9
                              00:00:24,124 --> 00:00:25,458
                              Ethan, tu fais quoi ?
                              
                              10
                              00:00:25,492 --> 00:00:28,360
                              Je t'ai dit d'aller dormir
                              il y a une heure.
                              
                              11
                              00:00:28,395 --> 00:00:29,895
                              Maman, regarde.";

        #endregion

        var buffer = Encoding.UTF8.GetBytes(text);
        await using var memory = new MemoryStream(buffer);
        await using var compressed = await compressorService.CompressAsync(memory, CancellationToken.None);

        await using var decompressed = await compressor.DecompressAsync(compressed, CancellationToken.None);
        await using var memoryResult = new MemoryStream();
        await decompressed.CopyToAsync(memoryResult);

        var decompressedBytes = memoryResult.ToArray();
        Encoding.UTF8.GetString(decompressedBytes).Should().Be(text);
    }
}