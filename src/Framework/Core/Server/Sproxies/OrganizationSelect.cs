namespace Crudspa.Framework.Core.Server.Sproxies;

public static class OrganizationSelect
{
    public static async Task<Organization?> Execute(String connection, Guid? organizationId, Guid? portalId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.OrganizationSelect";

        command.AddParameter("@Id", organizationId);
        command.AddParameter("@PortalId", portalId);

        return await command.ExecuteQuery(connection, ReadOrganizationGraph);
    }

    public static async Task<Organization?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? organizationId, Guid? portalId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.OrganizationSelect";

        command.AddParameter("@Id", organizationId);
        command.AddParameter("@PortalId", portalId);

        return await command.ExecuteQuery(connection, transaction, ReadOrganizationGraph);
    }

    private static async Task<Organization?> ReadOrganizationGraph(SqlDataReader reader)
    {
        if (!await reader.ReadAsync())
            return null;

        var organization = ReadOrganization(reader);

        await reader.NextResultAsync();

        while (await reader.ReadAsync())
            organization.Roles.Add(ReadRole(reader));

        await reader.NextResultAsync();

        var permissions = new List<Named>();

        while (await reader.ReadAsync())
            permissions.Add(reader.ReadNamed());

        await reader.NextResultAsync();

        var rolePermissions = new List<RolePermission>();

        while (await reader.ReadAsync())
            rolePermissions.Add(ReadRolePermission(reader));

        foreach (var role in organization.Roles)
        foreach (var permission in permissions)
            role.Permissions.Add(new()
            {
                Id = permission.Id,
                Name = permission.Name,
                Selected = rolePermissions.HasAny(x => x.RoleId.Equals(role.Id) && x.PermissionId.Equals(permission.Id)),
            });

        return organization;
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