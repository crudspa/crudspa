namespace Crudspa.Framework.Core.Server.Sproxies;

public static class OrganizationSelectByIds
{
    public static async Task<IList<Organization>> Execute(String connection, IEnumerable<Guid?> organizationIds, Guid? portalId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.OrganizationSelectByIds";

        command.AddParameter("@Ids", organizationIds.Distinct());
        command.AddParameter("@PortalId", portalId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            var organizations = new List<Organization>();

            while (await reader.ReadAsync())
                organizations.Add(ReadOrganization(reader));

            await reader.NextResultAsync();

            var roles = new List<Role>();

            while (await reader.ReadAsync())
                roles.Add(ReadRole(reader));

            await reader.NextResultAsync();

            var permissions = new List<Named>();

            while (await reader.ReadAsync())
                permissions.Add(reader.ReadNamed());

            await reader.NextResultAsync();

            var rolePermissions = new List<RolePermission>();

            while (await reader.ReadAsync())
                rolePermissions.Add(ReadRolePermission(reader));

            foreach (var organization in organizations)
            foreach (var role in roles.Where(x => x.OrganizationId.Equals(organization.Id)))
                organization.Roles.Add(role);

            foreach (var organization in organizations)
            foreach (var role in organization.Roles)
            foreach (var permission in permissions)
                role.Permissions.Add(new()
                {
                    Id = permission.Id,
                    Name = permission.Name,
                    Selected = rolePermissions.HasAny(x => x.RoleId.Equals(role.Id) && x.PermissionId.Equals(permission.Id)),
                });

            return organizations;
        });
    }

    private static Organization ReadOrganization(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Name = reader.ReadString(1) ?? "Organization",
            TimeZoneId = reader.ReadString(2),
        };
    }

    private static Role ReadRole(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Name = reader.ReadString(1),
            OrganizationId = reader.ReadGuid(2),
        };
    }

    private static RolePermission ReadRolePermission(SqlDataReader reader)
    {
        return new()
        {
            RoleId = reader.ReadGuid(0),
            PermissionId = reader.ReadGuid(1),
        };
    }
}