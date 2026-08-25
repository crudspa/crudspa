namespace Crudspa.Content.Display.Server.Sproxies;

public static class ForumRunUploadStage
{
    public static async Task<ForumUploadStageResults> Insert(String connection, Guid? sessionId, Guid? forumId,
        ForumUploadStage upload, IEnumerable<Guid?> licenseIds)
    {
        await using var command = new SqlCommand { CommandText = "ContentDisplay.ForumRunUploadStageInsert" };
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ForumId", forumId);
        command.AddParameter("@BlobId", upload.BlobId);
        command.AddParameter("@Type", upload.Type);
        command.AddParameter("@Name", 150, upload.Name);
        command.AddParameter("@Format", 10, upload.Format);
        command.AddParameter("@ContentType", 100, upload.ContentType);
        command.AddParameter("@Bytes", upload.Bytes);
        command.AddParameter("@LicenseIds", licenseIds);

        var result = await command.ReadSingle(connection, reader => reader.ReadInt32(0));
        return (ForumUploadStageResults)result.GetValueOrDefault();
    }

    public static async Task<ForumUploadStage?> Consume(String connection, Guid? sessionId, Guid? forumId,
        CommentMedia.Types type, Guid? blobId, IEnumerable<Guid?> licenseIds)
    {
        await using var command = new SqlCommand { CommandText = "ContentDisplay.ForumRunUploadStageConsume" };
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ForumId", forumId);
        command.AddParameter("@BlobId", blobId);
        command.AddParameter("@Type", type);
        command.AddParameter("@LicenseIds", licenseIds);

        return await command.ReadSingle(connection, reader => new ForumUploadStage
        {
            BlobId = reader.ReadGuid(0),
            Type = reader.ReadEnum<CommentMedia.Types>(1),
            Name = reader.ReadString(2),
            Format = reader.ReadString(3),
            ContentType = reader.ReadString(4),
            Bytes = reader.ReadInt64(5).GetValueOrDefault(),
        });
    }

    public static async Task Discard(String connection, Guid? sessionId, Guid? blobId)
    {
        await using var command = new SqlCommand { CommandText = "ContentDisplay.ForumRunUploadStageDiscard" };
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@BlobId", blobId);
        await command.Execute(connection);
    }

    public static async Task<IList<Guid>> FetchExpired(String connection)
    {
        await using var command = new SqlCommand { CommandText = "ContentDisplay.ForumRunUploadStageSelectExpired" };
        return await command.ReadAll(connection, reader => reader.ReadGuid(0)!.Value);
    }

    public static async Task DiscardExpired(String connection, Guid blobId)
    {
        await using var command = new SqlCommand { CommandText = "ContentDisplay.ForumRunUploadStageDiscardExpired" };
        command.AddParameter("@BlobId", blobId);
        await command.Execute(connection);
    }
}