namespace Crudspa.Education.Provider.Server.Sproxies;

public static class ProviderContactDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ProviderContact providerContact)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationProvider.ProviderContactDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", providerContact.Id);

        await command.Execute(connection, transaction);
    }
}