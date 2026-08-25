namespace Crudspa.Content.Design.Server.Sproxies;

public static class ForumUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Forum forum)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.ForumUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", forum.Id);
        command.AddParameter("@Title", 150, forum.Title);
        command.AddParameter("@StatusId", forum.StatusId);
        command.AddParameter("@PermissionId", forum.PermissionId);
        command.AddParameter("@AccessMode", forum.AccessMode);
        command.AddParameter("@Description", forum.Description);
        command.AddParameter("@ImageId", forum.ImageFile.Id);
        command.AddParameter("@Licenses", forum.Licenses);
        command.AddStructuredParameter("@ForumBundles", "[Content].[ForumBundleList]", forum.ForumBundles.ToForumBundleTable());

        await command.Execute(connection, transaction);
    }
}