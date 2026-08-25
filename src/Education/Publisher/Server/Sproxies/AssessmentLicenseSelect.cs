namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class AssessmentLicenseSelect
{
    public static async Task<AssessmentLicense?> Execute(String connection, Guid? sessionId, AssessmentLicense assessmentLicense)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.AssessmentLicenseSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", assessmentLicense.Id);

        return await command.ReadSingle(connection, ReadAssessmentLicense);
    }

    private static AssessmentLicense ReadAssessmentLicense(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            LicenseId = reader.ReadGuid(1),
            AssessmentId = reader.ReadGuid(2),
            AssessmentName = reader.ReadString(3),
        };
    }
}