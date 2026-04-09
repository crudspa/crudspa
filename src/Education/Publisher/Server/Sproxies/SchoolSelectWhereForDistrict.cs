namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class SchoolSelectWhereForDistrict
{
    public static async Task<IList<School>> Execute(String connection, Guid? sessionId, SchoolSearch search)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.SchoolSelectWhereForDistrict";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@DistrictId", search.ParentId);
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
            DistrictId = reader.ReadGuid(3),
            Key = reader.ReadString(4),
            CommunityId = reader.ReadGuid(5),
            CommunityName = reader.ReadString(6),
            Treatment = reader.ReadBoolean(7),
            OrganizationId = reader.ReadGuid(8),
            AddressId = reader.ReadGuid(9),
            SchoolContactCount = reader.ReadInt32(10),
        };
    }
}