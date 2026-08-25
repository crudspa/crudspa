namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ForumLicenseUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ForumLicense forumLicense)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ForumLicenseUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", forumLicense.Id);
        command.AddParameter("@ForumId", forumLicense.ForumId);

        await command.Execute(connection, transaction);
    }
}