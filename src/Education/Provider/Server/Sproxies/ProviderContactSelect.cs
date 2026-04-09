namespace Crudspa.Education.Provider.Server.Sproxies;

public static class ProviderContactSelect
{
    public static async Task<ProviderContact?> Execute(String connection, Guid? sessionId, ProviderContact providerContact)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationProvider.ProviderContactSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", providerContact.Id);

        return await command.ReadSingle(connection, ReadProviderContact);
    }

    private static ProviderContact ReadProviderContact(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            UserId = reader.ReadGuid(1),
            ContactId = reader.ReadGuid(2),
        };
    }
}