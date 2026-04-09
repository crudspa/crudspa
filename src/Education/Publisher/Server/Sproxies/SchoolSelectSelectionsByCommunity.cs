namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class SchoolSelectSelectionsByCommunity
{
    public static async Task<IList<Selectable>> Execute(String connection, Guid? sessionId, Guid? communityId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.SchoolSelectSelectionsByCommunity";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@CommunityId", communityId);

        return await command.ReadSelectables(connection);
    }
}