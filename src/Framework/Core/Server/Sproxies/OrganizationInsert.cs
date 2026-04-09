namespace Crudspa.Framework.Core.Server.Sproxies;

public static class OrganizationInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Organization organization)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.OrganizationInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Name", 75, organization.Name);
        command.AddParameter("@TimeZoneId", 32, organization.TimeZoneId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}