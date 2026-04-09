namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class DistrictLicenseUpdateRelations
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, DistrictLicense districtLicense)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.DistrictLicenseUpdateRelations";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", districtLicense.Id);
        command.AddParameter("@AllSchools", districtLicense.AllSchools ?? true);
        command.AddParameter("@Schools", districtLicense.Schools);

        await command.Execute(connection, transaction);
    }
}