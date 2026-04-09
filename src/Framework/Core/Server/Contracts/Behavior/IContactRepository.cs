namespace Crudspa.Framework.Core.Server.Contracts.Behavior;

public interface IContactRepository
{
    Task<IList<Contact>> SelectByIds(String connection, IEnumerable<Guid?> contactIds, Guid? portalId = null);
    Task<Contact?> Select(String connection, Guid? contactId, Guid? portalId = null);
    Task<Guid?> Insert(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Contact contact, Guid? portalId = null);
    Task Update(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Contact contact, Guid? portalId = null);
    Task Delete(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Contact contact);
    Task<List<Error>> Validate(String connection, Contact contact, Guid? portalId = null);
}