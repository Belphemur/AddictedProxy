using System.Data;
using AddictedProxy.Database.Context;
using AddictedProxy.OneTimeMigration.Model;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AddictedProxy.Migrations.Services;

/// <summary>
/// One-time migration that ensures every table with an UpdatedAt column has the PostgreSQL trigger in place.
/// </summary>
[MigrationDate(2026, 3, 12)]
public class EnsureUpdatedAtTriggersMigration : IMigration
{
    private readonly EntityContext _entityContext;
    private readonly ILogger<EnsureUpdatedAtTriggersMigration> _logger;

    public EnsureUpdatedAtTriggersMigration(
        EntityContext entityContext,
        ILogger<EnsureUpdatedAtTriggersMigration> logger)
    {
        _entityContext = entityContext;
        _logger = logger;
    }

    public async Task ExecuteAsync(PerformContext context, CancellationToken token)
    {
        context.WriteLine("Ensuring UpdatedAt triggers exist for all timestamped tables...");

        var connection = _entityContext.Database.GetDbConnection();
        var shouldCloseConnection = connection.State != ConnectionState.Open;
        if (shouldCloseConnection)
        {
            await connection.OpenAsync(token);
        }

        try
        {
            await ExecuteNonQueryAsync(connection, """
                                              CREATE OR REPLACE FUNCTION updated_set_now()
                                              RETURNS TRIGGER AS $$
                                              BEGIN
                                                NEW."UpdatedAt" := now() at time zone 'utc';
                                                RETURN NEW;
                                              END;
                                              $$ LANGUAGE plpgsql;
                                              """, token);

            var tables = await GetTablesWithUpdatedAtAsync(connection, token);
            context.WriteLine($"Found {tables.Count} tables with an UpdatedAt column.");

            foreach (var table in tables)
            {
                var triggerName = QuoteIdentifier($"updated_at_trigger_{table.Name.ToLowerInvariant()}");
                var qualifiedTableName = $"{QuoteIdentifier(table.Schema)}.{QuoteIdentifier(table.Name)}";

                await ExecuteNonQueryAsync(connection, $"DROP TRIGGER IF EXISTS {triggerName} ON {qualifiedTableName};", token);
                await ExecuteNonQueryAsync(connection, $"CREATE TRIGGER {triggerName} BEFORE UPDATE ON {qualifiedTableName} FOR EACH ROW EXECUTE FUNCTION updated_set_now();", token);

                context.WriteLine($"Ensured trigger on {table.Schema}.{table.Name}");
            }

            _logger.LogInformation("Ensured UpdatedAt triggers on {TableCount} tables", tables.Count);
        }
        finally
        {
            if (shouldCloseConnection)
            {
                await connection.CloseAsync();
            }
        }
    }

    private static async Task<List<DbTable>> GetTablesWithUpdatedAtAsync(System.Data.Common.DbConnection connection, CancellationToken token)
    {
        var tables = new List<DbTable>();

        await using var command = connection.CreateCommand();
        command.CommandText = """
                              SELECT c.table_schema, c.table_name
                              FROM information_schema.columns c
                              WHERE c.column_name = 'UpdatedAt'
                                AND c.table_schema NOT IN ('information_schema', 'pg_catalog')
                              GROUP BY c.table_schema, c.table_name
                              ORDER BY c.table_schema, c.table_name
                              """;

        await using var reader = await command.ExecuteReaderAsync(token);
        while (await reader.ReadAsync(token))
        {
            tables.Add(new DbTable(reader.GetString(0), reader.GetString(1)));
        }

        return tables;
    }

    private static async Task ExecuteNonQueryAsync(System.Data.Common.DbConnection connection, string sql, CancellationToken token)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        await command.ExecuteNonQueryAsync(token);
    }

    private static string QuoteIdentifier(string identifier)
    {
        return $"\"{identifier.Replace("\"", "\"\"")}\"";
    }

    private readonly record struct DbTable(string Schema, string Name);
}