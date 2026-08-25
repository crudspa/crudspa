namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ForumLicenseSelectForLicense
{
    public static async Task<IList<ForumLicense>> Execute(String connection, Guid? sessionId, Guid? licenseId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ForumLicenseSelectForLicense";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@LicenseId", licenseId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            var forumLicenses = new List<ForumLicense>();

            while (await reader.ReadAsync())
                forumLicenses.Add(ReadForumLicense(reader));

            return forumLicenses;
        });
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