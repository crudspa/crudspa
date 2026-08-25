namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class AssessmentLicenseUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, AssessmentLicense assessmentLicense)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.AssessmentLicenseUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", assessmentLicense.Id);
        command.AddParameter("@AssessmentId", assessmentLicense.AssessmentId);

        await command.Execute(connection, transaction);
    }
}