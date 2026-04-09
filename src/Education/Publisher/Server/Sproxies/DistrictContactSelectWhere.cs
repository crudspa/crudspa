namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class DistrictContactSelectWhere
{
    public static async Task<IList<DistrictContact>> Execute(String connection, Guid? sessionId, DistrictContactSearch search)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.DistrictContactSelectWhere";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PageNumber", search.Paged.PageNumber);
        command.AddParameter("@PageSize", search.Paged.PageSize);
        command.AddParameter("@SearchText", 50, search.Text);
        command.AddParameter("@SortField", search.Sort.Field);
        command.AddParameter("@SortAscending", search.Sort.Ascending);
        command.AddParameter("@Districts", search.Districts);

        return await command.ReadAll(connection, ReadDistrictContact);
    }

    private static DistrictContact ReadDistrictContact(SqlDataReader reader)
    {
        return new()
        {
            TotalCount = reader.ReadInt32(1),
            Id = reader.ReadGuid(2),
            DistrictId = reader.ReadGuid(3),
            Title = reader.ReadString(4),
            UserId = reader.ReadGuid(5),
            ContactId = reader.ReadGuid(6),
            DistrictName = reader.ReadString(7),
        };
    }
}