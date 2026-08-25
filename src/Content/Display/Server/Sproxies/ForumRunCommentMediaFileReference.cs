namespace Crudspa.Content.Display.Server.Sproxies;

public static class ForumRunCommentMediaFileReference
{
    public static async Task<Boolean> Exists(String connection, CommentMedia.Types type, Guid fileId)
    {
        await using var command = new SqlCommand { CommandText = "ContentDisplay.ForumRunCommentMediaFileIsReferenced" };
        command.AddParameter("@Type", type);
        command.AddParameter("@FileId", fileId);
        return await command.ReadSingle(connection, reader => reader.ReadBoolean(0)) == true;
    }
}