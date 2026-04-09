namespace Crudspa.Education.Provider.Server.Sproxies;

public static class ProviderContactInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ProviderContact providerContact)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationProvider.ProviderContactInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ContactId", providerContact.ContactId);
        command.AddParameter("@UserId", providerContact.UserId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}