namespace Crudspa.Framework.Core.Server.Sproxies;

public static class UserUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, User user)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.UserUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", user.Id);
        command.AddParameter("@Username", 150, user.Username);
        command.AddParameter("@ResetPassword", user.ResetPassword ?? true);
        command.AddParameter("@Roles", user.Roles);

        await command.Execute(connection, transaction);
    }
}