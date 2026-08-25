namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class SegmentLicenseSelect
{
    public static async Task<SegmentLicense?> Execute(String connection, Guid? sessionId, SegmentLicense segmentLicense)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.SegmentLicenseSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", segmentLicense.Id);

        return await command.ReadSingle(connection, ReadSegmentLicense);
    }

    private static SegmentLicense ReadSegmentLicense(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            LicenseId = reader.ReadGuid(1),
            SegmentId = reader.ReadGuid(2),
            SegmentTitle = reader.ReadString(3),
        };
    }
}