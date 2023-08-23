using Compressor.Utils;
using InversionOfControl.Model.Factory;
using System;

namespace Compressor.Factory;

public class CompressorFactory : EnumFactory<AlgorithmEnum, ICompressorService>
{
    public CompressorFactory(IEnumerable<ICompressorService> services) : base(services)
    {
    }

    /// <summary>
    /// Get the service by the magic number
    /// </summary>
    /// <returns></returns>
    public async Task<ICompressorService> GetServiceByMagicNumberAsync(Stream stream, CancellationToken token)
    {
        stream.ResetPosition();
        foreach (var service in Services.Where(service => service.Definition.HasMagicNumber).OrderByDescending(service => service.Definition.MagicNumberLength))
        {
            try
            {
                var magicNumber = new byte[service.Definition.MagicNumberLength];
                _ = await stream.ReadAsync(magicNumber.AsMemory(0, service.Definition.MagicNumberLength), token);
                if (service.Definition.MagicNumber == BitConverter.ToString(magicNumber))
                {
                    return service;
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