namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class PublisherContactSelect
{
    public static async Task<PublisherContact?> Execute(String connection, Guid? sessionId, PublisherContact publisherContact)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.PublisherContactSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", publisherContact.Id);

        return await command.ReadSingle(connection, ReadPublisherContact);
    }

    private static PublisherContact ReadPublisherContact(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            UserId = reader.ReadGuid(1),
            ContactId = reader.ReadGuid(2),
        };
    }
}