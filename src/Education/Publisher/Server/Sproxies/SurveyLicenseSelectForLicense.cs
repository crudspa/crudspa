namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class SurveyLicenseSelectForLicense
{
    public static async Task<IList<SurveyLicense>> Execute(String connection, Guid? sessionId, Guid? licenseId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.SurveyLicenseSelectForLicense";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@LicenseId", licenseId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            var surveyLicenses = new List<SurveyLicense>();

            while (await reader.ReadAsync())
                surveyLicenses.Add(ReadSurveyLicense(reader));

            return surveyLicenses;
        });
    }

    private static SurveyLicense ReadSurveyLicense(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            LicenseId = reader.ReadGuid(1),
            SurveyId = reader.ReadGuid(2),
            SurveyTitle = reader.ReadString(3),
        };
    }
}