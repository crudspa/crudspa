namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class LicenseUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, License license)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.LicenseUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", license.Id);
        command.AddParameter("@Name", 50, license.Name);
        command.AddParameter("@Description", license.Description);

        await command.Execute(connection, transaction);
    }
}