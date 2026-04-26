
using Thread = Crudspa.Content.Display.Shared.Contracts.Data.Thread;
using Comment = Crudspa.Content.Display.Shared.Contracts.Data.Comment;
namespace Crudspa.Content.Design.Server.Sproxies;

public static class CommentSelectTreeForThread
{
    public static async Task<IList<Comment>> Execute(String connection, Guid? sessionId, Guid? threadId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.CommentSelectTreeForThread";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ThreadId", threadId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            var comments = new List<Comment>();

            while (await reader.ReadAsync())
                comments.Add(ReadComment(reader));

            await reader.NextResultAsync();

            var commentMedias = new List<CommentMedia>();

            while (await reader.ReadAsync())
                commentMedias.Add(ReadCommentMedia(reader));

            foreach (var comment in comments)
                comment.CommentMedias = commentMedias.Where(x => x.CommentId.Equals(comment.Id)).ToObservable();
            return comments;
        });
    }

    private static Comment ReadComment(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            PostId = reader.ReadGuid(1),
            ParentId = reader.ReadGuid(2),
            ParentBody = reader.ReadString(3),
            Body = reader.ReadString(4),
            ById = reader.ReadGuid(5),
            ByFirstName = reader.ReadString(6),
            Posted = reader.ReadDateTimeOffset(7),
            Edited = reader.ReadDateTimeOffset(8),
            ThreadId = reader.ReadGuid(9),
        };
    }

    private static CommentMedia ReadCommentMedia(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            CommentId = reader.ReadGuid(1),
            CommentBody = reader.ReadString(2),
            Type = reader.ReadEnum<CommentMedia.Types>(3),
            AudioFile = new()
            {
                Id = reader.ReadGuid(4),
                BlobId = reader.ReadGuid(5),
                Name = reader.ReadString(6),
                Format = reader.ReadString(7),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(8),
                OptimizedBlobId = reader.ReadGuid(9),
                OptimizedFormat = reader.ReadString(10),
            },
            ImageFile = new()
            {
                Id = reader.ReadGuid(11),
                BlobId = reader.ReadGuid(12),
                Name = reader.ReadString(13),
                Format = reader.ReadString(14),
                Width = reader.ReadInt32(15),
                Height = reader.ReadInt32(16),
                Caption = reader.ReadString(17),
            },
            PdfFile = new()
            {
                Id = reader.ReadGuid(18),
                BlobId = reader.ReadGuid(19),
                Name = reader.ReadString(20),
                Format = reader.ReadString(21),
                Description = reader.ReadString(22),
            },
            VideoFile = new()
            {
                Id = reader.ReadGuid(23),
                BlobId = reader.ReadGuid(24),
                Name = reader.ReadString(25),
                Format = reader.ReadString(26),
                Width = reader.ReadInt32(27),
                Height = reader.ReadInt32(28),
                OptimizedStatus = reader.ReadEnum<VideoFile.OptimizationStatus>(29),
                OptimizedBlobId = reader.ReadGuid(30),
                OptimizedFormat = reader.ReadString(31),
            },
        Ordinal = reader.ReadInt32(32),
        };
    }
}