using Compressor.Utils;
using InversionOfControl.Model.Factory;

namespace Compressor.Factory;

public class CompressorFactory : EnumFactory<CompressorType, ICompressorService>
{
    public CompressorFactory(IEnumerable<ICompressorService> services) : base(services)
    {
    }

    /// <summary>
    /// Get the service by the magic number
    /// </summary>
    /// <returns></returns>
    public async Task<ICompressorService> GetServiceByMagicNumberAsync(MemoryStream stream, CancellationToken token)
    {
        stream.ResetPosition();
        foreach (var service in Services)
        {
            try
            {
                var magicNumber = new byte[service.Key.MagicNumberLength];
                _ = await stream.ReadAsync(magicNumber, 0, service.Key.MagicNumberLength, token);
                if (service.Key.MagicNumber == BitConverter.ToString(magicNumber))
                {
                    return service.Value;
                }
            }
            finally
            {
                stream.ResetPosition();
            }
        }
        
        throw new ArgumentOutOfRangeException(nameof(stream), stream, $"No service ({nameof(ICompressorService)}) found for the given stream");
    }
}