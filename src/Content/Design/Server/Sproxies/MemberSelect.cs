namespace Crudspa.Content.Design.Server.Sproxies;

public static class MemberSelect
{
    public static async Task<Member?> Execute(String connection, Guid? sessionId, Member member)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.MemberSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", member.Id);

        return await command.ExecuteQuery(connection, async reader =>
        {
            if (!await reader.ReadAsync())
                return null;

            var ret = ReadMember(reader);

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                ret.TokenValues.Add(ReadTokenValue(reader));

            return ret;
        });
    }

    private static Member ReadMember(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            MembershipId = reader.ReadGuid(1),
            Status = reader.ReadEnum<Member.Statuses>(2),
            Contact = new()
            {
                Id = reader.ReadGuid(3),
                FirstName = reader.ReadString(4),
                LastName = reader.ReadString(5),
                Email = reader.ReadString(6),
            },
        };
    }

    private static TokenValue ReadTokenValue(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            TokenId = reader.ReadGuid(1),
            ContactId = reader.ReadGuid(2),
            Value = reader.ReadString(3),
            TokenKey = reader.ReadString(4),
        };
    }
}