using Crudspa.Content.Display.Server;

namespace Crudspa.Content.Display.Server.Sproxies;

public static class ForumRunCommentSelect
{
    public static async Task<IList<Comment>> Execute(String connection, Guid? sessionId, Guid? threadId, IEnumerable<Guid?> licenseIds)
    {
        await using var command = new SqlCommand { CommandText = "ContentDisplay.ForumRunCommentSelectTree" };
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ThreadId", threadId);
        command.AddParameter("@LicenseIds", licenseIds);

        return await command.ExecuteQuery(connection, async reader =>
        {
            var comments = new List<Comment>();
            while (await reader.ReadAsync()) comments.Add(ForumRunDataReader.ReadComment(reader));

            await reader.NextResultAsync();
            var media = new List<CommentMedia>();
            while (await reader.ReadAsync()) media.Add(ForumRunDataReader.ReadCommentMedia(reader));

            await reader.NextResultAsync();
            var reactions = new List<CommentReaction>();
            while (await reader.ReadAsync()) reactions.Add(ForumRunDataReader.ReadReaction(reader));

            await reader.NextResultAsync();
            while (await reader.ReadAsync())
            {
                var comment = comments.FirstOrDefault(x => x.Id == reader.ReadGuid(0));
                var tagId = reader.ReadGuid(1);
                if (comment is not null && tagId.HasValue && comment.Tags.All(x => x.Id != tagId))
                    comment.Tags.Add(ForumRunDataReader.ReadTag(reader, 1));
            }

            foreach (var comment in comments)
            {
                comment.CommentMedias = media.Where(x => x.CommentId == comment.Id).OrderBy(x => x.Ordinal).ToObservable();
                comment.Reactions = reactions.Where(x => x.CommentId == comment.Id).ToObservable();
            }

            return comments;
        });
    }
}