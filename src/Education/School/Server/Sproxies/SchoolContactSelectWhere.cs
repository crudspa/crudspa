namespace Crudspa.Education.School.Server.Sproxies;

public static class SchoolContactSelectWhere
{
    public static async Task<IList<SchoolContact>> Execute(String connection, Guid? sessionId, SchoolContactSearch search)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.SchoolContactSelectWhere";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PageNumber", search.Paged.PageNumber);
        command.AddParameter("@PageSize", search.Paged.PageSize);
        command.AddParameter("@SearchText", 50, search.Text);
        command.AddParameter("@SortField", search.Sort.Field);
        command.AddParameter("@SortAscending", search.Sort.Ascending);

        return await command.ReadAll(connection, ReadSchoolContact);
    }

    private static SchoolContact ReadSchoolContact(SqlDataReader reader)
    {
        return new()
        {
            TotalCount = reader.ReadInt32(1),
            Id = reader.ReadGuid(2),
            TitleId = reader.ReadGuid(3),
            TitleName = reader.ReadString(4),
            TestAccount = reader.ReadBoolean(5),
            ContactId = reader.ReadGuid(6),
            UserId = reader.ReadGuid(7),
        };
    }
}