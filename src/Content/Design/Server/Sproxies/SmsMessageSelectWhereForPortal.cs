namespace Crudspa.Content.Design.Server.Sproxies;

public static class SmsMessageSelectWhereForPortal
{
    public static async Task<IList<SmsMessage>> Execute(String connection, Guid? sessionId, SmsMessageSearch search)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.SmsMessageSelectWhereForPortal";

        search.OccurredRange.ResolveDates(search.TimeZoneId!);

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PortalId", search.ParentId);
        command.AddParameter("@PageNumber", search.Paged.PageNumber);
        command.AddParameter("@PageSize", search.Paged.PageSize);
        command.AddParameter("@SearchText", 50, search.Text);
        command.AddParameter("@SortField", search.Sort.Field);
        command.AddParameter("@SortAscending", search.Sort.Ascending);
        command.AddParameter("@OccurredStart", search.OccurredRange.StartDateTimeOffset);
        command.AddParameter("@OccurredEnd", search.OccurredRange.EndDateTimeOffset);
        command.AddParameter("@Direction", search.Direction.HasValue ? (Int32?)search.Direction.Value : null);

        return await command.ReadAll(connection, ReadSmsMessage);
    }

    private static SmsMessage ReadSmsMessage(SqlDataReader reader)
    {
        return new()
        {
            TotalCount = reader.ReadInt32(1),
            Id = reader.ReadGuid(2),
            MembershipId = reader.ReadGuid(3),
            PortalId = reader.ReadGuid(4),
            SmsId = reader.ReadGuid(5),
            SmsChannelKey = reader.ReadString(6),
            MemberId = reader.ReadGuid(7),
            Body = reader.ReadString(8),
            Direction = reader.ReadEnum<SmsMessage.Directions>(9),
            Occurred = reader.ReadDateTimeOffset(10),
            FromNumber = reader.ReadString(11),
            ToNumber = reader.ReadString(12),
            Status = reader.ReadEnum<SmsMessage.Statuses>(13),
            ProviderMessageId = reader.ReadString(14),
            Provider = reader.ReadEnum<SmsMessage.Providers>(15),
            ApiResponse = reader.ReadString(16),
            ContactPhoneId = reader.ReadGuid(17),
            ContactId = reader.ReadGuid(18),
            ContactFirstName = reader.ReadString(19),
            ContactLastName = reader.ReadString(20),
        };
    }
}