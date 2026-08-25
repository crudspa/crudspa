namespace Crudspa.Content.Design.Server.Sproxies;

public static class ForumCoverFileAuthorize
{
    public static async Task<Boolean> Execute(String connection, Guid? sessionId, Guid? portalId,
        Guid? forumId, Guid? imageId, Guid? blobId)
    {
        await using var command = new SqlCommand { CommandText = "ContentDesign.ForumCoverFileAuthorize" };
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PortalId", portalId);
        command.AddParameter("@ForumId", forumId);
        command.AddParameter("@ImageId", imageId);
        command.AddParameter("@BlobId", blobId);

        return await command.ReadSingle(connection, reader => reader.ReadBoolean(0)) == true;
    }
}