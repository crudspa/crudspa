namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class BlogLicenseInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, BlogLicense blogLicense)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.BlogLicenseInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@LicenseId", blogLicense.LicenseId);
        command.AddParameter("@BlogId", blogLicense.BlogId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}