namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class DistrictSelectWhere
{
    public static async Task<IList<District>> Execute(String connection, Guid? sessionId, DistrictSearch search)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.DistrictSelectWhere";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PageNumber", search.Paged.PageNumber);
        command.AddParameter("@PageSize", search.Paged.PageSize);
        command.AddParameter("@SearchText", 50, search.Text);
        command.AddParameter("@SortField", search.Sort.Field);
        command.AddParameter("@SortAscending", search.Sort.Ascending);

        return await command.ReadAll(connection, ReadDistrict);
    }

    private static District ReadDistrict(SqlDataReader reader)
    {
        return new()
        {
            TotalCount = reader.ReadInt32(1),
            Id = reader.ReadGuid(2),
            StudentIdNumberLabel = reader.ReadString(3),
            AssessmentExplainer = reader.ReadString(4),
            OrganizationId = reader.ReadGuid(5),
            AddressId = reader.ReadGuid(6),
            DistrictContactCount = reader.ReadInt32(7),
            CommunityCount = reader.ReadInt32(8),
            SchoolCount = reader.ReadInt32(9),
        };
    }
}