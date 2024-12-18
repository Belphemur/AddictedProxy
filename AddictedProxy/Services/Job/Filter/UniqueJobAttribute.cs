using System.Text.Json;
using AddictedProxy.Services.Job.Model;
using Hangfire;
using Hangfire.Server;

namespace AddictedProxy.Services.Job.Filter;

using Hangfire.Client;
using Hangfire.Common;
using Hangfire.States;
using Hangfire.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

public class UniqueJobAttribute : JobFilterAttribute, IClientFilter, IApplyStateFilter, IClientExceptionFilter
{
    private const string FingerprintJobParameterKey = "fingerprint";
    private const string TimestampKey = "Timestamp";
    private static readonly TimeSpan LockTimeout = TimeSpan.FromSeconds(30);
    public double TtlFingerprintSeconds { get; set; } = TimeSpan.FromDays(1).TotalSeconds;

    private TimeSpan TtlFingerprint => TimeSpan.FromSeconds(TtlFingerprintSeconds);

    private bool AddFingerprintIfNotExists(IStorageConnection connection, Job? job, CreatingContext creatingContext)
    {
        var fingerprintKey = GetFingerprintKey(job);
        var fingerprintLockKey = GetFingerprintLockKey(fingerprintKey);
        using var distributedLock = connection.AcquireDistributedLock(fingerprintLockKey, LockTimeout);
        var fingerprint = connection.GetAllEntriesFromHash(fingerprintKey);

        if (fingerprint != null
            && fingerprint.TryGetValue(TimestampKey, out var timestamp)
            && DateTimeOffset.TryParse(timestamp, out var lastTimestamp)
            && DateTimeOffset.UtcNow - lastTimestamp < TtlFingerprint)
        {
            // Fingerprint exists and is actual.
            return false;
        }

        // Fingerprint does not exist, it is invalid (no `Timestamp` key),
        // or it is not actual (ttl expired).
        using var transaction = connection.CreateWriteTransaction();
        transaction.SetRangeInHash(fingerprintKey, [
            new(TimestampKey, DateTimeOffset.UtcNow.ToString("o"))
        ]);
        creatingContext.SetJobParameter(FingerprintJobParameterKey, fingerprintKey);
        transaction.Commit();

        return true;
    }

    private static void RemoveFingerprint(IStorageConnection connection, BackgroundJob job)
    {
        var fingerprintKey = SerializationHelper.Deserialize<string?>(connection.GetJobParameter(job.Id, FingerprintJobParameterKey));

        if (string.IsNullOrEmpty(fingerprintKey))
        {
            fingerprintKey = GetFingerprintKey(job.Job);
        }

        var fingerprintLockKey = GetFingerprintLockKey(fingerprintKey);
        using var distributedLock = connection.AcquireDistributedLock(fingerprintLockKey, LockTimeout);
        using var transaction = connection.CreateWriteTransaction();
        transaction.RemoveHash(fingerprintKey);
        transaction.Commit();
    }

    private static string GetFingerprintLockKey(string fingerprintKey)
    {
        return $"{fingerprintKey}:lock";
    }

    private static string GetFingerprintKey(Job? job)
    {
        return $"fingerprint:{GetFingerprint(job)}";
    }

    private static string GetFingerprint(Job? job)
    {
        var parameters = string.Empty;
        if (job?.Type == null || job.Method == null)
        {
            return string.Empty;
        }

        var uniqueKey = job.Args?.Where(arg => arg is IUniqueKey).Cast<IUniqueKey>().Select(key => key.Key).ToArray() ?? [];

        if (uniqueKey.Length > 0)
        {
            parameters = string.Join(".", uniqueKey);
        }
        else if (job.Args is { Count: > 0 })
        {
            parameters = JsonSerializer.Serialize(job.Args.Where(arg => arg is not CancellationToken));
        }


        //https://gist.github.com/odinserj/a8332a3f486773baa009#gistcomment-1898401
        var payload = $"{job.Type.FullName}.{job.Method.Name}.{parameters}";
        var hash = SHA384.HashData(System.Text.Encoding.UTF8.GetBytes(payload));
        var fingerprint = Convert.ToBase64String(hash);
        return fingerprint;
    }

    public void OnCreating(CreatingContext filterContext)
    {
        if (!AddFingerprintIfNotExists(filterContext.Connection, filterContext.Job, filterContext))
        {
            filterContext.Canceled = true;
        }
    }

    public void OnCreated(CreatedContext filterContext)
    {
        //do nothing
    }


    public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
    {
        if (context.NewState.Name.Equals(SucceededState.StateName)
            || context.NewState.Name.Equals(FailedState.StateName)
            || context.NewState.Name.Equals(DeletedState.StateName))
        {
            RemoveFingerprint(context.Connection, context.BackgroundJob);
        }
    }

    public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
    {
        // do nothing
    }

    public void OnClientException(ClientExceptionContext filterContext)
    {
        if (filterContext.Exception is DistributedLockTimeoutException or InvalidOperationException)
        {
            filterContext.ExceptionHandled = true;
        }
    }
}