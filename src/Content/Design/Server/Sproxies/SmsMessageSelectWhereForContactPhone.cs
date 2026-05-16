namespace Crudspa.Content.Design.Server.Sproxies;

public static class SmsMessageSelectWhereForContactPhone
{
    public static async Task<IList<SmsMessage>> Execute(String connection, Guid? sessionId, SmsMessageSearch search)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.SmsMessageSelectWhereForContactPhone";

        search.OccurredRange.ResolveDates(search.TimeZoneId!);

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ContactPhoneId", search.ParentId);
        command.AddParameter("@PageNumber", search.Paged.PageNumber);
        command.AddParameter("@PageSize", search.Paged.PageSize);
        command.AddParameter("@SearchText", 50, search.Text);
        command.AddParameter("@SortField", search.Sort.Field);
        command.AddParameter("@SortAscending", search.Sort.Ascending);
        command.AddParameter("@OccurredStart", search.OccurredRange.StartDateTimeOffset);
        command.AddParameter("@OccurredEnd", search.OccurredRange.EndDateTimeOffset);

        return await command.ReadAll(connection, ReadSmsMessage);
    }

    private static SmsMessage ReadSmsMessage(SqlDataReader reader)
    {
        return new()
        {
            TotalCount = reader.ReadInt32(1),
            Id = reader.ReadGuid(2),
            ContactPhoneId = reader.ReadGuid(3),
            Body = reader.ReadString(4),
            Direction = reader.ReadEnum<SmsMessage.Directions>(5),
            Occurred = reader.ReadDateTimeOffset(6),
            FromNumber = reader.ReadString(7),
            ToNumber = reader.ReadString(8),
            Status = reader.ReadEnum<SmsMessage.Statuses>(9),
        };
    }
}