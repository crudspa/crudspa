namespace Crudspa.Content.Design.Server.Sproxies;

public static class TokenSelectForMembership
{
    public static async Task<IList<Token>> Execute(String connection, Guid? sessionId, Guid? membershipId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.TokenSelectForMembership";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@MembershipId", membershipId);

        return await command.ReadAll(connection, ReadToken);
    }

    private static Token ReadToken(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            MembershipId = reader.ReadGuid(1),
            Key = reader.ReadString(2),
            Description = reader.ReadString(3),
            Ordinal = reader.ReadInt32(4),
        };
    }
}