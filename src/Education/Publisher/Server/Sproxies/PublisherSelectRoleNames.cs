namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class PublisherSelectRoleNames
{
    public static async Task<IList<Named>> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.PublisherSelectRoleNames";

        command.AddParameter("@SessionId", sessionId);

        return await command.ReadNameds(connection);
    }
}