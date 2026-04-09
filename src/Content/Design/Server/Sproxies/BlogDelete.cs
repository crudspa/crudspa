namespace Crudspa.Content.Design.Server.Sproxies;

public static class BlogDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Blog blog)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.BlogDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", blog.Id);

        await command.Execute(connection, transaction);
    }
}