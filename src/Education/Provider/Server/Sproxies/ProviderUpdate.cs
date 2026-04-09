namespace Crudspa.Education.Provider.Server.Sproxies;

using Provider = Shared.Contracts.Data.Provider;

public static class ProviderUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Provider provider)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationProvider.ProviderUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", provider.Id);

        await command.Execute(connection, transaction);
    }
}