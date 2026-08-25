namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ForumLicenseInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ForumLicense forumLicense)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ForumLicenseInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@LicenseId", forumLicense.LicenseId);
        command.AddParameter("@ForumId", forumLicense.ForumId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}