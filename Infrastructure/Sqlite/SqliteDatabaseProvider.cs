using System.Data.Common;
using System.IO;
using ImHelper.Infrastructure;
using Microsoft.Data.Sqlite;

namespace ImHelper.Infrastructure.Sqlite;

public sealed class SqliteDatabaseProvider(string databasePath) : IDatabaseProvider
{
    private readonly string connectionString = new SqliteConnectionStringBuilder
    {
        DataSource = databasePath,
        Mode = SqliteOpenMode.ReadWriteCreate
    }.ToString();

    public string SqlDirectoryName => "SQLite";

    public void Initialize()
    {
        var databaseDirectory = Path.GetDirectoryName(databasePath)
            ?? throw new InvalidOperationException("The SQLite database path must include a directory.");

        Directory.CreateDirectory(databaseDirectory);

        using var connection = OpenConnection();
    }

    public DbConnection OpenConnection()
    {
        var connection = new SqliteConnection(connectionString);
        connection.Open();
        return connection;
    }

    public void AddParameter(DbCommand command, string name, object? value)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var parameter = command.CreateParameter();
        parameter.ParameterName = $"@{name}";
        parameter.Value = value ?? DBNull.Value;
        command.Parameters.Add(parameter);
    }
}
