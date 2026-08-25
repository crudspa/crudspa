namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class BlogLicenseDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, BlogLicense blogLicense)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.BlogLicenseDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", blogLicense.Id);

        await command.Execute(connection, transaction);
    }
}