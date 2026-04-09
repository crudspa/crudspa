namespace Crudspa.Content.Design.Server.Sproxies;

public static class EmailSelectWhereForMembership
{
    public static async Task<IList<Email>> Execute(String connection, Guid? sessionId, EmailSearch search)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.EmailSelectWhereForMembership";

        search.SendRange.ResolveDates(search.TimeZoneId!);
        search.ProcessedRange.ResolveDates(search.TimeZoneId!);

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@MembershipId", search.ParentId);
        command.AddParameter("@PageNumber", search.Paged.PageNumber);
        command.AddParameter("@PageSize", search.Paged.PageSize);
        command.AddParameter("@SearchText", 50, search.Text);
        command.AddParameter("@SortField", search.Sort.Field);
        command.AddParameter("@SortAscending", search.Sort.Ascending);
        command.AddParameter("@SendStart", search.SendRange.StartDateTimeOffset);
        command.AddParameter("@SendEnd", search.SendRange.EndDateTimeOffset);
        command.AddParameter("@ProcessedStart", search.ProcessedRange.StartDateTimeOffset);
        command.AddParameter("@ProcessedEnd", search.ProcessedRange.EndDateTimeOffset);

        return await command.ReadAll(connection, ReadEmail);
    }

    private static Email ReadEmail(SqlDataReader reader)
    {
        return new()
        {
            TotalCount = reader.ReadInt32(1),
            Id = reader.ReadGuid(2),
            MembershipId = reader.ReadGuid(3),
            Subject = reader.ReadString(4),
            FromName = reader.ReadString(5),
            FromEmail = reader.ReadString(6),
            TemplateId = reader.ReadGuid(7),
            TemplateTitle = reader.ReadString(8),
            Send = reader.ReadDateTimeOffset(9),
            Body = reader.ReadString(10),
            Status = reader.ReadEnum<Email.Statuses>(11),
            Processed = reader.ReadDateTimeOffset(12),
        };
    }
}