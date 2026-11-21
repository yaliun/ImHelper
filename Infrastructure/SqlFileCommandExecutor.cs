using System.Data;
using System.Data.Common;
using System.IO;
using ImHelper.Infrastructure;

namespace ImHelper.Infrastructure;

public sealed class SqlFileCommandExecutor : ISqlFileExecutor
{
    private readonly IDatabaseProvider databaseProvider;
    private readonly ISqlExecutionErrorNotifier errorNotifier;
    private readonly string sqlDirectory;

    public SqlFileCommandExecutor(
        IDatabaseProvider databaseProvider,
        ISqlExecutionErrorNotifier errorNotifier,
        string baseDirectory)
    {
        ArgumentNullException.ThrowIfNull(databaseProvider);
        ArgumentNullException.ThrowIfNull(errorNotifier);
        ArgumentException.ThrowIfNullOrWhiteSpace(baseDirectory);

        this.databaseProvider = databaseProvider;
        this.errorNotifier = errorNotifier;
        sqlDirectory = Path.Combine(baseDirectory, "Sql", databaseProvider.SqlDirectoryName);
    }

    public async Task<int> ExecuteNonQueryAsync(
        string sqlFileName,
        IReadOnlyDictionary<string, object?> parameters,
        CancellationToken cancellationToken = default)
    {
        var (command, connection) = await CreateCommandAsync(sqlFileName, parameters, cancellationToken);
        await using (connection)
        await using (command)
        {
            try
            {
                return await command.ExecuteNonQueryAsync(cancellationToken);
            }
            catch (DbException exception)
            {
                throw ReportExecutionFailure(sqlFileName, exception);
            }
        }
    }

    public async Task<object?> ExecuteScalarAsync(
        string sqlFileName,
        IReadOnlyDictionary<string, object?> parameters,
        CancellationToken cancellationToken = default)
    {
        var (command, connection) = await CreateCommandAsync(sqlFileName, parameters, cancellationToken);
        await using (connection)
        await using (command)
        {
            try
            {
                return await command.ExecuteScalarAsync(cancellationToken);
            }
            catch (DbException exception)
            {
                throw ReportExecutionFailure(sqlFileName, exception);
            }
        }
    }

    public async Task<DataTable> QueryAsync(
        string sqlFileName,
        IReadOnlyDictionary<string, object?> parameters,
        CancellationToken cancellationToken = default)
    {
        var (command, connection) = await CreateCommandAsync(sqlFileName, parameters, cancellationToken);
        await using (connection)
        await using (command)
        {
            try
            {
                await using var reader = await command.ExecuteReaderAsync(cancellationToken);
                var result = new DataTable();
                result.Load(reader);
                return result;
            }
            catch (DbException exception)
            {
                throw ReportExecutionFailure(sqlFileName, exception);
            }
        }
    }

    private async Task<(DbCommand Command, DbConnection Connection)> CreateCommandAsync(
        string sqlFileName,
        IReadOnlyDictionary<string, object?> parameters,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        var sqlPath = GetSqlPath(sqlFileName);
        string sql;

        try
        {
            sql = await File.ReadAllTextAsync(sqlPath, cancellationToken);
        }
        catch (FileNotFoundException exception)
        {
            throw ReportFileFailure(sqlPath, exception);
        }
        catch (DirectoryNotFoundException exception)
        {
            throw ReportFileFailure(sqlPath, exception);
        }
        catch (IOException exception)
        {
            throw ReportFileFailure(sqlPath, exception);
        }
        catch (UnauthorizedAccessException exception)
        {
            throw ReportFileFailure(sqlPath, exception);
        }

        DbConnection? connection = null;
        DbCommand? command = null;
        try
        {
            connection = databaseProvider.OpenConnection();
            command = connection.CreateCommand();
            command.CommandText = sql;
            foreach (var parameter in parameters)
            {
                databaseProvider.AddParameter(command, parameter.Key, parameter.Value);
            }

            return (command, connection);
        }
        catch (DbException exception)
        {
            command?.Dispose();
            connection?.Dispose();
            throw ReportExecutionFailure(sqlFileName, exception);
        }
        catch
        {
            command?.Dispose();
            connection?.Dispose();
            throw;
        }
    }

    private string GetSqlPath(string sqlFileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sqlFileName);
        if (!string.Equals(Path.GetFileName(sqlFileName), sqlFileName, StringComparison.Ordinal)
            || !string.Equals(Path.GetExtension(sqlFileName), ".sql", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Specify a SQL file name ending in .sql without a directory path.", nameof(sqlFileName));
        }

        return Path.Combine(sqlDirectory, sqlFileName);
    }

    private SqlFileOperationException ReportFileFailure(string sqlPath, Exception exception)
    {
        var message = $"SQL 파일을 읽지 못했습니다: {sqlPath}{Environment.NewLine}{exception.Message}";
        errorNotifier.ShowError(message);
        return new SqlFileOperationException(message, exception);
    }

    private SqlFileOperationException ReportExecutionFailure(string sqlFileName, DbException exception)
    {
        var sqlPath = GetSqlPath(sqlFileName);
        var message = $"SQL 실행에 실패했습니다: {sqlPath}{Environment.NewLine}{exception.Message}";
        errorNotifier.ShowError(message);
        return new SqlFileOperationException(message, exception);
    }
}
