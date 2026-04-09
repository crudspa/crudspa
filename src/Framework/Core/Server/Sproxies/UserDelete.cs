namespace Crudspa.Framework.Core.Server.Sproxies;

public static class UserDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, User user)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.UserDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", user.Id);

        await command.Execute(connection, transaction);
    }
}