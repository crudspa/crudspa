namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class EmailTemplateSelectWhereForPortal
{
    public static async Task<IList<EmailTemplate>> Execute(String connection, Guid? sessionId, EmailTemplateSearch search)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.EmailTemplateSelectWhereForPortal";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PortalId", search.ParentId);
        command.AddParameter("@PageNumber", search.Paged.PageNumber);
        command.AddParameter("@PageSize", search.Paged.PageSize);
        command.AddParameter("@SearchText", 50, search.Text);
        command.AddParameter("@SortField", search.Sort.Field);
        command.AddParameter("@SortAscending", search.Sort.Ascending);

        return await command.ReadAll(connection, ReadEmailTemplate);
    }

    private static EmailTemplate ReadEmailTemplate(SqlDataReader reader)
    {
        return new()
        {
            TotalCount = reader.ReadInt32(1),
            Id = reader.ReadGuid(2),
            PortalId = reader.ReadGuid(3),
            MembershipId = reader.ReadGuid(4),
            OrganizationId = reader.ReadGuid(5),
            Title = reader.ReadString(6),
            Subject = reader.ReadString(7),
            Body = reader.ReadString(8),
        };
    }
}