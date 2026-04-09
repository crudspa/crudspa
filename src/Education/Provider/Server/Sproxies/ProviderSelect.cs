namespace Crudspa.Education.Provider.Server.Sproxies;

using Provider = Shared.Contracts.Data.Provider;

public static class ProviderSelect
{
    public static async Task<Provider?> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationProvider.ProviderSelect";

        command.AddParameter("@SessionId", sessionId);

        return await command.ReadSingle(connection, ReadProvider);
    }

    private static Provider ReadProvider(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            OrganizationId = reader.ReadGuid(1),
        };
    }
}