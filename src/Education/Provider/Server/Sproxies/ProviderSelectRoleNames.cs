namespace Crudspa.Education.Provider.Server.Sproxies;

public static class ProviderSelectRoleNames
{
    public static async Task<IList<Named>> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationProvider.ProviderSelectRoleNames";

        command.AddParameter("@SessionId", sessionId);

        return await command.ReadNameds(connection);
    }
}