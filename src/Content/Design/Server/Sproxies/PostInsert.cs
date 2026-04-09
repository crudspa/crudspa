namespace Crudspa.Content.Design.Server.Sproxies;

public static class PostInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Post post)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.PostInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@BlogId", post.BlogId);
        command.AddParameter("@Title", 150, post.Title);
        command.AddParameter("@StatusId", post.StatusId);
        command.AddParameter("@Author", 150, post.Author);
        command.AddParameter("@Published", post.Published);
        command.AddParameter("@Revised", post.Revised);
        command.AddParameter("@CommentRule", (Int32?)post.CommentRule);
        command.AddParameter("@PageTypeId", post.Page.TypeId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}