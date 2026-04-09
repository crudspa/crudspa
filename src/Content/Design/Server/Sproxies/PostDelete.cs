namespace Crudspa.Content.Design.Server.Sproxies;

public static class PostDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Post post)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.PostDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", post.Id);

        await command.Execute(connection, transaction);
    }
}