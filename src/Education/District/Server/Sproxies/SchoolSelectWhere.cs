namespace Crudspa.Education.District.Server.Sproxies;

public static class SchoolSelectWhere
{
    public static async Task<IList<School>> Execute(String connection, Guid? sessionId, SchoolSearch search)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationDistrict.SchoolSelectWhere";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PageNumber", search.Paged.PageNumber);
        command.AddParameter("@PageSize", search.Paged.PageSize);
        command.AddParameter("@SearchText", 50, search.Text);
        command.AddParameter("@SortField", search.Sort.Field);
        command.AddParameter("@SortAscending", search.Sort.Ascending);
        command.AddParameter("@Communities", search.Communities);

        return await command.ReadAll(connection, ReadSchool);
    }

    private static School ReadSchool(SqlDataReader reader)
    {
        return new()
        {
            TotalCount = reader.ReadInt32(1),
            Id = reader.ReadGuid(2),
            Key = reader.ReadString(3),
            CommunityId = reader.ReadGuid(4),
            CommunityName = reader.ReadString(5),
            Treatment = reader.ReadBoolean(6),
            AddressId = reader.ReadGuid(7),
            OrganizationId = reader.ReadGuid(8),
            ClassroomCount = reader.ReadInt32(9),
            SchoolContactCount = reader.ReadInt32(10),
        };
    }
}