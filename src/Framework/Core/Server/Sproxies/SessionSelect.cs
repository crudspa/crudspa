namespace Crudspa.Framework.Core.Server.Sproxies;

public static class SessionSelect
{
    public static async Task<Session?> Execute(String connection, Guid? sessionId, Guid? portalId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.SessionSelect";

        command.AddParameter("@Id", sessionId);
        command.AddParameter("@PortalId", portalId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            if (!await reader.ReadAsync())
                return null;

            var session = ReadSession(reader);

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                session.Permissions.Add(reader.ReadGuid(0).GetValueOrDefault());

            return session;
        });
    }

    private static Session ReadSession(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            PortalId = reader.ReadGuid(1),
            User = new()
            {
                Id = reader.ReadGuid(2),
                Username = reader.ReadString(3),
                ResetPassword = reader.ReadBoolean(4),
                OrganizationId = reader.ReadGuid(5),
                Contact = new()
                {
                    Id = reader.ReadGuid(6),
                    FirstName = reader.ReadString(7),
                    LastName = reader.ReadString(8),
                    TimeZoneId = reader.ReadString(9),
                },
            },
        };
    }
}