namespace Crudspa.Framework.Core.Server.Sproxies;

public static class OrganizationRoleUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Role role)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.OrganizationRoleUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", role.Id);
        command.AddParameter("@Name", 75, role.Name);
        command.AddParameter("@Permissions", role.Permissions);

        await command.Execute(connection, transaction);
    }
}