namespace Crudspa.Content.Design.Server.Contracts.Behavior;

public interface IBinderRepository
{
    Task<Guid?> Insert(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Binder binder);
}