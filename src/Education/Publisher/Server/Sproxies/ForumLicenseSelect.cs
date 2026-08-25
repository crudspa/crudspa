namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ForumLicenseSelect
{
    public static async Task<ForumLicense?> Execute(String connection, Guid? sessionId, ForumLicense forumLicense)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ForumLicenseSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", forumLicense.Id);

        return await command.ReadSingle(connection, ReadForumLicense);
    }

    private static ForumLicense ReadForumLicense(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            LicenseId = reader.ReadGuid(1),
            ForumId = reader.ReadGuid(2),
            ForumTitle = reader.ReadString(3),
        };
    }
}