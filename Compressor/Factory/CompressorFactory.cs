using Compressor.Utils;
using InversionOfControl.Model.Factory;
using System;

namespace Compressor.Factory;

public class CompressorFactory : EnumFactory<AlgorithmEnum, ICompressorService>
{
    private readonly ICompressorService[] _servicesByMagicLength;
    private readonly ushort _maxLength;

    public CompressorFactory(IEnumerable<ICompressorService> services) : base(services)
    {
        _servicesByMagicLength = Services.Where(service => service.Definition.HasMagicNumber).OrderByDescending(service => service.Definition.MagicNumberLength).ToArray();
        _maxLength = _servicesByMagicLength[0].Definition.MagicNumberLength;
    }

    /// <summary>
    /// Get the service by the magic number
    /// </summary>
    /// <returns></returns>
    public async Task<ICompressorService> GetServiceByMagicNumberAsync(Stream stream, CancellationToken token)
    {
        stream.ResetPosition();
        var maxMagicNumber = new byte[_maxLength];
        _ = await stream.ReadAsync(maxMagicNumber.AsMemory(0, _maxLength), token);
        try
        {
            foreach (var service in _servicesByMagicLength)
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