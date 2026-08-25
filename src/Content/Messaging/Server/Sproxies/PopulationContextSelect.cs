namespace Crudspa.Content.Messaging.Server.Sproxies;

public class PopulationContext
{
    public Population Population { get; set; } = new();
    public Guid? MembershipId { get; set; }
}

public static class PopulationContextSelect
{
    public static async Task<PopulationContext?> Execute(
        String connection,
        Guid? sessionId,
        PopulationRefresh refresh)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.PopulationContextSelect";
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PopulationId", refresh.PopulationId);
        command.AddParameter("@OrganizationId", refresh.OrganizationId);
        command.AddParameter("@ActivationScopeId", refresh.ActivationScopeId);

        return await command.ReadSingle(connection, reader => new PopulationContext
        {
            Population = new()
            {
                Id = reader.GetGuid(0),
                PortalId = reader.GetGuid(1),
                Key = reader.GetString(2),
                Name = reader.GetString(3),
                Description = reader.IsDBNull(4) ? null : reader.GetString(4),
                SupportsOptOut = reader.GetBoolean(5),
                ResolverKey = reader.GetString(6),
            },
            MembershipId = reader.IsDBNull(7) ? null : reader.GetGuid(7),
        });
    }
}