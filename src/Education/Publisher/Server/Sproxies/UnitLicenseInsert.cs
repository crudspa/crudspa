namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class UnitLicenseInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, UnitLicense unitLicense)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.UnitLicenseInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@LicenseId", unitLicense.LicenseId);
        command.AddParameter("@UnitId", unitLicense.UnitId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}