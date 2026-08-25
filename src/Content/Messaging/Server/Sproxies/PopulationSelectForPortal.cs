namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class PopulationSelectForPortal
{
    public static async Task<IList<Population>> Execute(String connection, Guid? sessionId, Guid? portalId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.PopulationSelectForPortal";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PortalId", portalId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            var populations = new List<Population>();

            while (await reader.ReadAsync())
                populations.Add(ReadPopulation(reader));

            return populations;
        });
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