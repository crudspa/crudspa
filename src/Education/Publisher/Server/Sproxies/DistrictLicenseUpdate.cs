namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class DistrictLicenseUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, DistrictLicense districtLicense)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.DistrictLicenseUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", districtLicense.Id);
        command.AddParameter("@DistrictId", districtLicense.DistrictId);

        await command.Execute(connection, transaction);
    }
}