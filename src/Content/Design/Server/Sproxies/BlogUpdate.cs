namespace Crudspa.Content.Design.Server.Sproxies;

public static class BlogUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Blog blog)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.BlogUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", blog.Id);
        command.AddParameter("@Title", 150, blog.Title);
        command.AddParameter("@StatusId", blog.StatusId);
        command.AddParameter("@Author", 150, blog.Author);
        command.AddParameter("@Description", blog.Description);
        command.AddParameter("@ImageId", blog.ImageFile.Id);

        await command.Execute(connection, transaction);
    }
}