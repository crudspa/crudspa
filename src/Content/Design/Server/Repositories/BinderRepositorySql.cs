namespace Crudspa.Content.Design.Server.Repositories;

public class BinderRepositorySql : IBinderRepository
{
    public async Task<Guid?> Insert(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Binder binder)
    {
        return await BinderInsert.Execute(connection, transaction, sessionId, binder);
    }
}