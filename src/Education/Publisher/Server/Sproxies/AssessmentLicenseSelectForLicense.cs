namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class AssessmentLicenseSelectForLicense
{
    public static async Task<IList<AssessmentLicense>> Execute(String connection, Guid? sessionId, Guid? licenseId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.AssessmentLicenseSelectForLicense";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@LicenseId", licenseId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            var assessmentLicenses = new List<AssessmentLicense>();

            while (await reader.ReadAsync())
                assessmentLicenses.Add(ReadAssessmentLicense(reader));

            return assessmentLicenses;
        });
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