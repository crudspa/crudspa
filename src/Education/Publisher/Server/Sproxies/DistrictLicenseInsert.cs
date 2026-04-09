namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class DistrictLicenseInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, DistrictLicense districtLicense)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.DistrictLicenseInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@LicenseId", districtLicense.LicenseId);
        command.AddParameter("@DistrictId", districtLicense.DistrictId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}