using OpenTelemetry.Trace;

namespace NeoSmart.Caching.Sqlite.Instrumentation;

/// <summary>
/// Extension methods to simplify registering of SqlCache instrumentation
/// </summary>
public static class TracerProviderBuilderExtensions
{
    /// <summary>
    /// Adds Hangfire instrumentation to the tracer provider.
    /// </summary>
    /// <param name="builder"><see cref="TracerProviderBuilder"/> being configured.</param>
    /// <returns>The instance of <see cref="TracerProviderBuilder"/> to chain the calls.</returns>
    public static TracerProviderBuilder AddSqlCacheInstrumentation(
        this TracerProviderBuilder builder)
    {
        return builder.AddSource(InstrumentationWrapper.ActivitySource.Name);
    }
}

