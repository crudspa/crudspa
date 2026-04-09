namespace Crudspa.Content.Design.Server.Sproxies;

public static class SectionSelectPageId
{
    public static async Task<Guid?> Execute(String connection, Guid? sessionId, Guid? sectionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.SectionSelectPageId";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", sectionId);

        var output = command.AddOutputParameter("@PageId");
        await command.Execute(connection);

        return output.Value is Guid pageId
            ? pageId
            : null;
    }
}