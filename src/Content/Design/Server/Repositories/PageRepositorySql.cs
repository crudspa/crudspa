using PageSelectForBinder = Crudspa.Content.Design.Server.Sproxies.PageSelectForBinder;

namespace Crudspa.Content.Design.Server.Repositories;

public class PageRepositorySql : IPageRepository
{
    public async Task<IList<Page>> SelectForBinder(String connection, Guid? sessionId, Guid? binderId)
    {
        return await PageSelectForBinder.Execute(connection, sessionId, binderId);
    }

    public async Task<Page?> Select(String connection, Guid? sessionId, Page page)
    {
        return await PageSelect.Execute(connection, sessionId, page);
    }

    public async Task<Guid?> Insert(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Page page)
    {
        return await PageInsert.Execute(connection, transaction, sessionId, page);
    }

    public async Task Update(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Page page)
    {
        await PageUpdate.Execute(connection, transaction, sessionId, page);
    }

    public async Task Delete(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Page page)
    {
        await PageDelete.Execute(connection, transaction, sessionId, page);
    }

    public async Task SaveOrder(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, IEnumerable<IOrderable> orderables)
    {
        await PageUpdateOrdinals.Execute(connection, transaction, sessionId, orderables);
    }
}