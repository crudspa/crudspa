namespace Crudspa.Content.Design.Server.Sproxies;

public static class ForumDeleteFileSelect
{
    public sealed record File(CommentMedia.Types Type, Guid? Id);

    public sealed record Result(Guid? CoverImageId, IList<File> CommentMediaFiles);

    public static async Task<Result> Execute(String connection, Guid? sessionId, Guid? forumId)
    {
        await using var command = new SqlCommand { CommandText = "ContentDesign.ForumDeleteFileSelect" };
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ForumId", forumId);

        return await command.ExecuteQuery<Result>(connection, async reader =>
        {
            Guid? coverImageId = null;
            if (await reader.ReadAsync())
                coverImageId = reader.ReadGuid(0);

            await reader.NextResultAsync();

            var mediaFiles = new List<File>();
            while (await reader.ReadAsync())
                mediaFiles.Add(new(reader.ReadEnum<CommentMedia.Types>(0), reader.ReadGuid(1)));

            return new(coverImageId, mediaFiles);
        });
    }
}