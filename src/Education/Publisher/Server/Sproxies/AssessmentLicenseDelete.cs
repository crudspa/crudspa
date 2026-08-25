namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class AssessmentLicenseDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, AssessmentLicense assessmentLicense)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.AssessmentLicenseDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", assessmentLicense.Id);

        await command.Execute(connection, transaction);
    }
}