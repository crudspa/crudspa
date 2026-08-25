namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class SurveyLicenseInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SurveyLicense surveyLicense)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.SurveyLicenseInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@LicenseId", surveyLicense.LicenseId);
        command.AddParameter("@SurveyId", surveyLicense.SurveyId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}