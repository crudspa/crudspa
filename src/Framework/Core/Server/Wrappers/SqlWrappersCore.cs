namespace Crudspa.Framework.Core.Server.Wrappers;

public class SqlWrappersCore(IServerConfigService configService) : ISqlWrappers
{
    private readonly String _connectionString = configService.Fetch().Database;

    public async Task WithConnection(Func<SqlConnection, SqlTransaction?, Task> func)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await func.Invoke(connection, null);
    }

    public async Task<T> WithConnection<T>(Func<SqlConnection, SqlTransaction?, Task<T>> func)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        return await func.Invoke(connection, null);
    }

    public async Task WithTransaction(Func<SqlConnection, SqlTransaction, Task> func)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var transaction = connection.BeginTransaction();

        try
        {
            await func.Invoke(connection, transaction);
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<T> WithTransaction<T>(Func<SqlConnection, SqlTransaction, Task<T>> func)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var transaction = connection.BeginTransaction();

        try
        {
            var result = await func.Invoke(connection, transaction);
            transaction.Commit();
            return result;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public static async Task MergeBatch<T, TU, TV>(TU connection, TV transaction, Guid? sessionId,
        IEnumerable<T>? existingItems,
        IEnumerable<T>? updatedItems,
        Func<TU, TV, Guid?, T, Task<Guid?>> insertAction,
        Func<TU, TV, Guid?, T, Task> updateAction,
        Func<TU, TV, Guid?, T, Task> deleteAction)
        where T : class, IUnique
    {
        var existingList = existingItems?.ToList() ?? new();
        var updatedList = updatedItems?.ToList() ?? new();

        foreach (var updated in updatedList)
        {
            var existing = existingList.FirstOrDefault(x => x.Id.Equals(updated.Id));

            if (existing is null)
                updated.Id = await insertAction(connection, transaction, sessionId, updated);
            else
                await updateAction(connection, transaction, sessionId, updated);
        }

        if (existingList.Count == 0)
            return;

        foreach (var existing in existingList.Where(existing => updatedList.All(x => x.Id != existing.Id)))
            await deleteAction(connection, transaction, sessionId, existing);
    }
}