namespace Crudspa.Framework.Jobs.Server.Sproxies;

public static class SessionInsert
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Guid? portalId, Guid? userId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkJobs.SessionInsert";

        command.AddParameter("@Id", sessionId);
        command.AddParameter("@PortalId", portalId);
        command.AddParameter("@UserId", userId);

        await command.Execute(connection, transaction);
    }
}