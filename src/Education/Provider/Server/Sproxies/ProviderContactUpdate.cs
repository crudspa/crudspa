namespace Crudspa.Education.Provider.Server.Sproxies;

public static class ProviderContactUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ProviderContact providerContact)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationProvider.ProviderContactUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", providerContact.Id);
        command.AddParameter("@UserId", providerContact.UserId);

        await command.Execute(connection, transaction);
    }
}