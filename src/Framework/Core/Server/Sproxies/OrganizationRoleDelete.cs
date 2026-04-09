namespace Crudspa.Framework.Core.Server.Sproxies;

public static class OrganizationRoleDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Role role)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.OrganizationRoleDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", role.Id);

        await command.Execute(connection, transaction);
    }
}