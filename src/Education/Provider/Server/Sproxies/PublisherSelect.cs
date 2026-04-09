namespace Crudspa.Education.Provider.Server.Sproxies;

public static class PublisherSelect
{
    public static async Task<Publisher?> Execute(String connection, Guid? sessionId, Publisher publisher)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationProvider.PublisherSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", publisher.Id);

        return await command.ReadSingle(connection, ReadPublisher);
    }

    private static Publisher ReadPublisher(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            OrganizationId = reader.ReadGuid(1),
            PublisherContactCount = reader.ReadInt32(2),
        };
    }
}