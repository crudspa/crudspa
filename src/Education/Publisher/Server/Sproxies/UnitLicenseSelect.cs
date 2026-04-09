namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class UnitLicenseSelect
{
    public static async Task<UnitLicense?> Execute(String connection, Guid? sessionId, UnitLicense unitLicense)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.UnitLicenseSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", unitLicense.Id);

        return await command.ExecuteQuery(connection, async reader =>
        {
            if (!await reader.ReadAsync())
                return null;

            unitLicense = ReadUnitLicense(reader);

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                unitLicense.Books.Add(reader.ReadSelectable());

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                unitLicense.Lessons.Add(reader.ReadSelectable());

            return unitLicense;
        });
    }

    private static UnitLicense ReadUnitLicense(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            LicenseId = reader.ReadGuid(1),
            UnitId = reader.ReadGuid(2),
            UnitTitle = reader.ReadString(3),
            AllBooks = reader.ReadBoolean(4),
            AllLessons = reader.ReadBoolean(5),
        };
    }
}