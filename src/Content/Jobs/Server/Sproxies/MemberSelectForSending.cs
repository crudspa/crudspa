namespace Crudspa.Content.Jobs.Server.Sproxies;

public static class MemberSelectForSending
{
    public static async Task<IList<Member>> Execute(String connection, Guid? sessionId, Guid? membershipId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentJobs.MemberSelectForSending";

        command.AddParameter("@MembershipId", membershipId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            var members = new List<Member>();

            while (await reader.ReadAsync())
                members.Add(ReadMember(reader));

            await reader.NextResultAsync();

            var tokenValues = new List<TokenValue>();

            while (await reader.ReadAsync())
                tokenValues.Add(ReadTokenValue(reader));

            foreach (var member in members)
                member.TokenValues = tokenValues.Where(x => x.ContactId.Equals(member.Contact.Id)).ToObservable();

            return members;
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