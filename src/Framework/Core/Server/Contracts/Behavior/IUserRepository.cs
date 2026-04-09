namespace Crudspa.Framework.Core.Server.Contracts.Behavior;

public interface IUserRepository
{
    Task<IList<User>> SelectByIds(String connection, IEnumerable<Guid?> userIds, Guid? portalId = null);
    Task<User?> Select(String connection, Guid? userId, Guid? portalId = null);
    Task<Guid?> Insert(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, User user, Guid? portalId);
    Task<Guid?> Update(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, User user, Guid? portalId);
    Task Delete(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, User user);
    Task<List<Error>> Validate(String connection, User user, Guid? portalId);
}