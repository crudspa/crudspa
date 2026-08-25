namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ForumLicenseDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ForumLicense forumLicense)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ForumLicenseDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", forumLicense.Id);

        await command.Execute(connection, transaction);
    }
}