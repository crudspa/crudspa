namespace Crudspa.Framework.Core.Server.Sproxies;

public static class UserSelectPassword
{
    public static async Task<User?> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.UserSelectPassword";

        command.AddParameter("@SessionId", sessionId);

        return await command.ReadSingle(connection, ReadUser);
    }

    private static User ReadUser(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Username = reader.ReadString(1),
            PasswordHash = reader.ReadByteArray(2),
            PasswordSalt = reader.ReadByteArray(3),
            ResetPassword = reader.ReadBoolean(4),
        };
    }
}