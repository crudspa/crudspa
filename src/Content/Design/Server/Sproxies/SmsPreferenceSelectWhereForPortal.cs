namespace Crudspa.Content.Design.Server.Sproxies;

public static class SmsPreferenceSelectWhereForPortal
{
    public static async Task<IList<SmsPreference>> Execute(String connection, Guid? sessionId, SmsPreferenceSearch search)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.SmsPreferenceSelectWhereForPortal";

        search.StatusChangedRange.ResolveDates(search.TimeZoneId!);

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PortalId", search.ParentId);
        command.AddParameter("@PageNumber", search.Paged.PageNumber);
        command.AddParameter("@PageSize", search.Paged.PageSize);
        command.AddParameter("@SearchText", 50, search.Text);
        command.AddParameter("@SortField", search.Sort.Field);
        command.AddParameter("@SortAscending", search.Sort.Ascending);
        command.AddParameter("@StatusChangedStart", search.StatusChangedRange.StartDateTimeOffset);
        command.AddParameter("@StatusChangedEnd", search.StatusChangedRange.EndDateTimeOffset);

        return await command.ReadAll(connection, ReadSmsPreference);
    }

    private static SmsPreference ReadSmsPreference(SqlDataReader reader)
    {
        return new()
        {
            TotalCount = reader.ReadInt32(1),
            Id = reader.ReadGuid(2),
            PortalId = reader.ReadGuid(3),
            Number = reader.ReadString(4),
            Status = reader.ReadEnum<SmsPreference.Statuses>(5),
            Source = reader.ReadEnum<SmsPreference.Sources>(6),
            StatusChanged = reader.ReadDateTimeOffset(7),
            Notes = reader.ReadString(8),
        };
    }
}