namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class PopulationMembershipEnsure
{
    public static async Task<Guid?> Execute(
        String connection, Guid? sessionId, Guid? populationId, Guid? organizationId, Guid? activationScopeId)
    {
        await using var command = new SqlCommand("ContentMessaging.PopulationMembershipEnsure");
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PopulationId", populationId);
        command.AddParameter("@OrganizationId", organizationId);
        command.AddParameter("@ActivationScopeId", activationScopeId);
        return await command.ReadSingle(connection, reader => reader.ReadGuid(0));
    }
}