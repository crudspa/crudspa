namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class LicenseSelectWhere
{
    public static async Task<IList<License>> Execute(String connection, Guid? sessionId, LicenseSearch search)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.LicenseSelectWhere";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PageNumber", search.Paged.PageNumber);
        command.AddParameter("@PageSize", search.Paged.PageSize);
        command.AddParameter("@SearchText", 50, search.Text);
        command.AddParameter("@SortField", search.Sort.Field);
        command.AddParameter("@SortAscending", search.Sort.Ascending);

        return await command.ReadAll(connection, ReadLicense);
    }

    private static License ReadLicense(SqlDataReader reader)
    {
        return new()
        {
            TotalCount = reader.ReadInt32(1),
            Id = reader.ReadGuid(2),
            Name = reader.ReadString(3),
            Description = reader.ReadString(4),
            DistrictLicenseCount = reader.ReadInt32(5),
            UnitLicenseCount = reader.ReadInt32(6),
        };
    }
}