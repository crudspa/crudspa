namespace Crudspa.Content.Design.Server.Contracts.Behavior;

public interface IPageRepository
{
    Task<IList<Page>> SelectForBinder(String connection, Guid? sessionId, Guid? binderId);
    Task<Page?> Select(String connection, Guid? sessionId, Page page);
    Task<Guid?> Insert(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Page page);
    Task Update(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Page page);
    Task Delete(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Page page);
    Task SaveOrder(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, IEnumerable<IOrderable> orderables);
}