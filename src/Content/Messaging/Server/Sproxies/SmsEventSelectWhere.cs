namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class SmsEventSelectWhere
{
    public static async Task<IList<SmsEvent>> Execute(String connection, Guid? sessionId, SmsEventSearch search)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.SmsEventSelectWhere";

        search.ReceivedRange.ResolveDates(search.TimeZoneId!);

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PageNumber", search.Paged.PageNumber);
        command.AddParameter("@PageSize", search.Paged.PageSize);
        command.AddParameter("@SearchText", 50, search.Text);
        command.AddParameter("@SortField", search.Sort.Field);
        command.AddParameter("@SortAscending", search.Sort.Ascending);
        command.AddParameter("@ReceivedStart", search.ReceivedRange.StartDateTimeOffset);
        command.AddParameter("@ReceivedEnd", search.ReceivedRange.EndDateTimeOffset);

        return await command.ReadAll(connection, ReadSmsEvent);
    }

    private static SmsEvent ReadSmsEvent(SqlDataReader reader)
    {
        return new()
        {
            TotalCount = reader.ReadInt32(1),
            Id = reader.ReadGuid(2),
            ProviderMessageId = reader.ReadString(3),
            Provider = reader.ReadEnum<SmsEvent.Providers>(4),
            Type = reader.ReadEnum<SmsEvent.Types>(5),
            ProviderStatus = reader.ReadString(6),
            SignatureValid = reader.ReadBoolean(7),
            Received = reader.ReadDateTimeOffset(8),
            Processed = reader.ReadDateTimeOffset(9),
            Status = reader.ReadEnum<SmsEvent.Statuses>(10),
            ErrorMessage = reader.ReadString(11),
        };
    }
}