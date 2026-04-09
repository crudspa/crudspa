namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class LicenseInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, License license)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.LicenseInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Name", 50, license.Name);
        command.AddParameter("@Description", license.Description);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}