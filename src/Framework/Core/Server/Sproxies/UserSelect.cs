namespace Crudspa.Framework.Core.Server.Sproxies;

public static class UserSelect
{
    public static async Task<User?> Execute(String connection, Guid? userId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.UserSelect";

        command.AddParameter("@Id", userId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            if (!await reader.ReadAsync())
                return null;

            var user = ReadUser(reader);

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                user.Contact.Emails.Add(ReadContactEmail(reader));

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                user.Roles.Add(reader.ReadSelectable());

            return user;
        });
    }

    private static User ReadUser(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            PortalId = reader.ReadGuid(1),
            Username = reader.ReadString(2),
            PasswordHash = reader.ReadByteArray(3),
            PasswordSalt = reader.ReadByteArray(4),
            ResetPassword = reader.ReadBoolean(5),
            Contact = new()
            {
                Id = reader.ReadGuid(6),
                FirstName = reader.ReadString(7),
                LastName = reader.ReadString(8),
                TimeZoneId = reader.ReadString(9),
            },
        };
    }

    private static ContactEmail ReadContactEmail(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            ContactId = reader.ReadGuid(1),
            Email = reader.ReadString(2),
            Ordinal = reader.ReadInt32(3),
        };
    }
}