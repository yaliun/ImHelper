namespace ImHelper.Infrastructure;

public enum DatabaseProviderKind
{
    SQLite,
    Oracle
}

public sealed record DatabaseSettings(
    DatabaseProviderKind Provider,
    string SqliteDatabasePath,
    string? OracleConnectionString);
