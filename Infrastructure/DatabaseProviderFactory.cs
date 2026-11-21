using System.IO;
using ImHelper.Infrastructure.Oracle;
using ImHelper.Infrastructure.Sqlite;

namespace ImHelper.Infrastructure;

public sealed class DatabaseProviderFactory
{
    public IDatabaseProvider Create(DatabaseSettings settings, string baseDirectory)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentException.ThrowIfNullOrWhiteSpace(baseDirectory);

        return settings.Provider switch
        {
            DatabaseProviderKind.SQLite => new SqliteDatabaseProvider(
                Path.GetFullPath(settings.SqliteDatabasePath, baseDirectory)),
            DatabaseProviderKind.Oracle => new OracleDatabaseProvider(settings.OracleConnectionString),
            _ => throw new ArgumentOutOfRangeException(nameof(settings), settings.Provider, "Unsupported database provider.")
        };
    }
}
