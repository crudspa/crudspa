namespace Crudspa.Content.Design.Server.Sproxies;

public static class SegmentSelectPageId
{
    public static async Task<Guid?> Execute(String connection, Guid? sessionId, Guid? segmentId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.SegmentSelectPageId";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", segmentId);

        var output = command.AddOutputParameter("@PageId");
        await command.Execute(connection);

        return output.Value is Guid pageId
            ? pageId
            : null;
    }
}