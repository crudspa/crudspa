namespace Crudspa.Framework.Core.Server.Sproxies;

public static class SessionEnd
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.SessionEnd";

        command.AddParameter("@Id", sessionId);

        await command.Execute(connection, transaction);
    }
}