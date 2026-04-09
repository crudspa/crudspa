namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class UnitLicenseDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, UnitLicense unitLicense)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.UnitLicenseDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", unitLicense.Id);

        await command.Execute(connection, transaction);
    }
}