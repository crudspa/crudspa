namespace Crudspa.Content.Design.Server.Sproxies;

public static class EmailTemplateSelectWhereForMembership
{
    public static async Task<IList<EmailTemplate>> Execute(String connection, Guid? sessionId, EmailTemplateSearch search)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.EmailTemplateSelectWhereForMembership";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@MembershipId", search.ParentId);
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
            MembershipId = reader.ReadGuid(3),
            Title = reader.ReadString(4),
            Subject = reader.ReadString(5),
            Body = reader.ReadString(6),
        };
    }
}