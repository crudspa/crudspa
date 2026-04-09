namespace Crudspa.Content.Design.Server.Sproxies;

public static class PostUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Post post)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.PostUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", post.Id);
        command.AddParameter("@Title", 150, post.Title);
        command.AddParameter("@StatusId", post.StatusId);
        command.AddParameter("@Author", 150, post.Author);
        command.AddParameter("@Published", post.Published);
        command.AddParameter("@Revised", post.Revised);
        command.AddParameter("@CommentRule", (Int32?)post.CommentRule);
        command.AddParameter("@PageTypeId", post.Page.TypeId);

        await command.Execute(connection, transaction);
    }
}