namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class BlogLicenseUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, BlogLicense blogLicense)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.BlogLicenseUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", blogLicense.Id);
        command.AddParameter("@BlogId", blogLicense.BlogId);

        await command.Execute(connection, transaction);
    }
}