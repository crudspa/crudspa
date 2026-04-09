namespace Crudspa.Education.Publisher.Server.Sproxies;

using Publisher = Shared.Contracts.Data.Publisher;

public static class PublisherSelect
{
    public static async Task<Publisher?> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.PublisherSelect";

        command.AddParameter("@SessionId", sessionId);

        return await command.ReadSingle(connection, ReadPublisher);
    }

    private static Publisher ReadPublisher(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            OrganizationId = reader.ReadGuid(1),
        };
    }
}