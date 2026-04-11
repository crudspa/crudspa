namespace Crudspa.Content.Design.Server.Sproxies;

public static class SectionSelectIdsForPage
{
    public static async Task<IList<Guid?>> Execute(String connection, Guid? sessionId, Guid? pageId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.SectionSelectIdsForPage";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PageId", pageId);

        return await command.ReadAll(connection, reader => reader.ReadGuid(0));
    }
}