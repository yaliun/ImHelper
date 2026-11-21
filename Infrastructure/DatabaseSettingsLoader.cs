using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ImHelper.Infrastructure;

public sealed class DatabaseSettingsLoader
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public DatabaseSettings Load(string baseDirectory)
    {
        var defaultsPath = Path.Combine(baseDirectory, "DatabaseSettings.json");
        var defaults = ReadRequired(defaultsPath);

        var localPath = Path.Combine(baseDirectory, "DatabaseSettings.Local.json");
        var local = File.Exists(localPath) ? ReadRequired(localPath) : null;

        var provider = local?.Provider ?? defaults.Provider;
        if (!Enum.TryParse<DatabaseProviderKind>(provider, ignoreCase: true, out var databaseProvider))
        {
            throw new InvalidDataException($"Unsupported database provider '{provider}'.");
        }

        var sqliteDatabasePath = local?.SqliteDatabasePath ?? defaults.SqliteDatabasePath;
        if (string.IsNullOrWhiteSpace(sqliteDatabasePath))
        {
            throw new InvalidDataException("The SQLite database path must be configured.");
        }

        return new DatabaseSettings(
            databaseProvider,
            sqliteDatabasePath,
            local?.OracleConnectionString ?? defaults.OracleConnectionString);
    }

    private static SettingsFile ReadRequired(string path)
    {
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<SettingsFile>(json, SerializerOptions)
            ?? throw new InvalidDataException($"Database settings file is empty: {path}");
    }

    private sealed class SettingsFile
    {
        public string? Provider { get; init; }

        public string? SqliteDatabasePath { get; init; }

        public string? OracleConnectionString { get; init; }
    }
}
