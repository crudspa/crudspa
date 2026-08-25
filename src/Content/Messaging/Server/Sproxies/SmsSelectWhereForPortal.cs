namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class SmsSelectWhereForPortal
{
    public static async Task<IList<Sms>> Execute(String connection, Guid? sessionId, SmsSearch search)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.SmsSelectWhereForPortal";

        search.SendRange.ResolveDates(search.TimeZoneId!);
        search.ProcessedRange.ResolveDates(search.TimeZoneId!);

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PortalId", search.ParentId);
        command.AddParameter("@PageNumber", search.Paged.PageNumber);
        command.AddParameter("@PageSize", search.Paged.PageSize);
        command.AddParameter("@SearchText", 50, search.Text);
        command.AddParameter("@SortField", search.Sort.Field);
        command.AddParameter("@SortAscending", search.Sort.Ascending);
        command.AddParameter("@SendStart", search.SendRange.StartDateTimeOffset);
        command.AddParameter("@SendEnd", search.SendRange.EndDateTimeOffset);
        command.AddParameter("@ProcessedStart", search.ProcessedRange.StartDateTimeOffset);
        command.AddParameter("@ProcessedEnd", search.ProcessedRange.EndDateTimeOffset);

        return await command.ReadAll(connection, ReadSms);
    }

    private static Sms ReadSms(SqlDataReader reader)
    {
        return new()
        {
            TotalCount = reader.ReadInt32(1),
            Id = reader.ReadGuid(2),
            MembershipId = reader.ReadGuid(3),
            MembershipName = reader.ReadString(4),
            Body = reader.ReadString(5),
            Send = reader.ReadDateTimeOffset(6),
            Status = reader.ReadEnum<Sms.Statuses>(7),
            Processed = reader.ReadDateTimeOffset(8),
        };
    }
}