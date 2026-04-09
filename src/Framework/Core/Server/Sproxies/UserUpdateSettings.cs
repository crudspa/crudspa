namespace Crudspa.Framework.Core.Server.Sproxies;

public static class UserUpdateSettings
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, User user)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.UserUpdateSettings";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Username", 250, user.Username);
        command.AddParameter("@FirstName", 75, user.Contact.FirstName);
        command.AddParameter("@LastName", 75, user.Contact.LastName);
        command.AddParameter("@TimeZoneId", 32, user.Contact.TimeZoneId);

        await command.Execute(connection, transaction);
    }
}