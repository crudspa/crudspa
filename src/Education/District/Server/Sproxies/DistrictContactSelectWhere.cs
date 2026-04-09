namespace Crudspa.Education.District.Server.Sproxies;

public static class DistrictContactSelectWhere
{
    public static async Task<IList<DistrictContact>> Execute(String connection, Guid? sessionId, DistrictContactSearch search)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationDistrict.DistrictContactSelectWhere";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PageNumber", search.Paged.PageNumber);
        command.AddParameter("@PageSize", search.Paged.PageSize);
        command.AddParameter("@SearchText", 50, search.Text);
        command.AddParameter("@SortField", search.Sort.Field);
        command.AddParameter("@SortAscending", search.Sort.Ascending);

        return await command.ReadAll(connection, ReadDistrictContact);
    }

    private static DistrictContact ReadDistrictContact(SqlDataReader reader)
    {
        return new()
        {
            TotalCount = reader.ReadInt32(1),
            Id = reader.ReadGuid(2),
            Title = reader.ReadString(3),
            UserId = reader.ReadGuid(4),
            ContactId = reader.ReadGuid(5),
        };
    }
}