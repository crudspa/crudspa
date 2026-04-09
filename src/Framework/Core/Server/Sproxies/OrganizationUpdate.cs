namespace Crudspa.Framework.Core.Server.Sproxies;

public static class OrganizationUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Organization organization)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.OrganizationUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", organization.Id);
        command.AddParameter("@Name", 75, organization.Name);
        command.AddParameter("@TimeZoneId", 32, organization.TimeZoneId);

        await command.Execute(connection, transaction);
    }
}