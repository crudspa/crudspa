namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class SurveyLicenseUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SurveyLicense surveyLicense)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.SurveyLicenseUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", surveyLicense.Id);
        command.AddParameter("@SurveyId", surveyLicense.SurveyId);

        await command.Execute(connection, transaction);
    }
}