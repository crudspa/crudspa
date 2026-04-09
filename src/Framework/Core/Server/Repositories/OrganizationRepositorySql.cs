namespace Crudspa.Framework.Core.Server.Repositories;

public class OrganizationRepositorySql(ISessionService sessionService) : IOrganizationRepository
{
    public async Task<IList<Organization>> SelectByIds(String connection, IEnumerable<Guid?> organizationIds, Guid? portalId)
    {
        var organizations = await OrganizationSelectByIds.Execute(connection, organizationIds, portalId);

        return organizations;
    }

    public async Task<Organization?> Select(String connection, Guid? organizationId, Guid? portalId)
    {
        var organization = await OrganizationSelect.Execute(connection, organizationId, portalId);

        return organization;
    }

    public async Task<Guid?> Insert(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Organization organization, Guid? portalId = null)
    {
        organization.Id = await OrganizationInsert.Execute(connection, transaction, sessionId, organization);
        await SaveRoles(connection, transaction, sessionId, organization, null);
        await sessionService.InvalidateAll();
        return organization.Id;
    }

    public async Task Update(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Organization organization, Guid? portalId)
    {
        var existing = await OrganizationSelect.Execute(connection, transaction, organization.Id, portalId);
        await OrganizationUpdate.Execute(connection, transaction, sessionId, organization);
        await SaveRoles(connection, transaction, sessionId, organization, existing);
        await sessionService.InvalidateAll();
    }

    public async Task Delete(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Organization organization)
    {
        if (organization.Id is null)
            return;

        await OrganizationDelete.Execute(connection, transaction, sessionId, organization);
        await sessionService.InvalidateAll();
    }

    public async Task<List<Error>> Validate(String connection, Organization organization, Guid? portalId = null)
    {
        return await ErrorsEx.Validate(async errors =>
        {
            var ordinal = 1;

            foreach (var role in organization.Roles)
            {
                var roleErrors = role.Validate();
                roleErrors.PrependMessages($"(Role #{ordinal}) ");
                errors.AddRange(roleErrors);

                ordinal++;
            }

            await Task.CompletedTask;
        });
    }

    private static async Task SaveRoles(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Organization incoming, Organization? existing)
    {
        foreach (var role in incoming.Roles)
            role.OrganizationId = incoming.Id;

        await SqlWrappersCore.MergeBatch(connection, transaction, sessionId,
            existing?.Roles ?? [],
            incoming.Roles,
            OrganizationRoleInsert.Execute,
            OrganizationRoleUpdate.Execute,
            OrganizationRoleDelete.Execute);
    }
}