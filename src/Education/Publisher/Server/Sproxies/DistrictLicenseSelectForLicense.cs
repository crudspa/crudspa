namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class DistrictLicenseSelectForLicense
{
    public static async Task<IList<DistrictLicense>> Execute(String connection, Guid? sessionId, Guid? licenseId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.DistrictLicenseSelectForLicense";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@LicenseId", licenseId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            var districtLicenses = new List<DistrictLicense>();

            while (await reader.ReadAsync())
                districtLicenses.Add(ReadDistrictLicense(reader));

            await reader.NextResultAsync();

            var schools = new List<Selectable>();

            while (await reader.ReadAsync())
                schools.Add(reader.ReadSelectable());

            foreach (var districtLicense in districtLicenses)
            {
                foreach (var selectable in schools.Where(x => x.RootId.Equals(districtLicense.Id)))
                    districtLicense.Schools.Add(selectable);
            }

            return districtLicenses;
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