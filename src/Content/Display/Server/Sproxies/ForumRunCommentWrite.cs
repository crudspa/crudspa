namespace Crudspa.Content.Display.Server.Sproxies;

public static class ForumRunCommentWrite
{
    public static async Task<Guid?> Insert(SqlConnection connection, SqlTransaction? transaction,
        Guid? sessionId, Comment comment, IEnumerable<Guid?> licenseIds)
    {
        await using var command = new SqlCommand { CommandText = "ContentDisplay.ForumRunCommentInsert" };
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ThreadId", comment.ThreadId);
        command.AddParameter("@ParentId", comment.ParentId);
        command.AddParameter("@Body", comment.Body);
        command.AddParameter("@TagIds", SelectedTagIds(comment.Tags));
        command.AddParameter("@LicenseIds", licenseIds);
        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }

    public static async Task Update(SqlConnection connection, SqlTransaction? transaction,
        Guid? sessionId, Comment comment, IEnumerable<Guid?> licenseIds)
    {
        await using var command = new SqlCommand { CommandText = "ContentDisplay.ForumRunCommentUpdate" };
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", comment.Id);
        command.AddParameter("@Body", comment.Body);
        command.AddParameter("@TagIds", SelectedTagIds(comment.Tags));
        command.AddParameter("@LicenseIds", licenseIds);
        await command.Execute(connection, transaction);
    }

    public static async Task Delete(SqlConnection connection, SqlTransaction? transaction,
        Guid? sessionId, Guid? id, IEnumerable<Guid?> licenseIds)
    {
        await using var command = new SqlCommand { CommandText = "ContentDisplay.ForumRunCommentDelete" };
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