namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class SegmentLicenseInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SegmentLicense segmentLicense)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.SegmentLicenseInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@LicenseId", segmentLicense.LicenseId);
        command.AddParameter("@SegmentId", segmentLicense.SegmentId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}