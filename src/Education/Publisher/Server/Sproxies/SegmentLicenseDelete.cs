namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class SegmentLicenseDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SegmentLicense segmentLicense)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.SegmentLicenseDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", segmentLicense.Id);

        await command.Execute(connection, transaction);
    }
}