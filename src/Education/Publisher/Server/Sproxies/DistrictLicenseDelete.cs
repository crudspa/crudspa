namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class DistrictLicenseDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, DistrictLicense districtLicense)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.DistrictLicenseDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", districtLicense.Id);

        await command.Execute(connection, transaction);
    }
}