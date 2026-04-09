namespace Crudspa.Framework.Core.Server.Sproxies;

public static class SessionUpdateUser
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Guid? userId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.SessionUpdateUser";

        command.AddParameter("@Id", sessionId);
        command.AddParameter("@UserId", userId);

        await command.Execute(connection, transaction);
    }
}