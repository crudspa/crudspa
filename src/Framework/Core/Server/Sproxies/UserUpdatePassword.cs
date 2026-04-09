namespace Crudspa.Framework.Core.Server.Sproxies;

public static class UserUpdatePassword
{
    public static async Task Execute(String connection, Guid? sessionId, User user)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.UserUpdatePassword";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PasswordHash", 32, user.PasswordHash);
        command.AddParameter("@PasswordSalt", 32, user.PasswordSalt);

        await command.Execute(connection);
    }
}