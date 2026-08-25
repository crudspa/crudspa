namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class SegmentLicenseUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SegmentLicense segmentLicense)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.SegmentLicenseUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", segmentLicense.Id);
        command.AddParameter("@SegmentId", segmentLicense.SegmentId);

        await command.Execute(connection, transaction);
    }
}