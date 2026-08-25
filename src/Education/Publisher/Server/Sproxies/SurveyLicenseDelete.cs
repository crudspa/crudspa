namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class SurveyLicenseDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SurveyLicense surveyLicense)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.SurveyLicenseDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", surveyLicense.Id);

        await command.Execute(connection, transaction);
    }
}