namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class SchoolContactSelectWhereForSchool
{
    public static async Task<IList<SchoolContact>> Execute(String connection, Guid? sessionId, SchoolContactSearch search)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.SchoolContactSelectWhereForSchool";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@SchoolId", search.ParentId);
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
            SchoolId = reader.ReadGuid(3),
            TitleId = reader.ReadGuid(4),
            TitleName = reader.ReadString(5),
            TestAccount = reader.ReadBoolean(6),
            Treatment = reader.ReadBoolean(7),
            ContactId = reader.ReadGuid(8),
            UserId = reader.ReadGuid(9),
        };
    }
}