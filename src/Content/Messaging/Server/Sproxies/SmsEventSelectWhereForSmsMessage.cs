namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class SmsEventSelectWhereForSmsMessage
{
    public static async Task<IList<SmsEvent>> Execute(String connection, Guid? sessionId, SmsEventSearch search)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.SmsEventSelectWhereForSmsMessage";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@SmsMessageId", search.ParentId);
        command.AddParameter("@PageNumber", search.Paged.PageNumber);
        command.AddParameter("@PageSize", search.Paged.PageSize);
        command.AddParameter("@SearchText", 50, search.Text);
        command.AddParameter("@SortField", search.Sort.Field);
        command.AddParameter("@SortAscending", search.Sort.Ascending);

        return await command.ReadAll(connection, ReadSmsEvent);
    }

    private static SmsEvent ReadSmsEvent(SqlDataReader reader)
    {
        return new()
        {
            TotalCount = reader.ReadInt32(1),
            Id = reader.ReadGuid(2),
            SmsMessageId = reader.ReadGuid(3),
            ProviderMessageId = reader.ReadString(4),
            Provider = reader.ReadEnum<SmsEvent.Providers>(5),
            Type = reader.ReadEnum<SmsEvent.Types>(6),
            ProviderStatus = reader.ReadString(7),
            Received = reader.ReadDateTimeOffset(8),
            Status = reader.ReadEnum<SmsEvent.Statuses>(9),
        };
    }
}