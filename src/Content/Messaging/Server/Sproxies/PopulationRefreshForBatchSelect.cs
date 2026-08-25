namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class PopulationRefreshForBatchSelect
{
    public static async Task<IList<PopulationRefresh>> Execute(
        String connection, Guid? sessionId, Guid? batchId)
    {
        await using var command = new SqlCommand("ContentMessaging.PopulationRefreshForBatchSelect");
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@BatchId", batchId);

        return await command.ReadAll(connection, reader => new PopulationRefresh
        {
            PopulationId = reader.ReadGuid(0),
            OrganizationId = reader.ReadGuid(1),
            ActivationScopeId = reader.ReadGuid(2),
        });
    }
}