namespace Crudspa.Education.Provider.Server.Sproxies;

public static class PublisherSelectRoleNames
{
    public static async Task<IList<Named>> Execute(String connection, Guid? sessionId, Guid? publisherId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationProvider.PublisherSelectRoleNames";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PublisherId", publisherId);

        return await command.ReadNameds(connection);
    }
}