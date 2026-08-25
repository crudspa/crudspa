namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class BlogLicenseSelectForLicense
{
    public static async Task<IList<BlogLicense>> Execute(String connection, Guid? sessionId, Guid? licenseId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.BlogLicenseSelectForLicense";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@LicenseId", licenseId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            var blogLicenses = new List<BlogLicense>();

            while (await reader.ReadAsync())
                blogLicenses.Add(ReadBlogLicense(reader));

            return blogLicenses;
        });
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