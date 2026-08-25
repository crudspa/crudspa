namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class SmsTemplateSelectWhereForPortal
{
    public static async Task<IList<SmsTemplate>> Execute(String connection, Guid? sessionId, SmsTemplateSearch search)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.SmsTemplateSelectWhereForPortal";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PortalId", search.ParentId);
        command.AddParameter("@PageNumber", search.Paged.PageNumber);
        command.AddParameter("@PageSize", search.Paged.PageSize);
        command.AddParameter("@SearchText", 50, search.Text);
        command.AddParameter("@SortField", search.Sort.Field);
        command.AddParameter("@SortAscending", search.Sort.Ascending);

        return await command.ReadAll(connection, ReadSmsTemplate);
    }

    private static SmsTemplate ReadSmsTemplate(SqlDataReader reader)
    {
        return new()
        {
            TotalCount = reader.ReadInt32(1),
            Id = reader.ReadGuid(2),
            PortalId = reader.ReadGuid(3),
            Title = reader.ReadString(4),
            Body = reader.ReadString(5),
        };
    }
}