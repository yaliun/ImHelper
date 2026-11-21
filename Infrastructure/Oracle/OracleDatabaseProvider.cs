using System.Data.Common;
using ImHelper.Infrastructure;

namespace ImHelper.Infrastructure.Oracle;

public sealed class OracleDatabaseProvider(string? connectionString) : IDatabaseProvider
{
    public string SqlDirectoryName => "Oracle";

    public void Initialize()
    {
        throw new NotSupportedException(
            "Oracle support is a placeholder. Add Oracle.ManagedDataAccess and implement the Oracle provider before selecting it.");
    }

    public DbConnection OpenConnection()
    {
        throw new NotSupportedException(
            "Oracle support is a placeholder. Add Oracle.ManagedDataAccess and implement the Oracle provider before selecting it.");
    }

    public void AddParameter(DbCommand command, string name, object? value)
    {
        throw new NotSupportedException(
            "Oracle parameter binding is not implemented. Add Oracle.ManagedDataAccess and implement the Oracle provider before selecting it.");
    }
}
