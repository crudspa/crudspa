namespace Crudspa.Framework.Core.Server.Sproxies;

public static class OrganizationRoleInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Role role)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.OrganizationRoleInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@OrganizationId", role.OrganizationId);
        command.AddParameter("@Name", 75, role.Name);
        command.AddParameter("@Permissions", role.Permissions);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}