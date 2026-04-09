namespace Crudspa.Framework.Core.Server.Sproxies;

public static class UserUpdateResetPassword
{
    public static async Task Execute(String connection, Guid? sessionId, Guid? userId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.UserUpdateResetPassword";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@UserId", userId);

        await command.Execute(connection);
    }
}