using System.Data;

namespace ImHelper.Infrastructure;

public interface ISqlFileExecutor
{
    Task<int> ExecuteNonQueryAsync(
        string sqlFileName,
        IReadOnlyDictionary<string, object?> parameters,
        CancellationToken cancellationToken = default);

    Task<object?> ExecuteScalarAsync(
        string sqlFileName,
        IReadOnlyDictionary<string, object?> parameters,
        CancellationToken cancellationToken = default);

    Task<DataTable> QueryAsync(
        string sqlFileName,
        IReadOnlyDictionary<string, object?> parameters,
        CancellationToken cancellationToken = default);
}
