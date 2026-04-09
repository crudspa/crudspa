namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class UnitLicenseUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, UnitLicense unitLicense)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.UnitLicenseUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", unitLicense.Id);
        command.AddParameter("@UnitId", unitLicense.UnitId);

        await command.Execute(connection, transaction);
    }
}