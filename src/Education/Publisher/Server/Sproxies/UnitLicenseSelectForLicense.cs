namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class UnitLicenseSelectForLicense
{
    public static async Task<IList<UnitLicense>> Execute(String connection, Guid? sessionId, Guid? licenseId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.UnitLicenseSelectForLicense";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@LicenseId", licenseId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            var unitLicenses = new List<UnitLicense>();

            while (await reader.ReadAsync())
                unitLicenses.Add(ReadUnitLicense(reader));

            await reader.NextResultAsync();

            var books = new List<Selectable>();

            while (await reader.ReadAsync())
                books.Add(reader.ReadSelectable());

            await reader.NextResultAsync();

            var lessons = new List<Selectable>();

            while (await reader.ReadAsync())
                lessons.Add(reader.ReadSelectable());

            foreach (var unitLicense in unitLicenses)
            {
                foreach (var selectable in books.Where(x => x.RootId.Equals(unitLicense.Id)))
                    unitLicense.Books.Add(selectable);
                foreach (var selectable in lessons.Where(x => x.RootId.Equals(unitLicense.Id)))
                    unitLicense.Lessons.Add(selectable);
            }

            return unitLicenses;
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