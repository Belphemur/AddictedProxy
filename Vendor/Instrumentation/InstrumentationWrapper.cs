using System.Diagnostics;

namespace NeoSmart.Caching.Sqlite.Instrumentation;

public class InstrumentationWrapper
{
    public static readonly ActivitySource ActivitySource = new("SqlCache", typeof(InstrumentationWrapper).Assembly.GetName().Version?.ToString() ?? "1.0.0");
}