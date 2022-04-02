using AddictedProxy.Database.Context;
using AddictedProxy.Database.EnvVar;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Z.EntityFramework.Extensions;

namespace AddictedProxy.Database.Bootstrap;

public class SetupEfCoreHostedService : IHostedService
{
    private readonly EFCoreLicense _license;

    public SetupEfCoreHostedService(EFCoreLicense license)
    {
        _license = license;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        EntityFrameworkManager.ContextFactory = context => new EntityContext();
        EFCoreConfig.AddLicense(_license.License, _license.Key);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}