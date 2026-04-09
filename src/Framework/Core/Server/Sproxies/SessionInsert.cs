namespace Crudspa.Framework.Core.Server.Sproxies;

public static class SessionInsert
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? portalId, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.SessionInsert";

        command.AddParameter("@Id", sessionId);
        command.AddParameter("@PortalId", portalId);

        await command.Execute(connection, transaction);
    }
}