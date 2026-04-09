namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class LicenseDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, License license)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.LicenseDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", license.Id);

        await command.Execute(connection, transaction);
    }
}