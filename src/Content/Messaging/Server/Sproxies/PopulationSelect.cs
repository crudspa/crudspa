namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class PopulationSelect
{
    public static async Task<Population?> Execute(String connection, Guid? sessionId, Population population)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.PopulationSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", population.Id);

        return await command.ReadSingle(connection, ReadPopulation);
    }

    private static Population ReadPopulation(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            PortalId = reader.ReadGuid(1),
            Key = reader.ReadString(2),
            Name = reader.ReadString(3),
            Description = reader.ReadString(4),
            SupportsOptOut = reader.ReadBoolean(5) ?? false,
            ResolverKey = reader.ReadString(6),
        };
    }
}