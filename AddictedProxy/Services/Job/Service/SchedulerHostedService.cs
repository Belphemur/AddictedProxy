using AddictedProxy.Services.Credentials.Job;
using AddictedProxy.Services.Provider.Shows.Jobs;
using Hangfire;

namespace AddictedProxy.Services.Job.Service;

public class SchedulerHostedService : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        RecurringJob.AddOrUpdate<RefreshAvailableShowsJob>(nameof(RefreshAvailableShowsJob), job => job.ExecuteAsync(CancellationToken.None), "0 2 * * *");
        RecurringJob.AddOrUpdate<CheckCompletedTmdbJob>($"{nameof(CheckCompletedTmdbJob)}-not-completed", job => job.ExecuteAsync(false, CancellationToken.None), "50 2 * * *");
        RecurringJob.AddOrUpdate<CheckCompletedTmdbJob>($"{nameof(CheckCompletedTmdbJob)}-completed", job => job.ExecuteAsync(true, CancellationToken.None), "10 2 * * */2");
        RecurringJob.AddOrUpdate<DownloadCredsRedeemerJob>(nameof(DownloadCredsRedeemerJob), job => job.ExecuteAsync(CancellationToken.None), "11 * * * *");

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}