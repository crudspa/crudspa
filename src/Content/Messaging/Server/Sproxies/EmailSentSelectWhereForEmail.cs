namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class EmailSentSelectWhereForEmail
{
    public static async Task<IList<EmailSent>> Execute(String connection, Guid? sessionId, EmailSentSearch search)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.EmailSentSelectWhereForEmail";

        search.ProcessedRange.ResolveDates(search.TimeZoneId!);

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@EmailId", search.ParentId);
        command.AddParameter("@PageNumber", search.Paged.PageNumber);
        command.AddParameter("@PageSize", search.Paged.PageSize);
        command.AddParameter("@SearchText", 50, search.Text);
        command.AddParameter("@SortField", search.Sort.Field);
        command.AddParameter("@SortAscending", search.Sort.Ascending);
        command.AddParameter("@Statuses", search.Statuses);
        command.AddParameter("@ProcessedStart", search.ProcessedRange.StartDateTimeOffset);
        command.AddParameter("@ProcessedEnd", search.ProcessedRange.EndDateTimeOffset);

        return await command.ReadAll(connection, ReadEmailSent);
    }

    private static EmailSent ReadEmailSent(SqlDataReader reader)
    {
        return new()
        {
            TotalCount = reader.ReadInt32(1),
            Id = reader.ReadGuid(2),
            EmailId = reader.ReadGuid(3),
            RecipientId = reader.ReadGuid(4),
            RecipientEmail = reader.ReadString(5),
            Processed = reader.ReadDateTimeOffset(6),
            Status = reader.ReadEnum<EmailSent.Statuses>(7),
            ApiResponse = reader.ReadString(8),
            RecipientFirstName = reader.ReadString(9),
            RecipientLastName = reader.ReadString(10),
        };
    }
}