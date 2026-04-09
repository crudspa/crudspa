namespace Crudspa.Framework.Core.Server.Contracts.Behavior;

public interface IOrganizationRepository
{
    Task<IList<Organization>> SelectByIds(String connection, IEnumerable<Guid?> organizationIds, Guid? portalId);
    Task<Organization?> Select(String connection, Guid? organizationId, Guid? portalId);
    Task<Guid?> Insert(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Organization organization, Guid? portalId = null);
    Task Update(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Organization organization, Guid? portalId);
    Task Delete(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Organization organization);
    Task<List<Error>> Validate(String connection, Organization organization, Guid? portalId = null);
}