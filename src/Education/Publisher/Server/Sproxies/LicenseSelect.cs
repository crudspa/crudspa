namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class LicenseSelect
{
    public static async Task<License?> Execute(String connection, Guid? sessionId, License license)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.LicenseSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", license.Id);

        return await command.ReadSingle(connection, ReadLicense);
    }

    private static License ReadLicense(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Name = reader.ReadString(1),
            Description = reader.ReadString(2),
            DistrictLicenseCount = reader.ReadInt32(3),
            UnitLicenseCount = reader.ReadInt32(4),
        };
    }
}