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
        var maxMagicNumberLength = Services.Where(service => service.Definition.HasMagicNumber).Max(service => service.Definition.MagicNumberLength);
        var maxMagicNumber = new byte[maxMagicNumberLength];
        _ = await stream.ReadAsync(maxMagicNumber.AsMemory(0, maxMagicNumberLength), token);
        try
        {
            foreach (var service in Services.Where(service => service.Definition.HasMagicNumber).OrderByDescending(service => service.Definition.MagicNumberLength))
            {
                var magicNumber = maxMagicNumber.AsSpan(0, service.Definition.MagicNumberLength).ToArray();
                if (magicNumber.CompareWith(service.Definition.MagicNumberByte))
                {
                    return service;
                }
            }
        }
        finally
        {
            stream.ResetPosition();
        }

        throw new ArgumentOutOfRangeException(nameof(stream), stream, $"No service ({nameof(ICompressorService)}) found for the given stream");
    }
}