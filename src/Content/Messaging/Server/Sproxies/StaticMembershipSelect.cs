namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class StaticMembershipSelect
{
    public static IList<PopulationToken> CreateTokens() =>
    [
        new() { Key = "FirstName", Description = "Recipient first name", Ordinal = 0 },
        new() { Key = "LastName", Description = "Recipient last name", Ordinal = 1 },
        new() { Key = "Email", Description = "Recipient email address", Ordinal = 2 },
    ];

    public static async Task<PopulationResult> Execute(
        String connection, Guid? sessionId, Guid? portalId, Guid? populationId, Guid organizationId)
    {
        await using var command = new SqlCommand("ContentMessaging.StaticMembershipSelect");
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PortalId", portalId);
        command.AddParameter("@PopulationId", populationId);
        command.AddParameter("@OrganizationId", organizationId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            var result = new PopulationResult
            {
                Tokens = CreateTokens(),
            };

            while (await reader.ReadAsync())
            {
                var contactId = reader.ReadGuid(0)!.Value;
                result.Members.Add(new() { ContactId = contactId });
                result.TokenValues.Add(new() { ContactId = contactId, Key = "FirstName", Value = reader.ReadString(1) });
                result.TokenValues.Add(new() { ContactId = contactId, Key = "LastName", Value = reader.ReadString(2) });
                result.TokenValues.Add(new() { ContactId = contactId, Key = "Email", Value = reader.ReadString(3) });
            }

            return result;
        });
    }
}