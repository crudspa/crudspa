namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class GuidePageSelectForPage
{
    public static async Task<GuidePage?> Execute(String connection, Guid? sessionId, Guid? pageId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.GuidePageSelectForPage";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PageId", pageId);

        return await command.ReadSingle(connection, GuideDataReaders.ReadGuidePage);
    }
}