namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class MessageSelectWhereForMembership
{
    public static async Task<IList<Message>> Execute(String connection, Guid? sessionId, MessageSearch search)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.MessageSelectWhereForMembership";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@MembershipId", search.ParentId);
        command.AddParameter("@PageNumber", search.Paged.PageNumber);
        command.AddParameter("@PageSize", search.Paged.PageSize);
        command.AddParameter("@SearchText", 50, search.Text);
        command.AddParameter("@SortField", search.Sort.Field);
        command.AddParameter("@SortAscending", search.Sort.Ascending);

        return await command.ReadAll(connection, ReadMessage);
    }

    private static Message ReadMessage(SqlDataReader reader)
    {
        return new()
        {
            TotalCount = reader.ReadInt32(1),
            Id = reader.ReadGuid(2),
            MembershipId = reader.ReadGuid(3),
            StageId = reader.ReadGuid(4),
            StageName = reader.ReadString(5),
            ActivationId = reader.ReadGuid(6),
            EmailId = reader.ReadGuid(7),
            SmsId = reader.ReadGuid(8),
        };
    }
}