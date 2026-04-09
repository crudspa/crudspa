namespace Crudspa.Education.Provider.Server.Sproxies;

public static class PublisherContactSelect
{
    public static async Task<PublisherContact?> Execute(String connection, Guid? sessionId, PublisherContact publisherContact)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationProvider.PublisherContactSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", publisherContact.Id);

        return await command.ReadSingle(connection, ReadPublisherContact);
    }

    private static PublisherContact ReadPublisherContact(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            PublisherId = reader.ReadGuid(1),
            UserId = reader.ReadGuid(2),
            ContactId = reader.ReadGuid(3),
        };
    }
}