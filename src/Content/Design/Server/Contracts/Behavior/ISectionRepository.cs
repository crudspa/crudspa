namespace Crudspa.Content.Design.Server.Contracts.Behavior;

public interface ISectionRepository
{
    Task<Guid?> Insert(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Section section);
    Task Update(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Section section);
    Task Delete(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Section section);
    Task SaveOrder(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, IEnumerable<IOrderable> orderables);
}