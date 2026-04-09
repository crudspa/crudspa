namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class DistrictLicenseSelect
{
    public static async Task<DistrictLicense?> Execute(String connection, Guid? sessionId, DistrictLicense districtLicense)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.DistrictLicenseSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", districtLicense.Id);

        return await command.ExecuteQuery(connection, async reader =>
        {
            if (!await reader.ReadAsync())
                return null;

            districtLicense = ReadDistrictLicense(reader);

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                districtLicense.Schools.Add(reader.ReadSelectable());

            return districtLicense;
        });
    }

    private static DistrictLicense ReadDistrictLicense(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            LicenseId = reader.ReadGuid(1),
            DistrictId = reader.ReadGuid(2),
            DistrictName = reader.ReadString(3),
            AllSchools = reader.ReadBoolean(4),
        };
    }
}