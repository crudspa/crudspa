namespace Crudspa.Framework.Core.Server.Sproxies;

public static class OrganizationDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Organization organization)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.OrganizationDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", organization.Id);

        await command.Execute(connection, transaction);
    }
}