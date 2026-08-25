namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class SurveyLicenseSelect
{
    public static async Task<SurveyLicense?> Execute(String connection, Guid? sessionId, SurveyLicense surveyLicense)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.SurveyLicenseSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", surveyLicense.Id);

        return await command.ReadSingle(connection, ReadSurveyLicense);
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