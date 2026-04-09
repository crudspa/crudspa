namespace Crudspa.Framework.Core.Server.Sproxies;

public static class UserInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, User user, Guid? portalId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.UserInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ContactId", user.Contact.Id);
        command.AddParameter("@PortalId", portalId);
        command.AddParameter("@OrganizationId", user.OrganizationId);
        command.AddParameter("@Username", 150, user.Username);
        command.AddParameter("@ResetPassword", user.ResetPassword ?? true);
        command.AddParameter("@Roles", user.Roles);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}