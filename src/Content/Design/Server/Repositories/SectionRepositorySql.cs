namespace Crudspa.Content.Design.Server.Repositories;

public class SectionRepositorySql : ISectionRepository
{
    public async Task<Guid?> Insert(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Section section)
    {
        return await SectionInsert.Execute(connection, transaction, sessionId, section);
    }

    public async Task Update(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Section section)
    {
        await SectionUpdate.Execute(connection, transaction, sessionId, section);
    }

    public async Task Delete(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Section section)
    {
        await SectionDelete.Execute(connection, transaction, sessionId, section);
    }

    public async Task SaveOrder(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, IEnumerable<IOrderable> orderables)
    {
        await SectionUpdateOrdinals.Execute(connection, transaction, sessionId, orderables);
    }
}