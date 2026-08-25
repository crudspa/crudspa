namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class PopulationRefreshForActivationSelect
{
    public static async Task<IList<PopulationRefresh>> Execute(
        String connection, Guid? sessionId, Guid? activationId)
    {
        await using var command = new SqlCommand("ContentMessaging.PopulationRefreshForActivationSelect");
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ActivationId", activationId);

        return await command.ReadAll(connection, reader => new PopulationRefresh
        {
            PopulationId = reader.ReadGuid(0),
            OrganizationId = reader.ReadGuid(1),
            ActivationScopeId = reader.ReadGuid(2),
        });
    }
}