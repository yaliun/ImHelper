using System.Data.Common;

namespace ImHelper.Infrastructure;

public interface IDatabaseProvider
{
    string SqlDirectoryName { get; }

    void Initialize();

    DbConnection OpenConnection();

    void AddParameter(DbCommand command, string name, object? value);
}
