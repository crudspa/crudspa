using MemberSearch = Crudspa.Content.Design.Shared.Contracts.Data.MemberSearch;

namespace Crudspa.Content.Design.Server.Sproxies;

public static class MemberSelectWhereForMembership
{
    public static async Task<IList<Member>> Execute(String connection, Guid? sessionId, MemberSearch search)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.MemberSelectWhereForMembership";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@MembershipId", search.ParentId);
        command.AddParameter("@PageNumber", search.Paged.PageNumber);
        command.AddParameter("@PageSize", search.Paged.PageSize);
        command.AddParameter("@SearchText", 50, search.Text);
        command.AddParameter("@SortField", search.Sort.Field);
        command.AddParameter("@SortAscending", search.Sort.Ascending);

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
            TotalCount = reader.ReadInt32(1),
            Id = reader.ReadGuid(2),
            MembershipId = reader.ReadGuid(3),
            Status = reader.ReadEnum<Member.Statuses>(4),
            Contact = new()
            {
                Id = reader.ReadGuid(5),
                FirstName = reader.ReadString(6),
                LastName = reader.ReadString(7),
                Email = reader.ReadString(8),
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