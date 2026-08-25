namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class AssessmentLicenseInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, AssessmentLicense assessmentLicense)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.AssessmentLicenseInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@LicenseId", assessmentLicense.LicenseId);
        command.AddParameter("@AssessmentId", assessmentLicense.AssessmentId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}