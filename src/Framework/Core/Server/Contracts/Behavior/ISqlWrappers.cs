namespace Crudspa.Framework.Core.Server.Contracts.Behavior;

public interface ISqlWrappers
{
    Task WithConnection(Func<SqlConnection, SqlTransaction?, Task> func);
    Task<T> WithConnection<T>(Func<SqlConnection, SqlTransaction?, Task<T>> func);
    Task WithTransaction(Func<SqlConnection, SqlTransaction, Task> func);
    Task<T> WithTransaction<T>(Func<SqlConnection, SqlTransaction, Task<T>> func);
}