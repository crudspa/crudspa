using Thread = Crudspa.Content.Display.Shared.Contracts.Data.Thread;

namespace Crudspa.Content.Display.Server.Sproxies;

public static class ForumRunThreadWrite
{
    public static async Task<(Guid? ThreadId, Guid? CommentId)> Insert(SqlConnection connection, SqlTransaction? transaction,
        Guid? sessionId, Thread thread, IEnumerable<Guid?> licenseIds)
    {
        await using var command = new SqlCommand { CommandText = "ContentDisplay.ForumRunThreadInsert" };
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ForumId", thread.ForumId);
        command.AddParameter("@Title", 150, thread.Title);
        command.AddParameter("@CommentBody", thread.Comment.Body);
        command.AddParameter("@TagIds", SelectedTagIds(thread.Tags));
        command.AddParameter("@LicenseIds", licenseIds);

        var threadId = command.AddOutputParameter("@Id");
        var commentId = command.AddOutputParameter("@CommentId");
        await command.Execute(connection, transaction);
        return ((Guid?)threadId.Value, (Guid?)commentId.Value);
    }

    public static async Task Update(SqlConnection connection, SqlTransaction? transaction,
        Guid? sessionId, Thread thread, IEnumerable<Guid?> licenseIds)
    {
        await using var command = new SqlCommand { CommandText = "ContentDisplay.ForumRunThreadUpdate" };
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", thread.Id);
        command.AddParameter("@Title", 150, thread.Title);
        command.AddParameter("@Pinned", thread.Pinned ?? false);
        command.AddParameter("@CommentBody", thread.Comment.Body);
        command.AddParameter("@TagIds", SelectedTagIds(thread.Tags));
        command.AddParameter("@LicenseIds", licenseIds);
        await command.Execute(connection, transaction);
    }

    public static async Task Delete(SqlConnection connection, SqlTransaction? transaction,
        Guid? sessionId, Guid? id, IEnumerable<Guid?> licenseIds)
    {
        await using var command = new SqlCommand { CommandText = "ContentDisplay.ForumRunThreadDelete" };
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", id);
        command.AddParameter("@LicenseIds", licenseIds);
        await command.Execute(connection, transaction);
    }

    private static IEnumerable<Guid?> SelectedTagIds(IEnumerable<Selectable> tags) => tags
        .Where(x => x.Selected == true && x.Id.HasValue)
        .Select(x => x.Id)
        .Distinct();
}