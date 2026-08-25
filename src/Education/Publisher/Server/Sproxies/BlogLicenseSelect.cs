namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class BlogLicenseSelect
{
    public static async Task<BlogLicense?> Execute(String connection, Guid? sessionId, BlogLicense blogLicense)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.BlogLicenseSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", blogLicense.Id);

        return await command.ReadSingle(connection, ReadBlogLicense);
    }

    private static BlogLicense ReadBlogLicense(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            LicenseId = reader.ReadGuid(1),
            BlogId = reader.ReadGuid(2),
            BlogTitle = reader.ReadString(3),
        };
    }
}